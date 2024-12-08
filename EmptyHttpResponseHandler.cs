/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Text;
using Ninject;
using GenXdev.MemoryManagement;
using GenXdev.Configuration;
using GenXdev.Buffers;
using GenXdev.AsyncSockets.Arguments;
using GenXdev.Helpers;
using System.Net;

namespace GenXdev.AsyncSockets.Handlers
{
    public class EmptyHttpResponseHandler : SocketHandlerBase, IHttpResponseHandler
    {
        #region Initialization

        [Inject]
        public EmptyHttpResponseHandler(
            IKernel Kernel,
            IMemoryManagerConfiguration MemoryConfiguration,
            IServiceMemoryManager MemoryManager,
            String ServiceName

#if (Logging)
, IServiceLogger Logger
#endif
, object Pool = null
)
            : base(Kernel, MemoryConfiguration, MemoryManager, ServiceName
#if (Logging)
, Logger
#endif
, Pool
            )
        { }

        protected override void SetupHandlers()
        {
            this.OnHandleInitialize += EmptyHttpResponseHandler_OnHandleInitialize;
            this.OnHandleReceive += EmptyHttpResponseHandler_OnHandleReceive;
            this.OnHandleSend += EmptyHttpResponseHandler_OnHandleSend;
            this.OnHandleAsyncAction += EmptyHttpResponseHandler_OnHandleAsyncAction;
            this.OnHandleReset += EmptyHttpResponseHandler_OnHandleReset;
        }

        #endregion

        #region Fields
        int BytesToReceiveFromClient;
        bool CloseConnection;
        bool HeadersOnly;
        string externalIP;

        // encodings
        UTF8Encoding UTF8Encoding = new UTF8Encoding(false, true);

        // frequently used constants
        static byte[] HttpHeaderOpening = (new UTF8Encoding(false, true)).GetBytes("HTTP/1.1 ");
        static byte[] HttpCacheControlHeader = (new UTF8Encoding(false, true)).GetBytes("Cache-Control: no-cache\r\n");
        static byte[] HttpACAllowOriginHeaderOpening = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Origin: ");
        static byte[] HttpContentTypeJsonHeader = (new UTF8Encoding(false, true)).GetBytes("Content-Type: application/json; charset=utf-8\r\n");
        static byte[] HttpACAllowMethodsHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Methods: GET, POST, OPTIONS, HEAD, PUT, CONNECT\r\n");
        static byte[] HttpACMaxAgeHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Max-Age: 86400\r\n");
        static byte[] HttpACAllowHeadersHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Headers: x-requested-with, content-type, accept, connection\r\n");
        static byte[] HttpACAllowHeadersWithWebsocketSupportHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Headers: x-requested-with, content-type, accept, connection, upgrade, sec-websocket-key, sec-websocket-protocol, sec-websocket-version\r\n");
        static byte[] HttpConnectionCloseHeader = (new UTF8Encoding(false, true)).GetBytes("Connection: close\r\n");
        static byte[] HttpContentLengthHeaderOpening = (new UTF8Encoding(false, true)).GetBytes("Content-Length: ");
        static byte[] HttpContentLength0Header = (new UTF8Encoding(false, true)).GetBytes("Content-Length: 0\r\n");
        static byte[] HttpHeaderClosing = (new UTF8Encoding(false, true)).GetBytes("\r\n");

        #endregion

        #region Properties

        public IHttpRequestSocketHandler HttpRequestSocketHandler { get; set; }
        public int HttpResultCode { get; set; }
        public string HttpStatusMessage { get; set; }
        public string Location { get; set; }
        public bool AddJsonTlsHostInfo { get; set; }
        public bool ForceDisconnect { get; set; }

        #endregion

        #region Handling

        void EmptyHttpResponseHandler_OnHandleInitialize(object sender, HandleSocketBEventArgs e)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Sending \"" + HttpResultCode.ToString() + " " + HttpStatusMessage + "\""; });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            BytesToReceiveFromClient = 0;
            if (HttpRequestSocketHandler.HttpRequestHeaders.IsValidRequest && (HttpRequestSocketHandler.HttpRequestHeaders != null) && (HttpRequestSocketHandler.HttpRequestHeaders.Method == "POST"))
            {
                // get content length
                if (!Int32.TryParse(HttpRequestSocketHandler.HttpRequestHeaders["content-length"], out BytesToReceiveFromClient))
                {
                    ForceDisconnect = true;
                    BytesToReceiveFromClient = 0;
                }
            }

            ForceDisconnect |= !String.IsNullOrWhiteSpace(Location);

            // set timeout
            SetActivityTimestamp(false);
            SetCurrentStageTimeoutSeconds(10, !ForceDisconnect);
            if ((HttpRequestSocketHandler.HttpRequestHeaders == null) || ForceDisconnect)
            {
                CloseConnection = true;
            }
            else
            {
                var connection = HttpRequestSocketHandler.HttpRequestHeaders.IsValidRequest ? (String)HttpRequestSocketHandler.HttpRequestHeaders["connection"] : "close";
                if ((!String.IsNullOrWhiteSpace(connection)) && (connection.ToLower() == "close"))
                {
                    CloseConnection = true;
                }
                if (HttpRequestSocketHandler.HttpRequestHeaders.IsValidRequest)
                    // check http method
                    switch (HttpRequestSocketHandler.HttpRequestHeaders.Method)
                    {
                        case "HEAD":
                        case "OPTIONS":
                            HeadersOnly = true;
                            break;

                        default:
                            HeadersOnly = false;
                            break;
                    }
            }

            e.NextAction = NextRequestedHandlerAction.AsyncAction;
        }

        void DetermineFirstAction(HandleSocketBEventArgs e)
        {
            var left = Math.Max(0, BytesToReceiveFromClient - e.RxBuffer.Count);
            var tooMuch = (left > 1024 * 1024);

            if (left > 0 && !tooMuch)
            {
                SetCurrentStageTimeoutSeconds(5, !tooMuch);

                e.Count = BytesToReceiveFromClient - e.RxBuffer.Count;
                e.NextAction = NextRequestedHandlerAction.Receive;
                return;
            }

            // initialize response
            InitializeHttpResponseHeaders(e.TxBuffer);

            e.NextAction = NextRequestedHandlerAction.Send;
        }

        void EmptyHttpResponseHandler_OnHandleReceive(object sender, HandleSocketBEventArgs e)
        {
            var left = Math.Max(0, BytesToReceiveFromClient - e.RxBuffer.Count);

            if (left > 0)
            {
                e.Count = left;
                e.NextAction = NextRequestedHandlerAction.Receive;
                return;
            }

            // initialize response
            InitializeHttpResponseHeaders(e.TxBuffer);

            e.NextAction = NextRequestedHandlerAction.Send;
        }

        void EmptyHttpResponseHandler_OnHandleSend(object sender, HandleSocketBEventArgs e)
        {
            // still sending headers?
            if (e.TxBuffer.Count > 0)
            {
                e.NextAction = NextRequestedHandlerAction.Send;
                return;
            }

            // all done!
            if (ForceDisconnect)
            {
                e.NextAction = NextRequestedHandlerAction.Disconnect;

                return;
            }

            // restore and continue http handler
            e.NextAction = NextRequestedHandlerAction.ReleaseControl;
        }

        async Task EmptyHttpResponseHandler_OnHandleAsyncAction(object sender, HandleSocketBEventArgs e)
        {
            externalIP = GenXdev.Helpers.Network.GetPublicExternalHostname(((IPEndPoint)saeaHandler.AcceptSocket.RemoteEndPoint).ToString());

            DetermineFirstAction(e);
        }

        void EmptyHttpResponseHandler_OnHandleReset(object sender, EventArgs e)
        {
            // dereference
            HttpRequestSocketHandler = null;
            HttpResultCode = 200;
            HttpStatusMessage = "OK";
            Location = null;
            ForceDisconnect = false;
            BytesToReceiveFromClient = 0;
            AddJsonTlsHostInfo = false;
            CloseConnection = false;
        }

        #endregion

        #region Private

        #region Response

        void InitializeHttpResponseHeaders(DynamicBuffer txBuffer)
        {
            // write http version and result code
            txBuffer.Add(HttpHeaderOpening);
            txBuffer.Add(UTF8Encoding.GetBytes(HttpResultCode.ToString() + " " + HttpStatusMessage));
            txBuffer.Add(HttpHeaderClosing);

            // set caching                
            txBuffer.Add(HttpCacheControlHeader);

            // add crossdomain security policy if requested
            if (HttpRequestSocketHandler.HttpRequestHeaders != null)
            {
                String origin = (String)HttpRequestSocketHandler.HttpRequestHeaders["origin"];
                if (!String.IsNullOrWhiteSpace(origin))
                {
                    // allow origin
                    txBuffer.Add(HttpACAllowOriginHeaderOpening);
                    txBuffer.Add(UTF8Encoding.GetBytes(origin));
                    txBuffer.Add(HttpHeaderClosing);
                    if (HttpRequestSocketHandler.HttpRequestHeaders.Method == "OPTIONS")
                    {
                        // allow methods
                        txBuffer.Add(HttpACAllowMethodsHeader);
                        txBuffer.Add(HttpACAllowHeadersHeader);

                        // max age
                        txBuffer.Add(HttpACMaxAgeHeader);
                    }
                }
            }

            var isWebSocket = (HttpRequestSocketHandler.HttpRequestHeaders != null) && (HttpRequestSocketHandler.HttpRequestHeaders["upgrade"].ToLower() == "websocket");
            var haveLocation = !String.IsNullOrWhiteSpace(Location);

            if (!HeadersOnly && haveLocation && AddJsonTlsHostInfo && !isWebSocket)
            {
                // lookup hostname to suggest
                var hostName = String.Empty;

                hostName = GenXdev.Helpers.Pki.GetPrimaryDnsName(

                    // match what is expected on the TLS certificate
                    HttpRequestSocketHandler.TLSConfiguration.TLS_CertificateUsage == Configuration.SocketHandlerTLSCertificateUsage.AutoGenerate ?
                        externalIP :
                        (HttpRequestSocketHandler.TLSConfiguration.TLS_AdditionalHostNames != null) && (HttpRequestSocketHandler.TLSConfiguration.TLS_AdditionalHostNames.Length > 0) ?
                        HttpRequestSocketHandler.TLSConfiguration.TLS_AdditionalHostNames[0] :
                        externalIP
                    ,
                    HttpRequestSocketHandler.TLSConfiguration.TLS_AdditionalHostNames
                );

                // change location's hostname
                string hostUri = HttpRequestSocketHandler.HttpRequestHeaders["host"];
                if (!string.IsNullOrWhiteSpace(hostUri)) hostUri = hostUri.Split(new char[] { ':' })[0].Trim().ToLowerInvariant();

                var newUri = new UriBuilder(Location);

                newUri.Host = GenXdev.Helpers.Pki.GetPrimaryDnsName(hostUri, HttpRequestSocketHandler.TLSConfiguration.TLS_AdditionalHostNames);
                newUri.Scheme = "https";

                Location = newUri.ToString();

                // construct json
                var uri = new Uri(Location);
                var json = UTF8Encoding.GetBytes(GenXdev.Helpers.Serialization.ToJsonAnonymous(
                    new
                    {
                        x_redirect = new
                        {
                            host = hostName,
                            port = uri.Port,
                            url = isWebSocket ? Location.Replace("http", "ws") : Location
                        }
                    }
                ));

                // write content type
                txBuffer.Add(HttpContentTypeJsonHeader);

                // write content length
                txBuffer.Add(HttpContentLengthHeaderOpening);
                txBuffer.Add(UTF8Encoding.GetBytes(json.Length.ToString()));
                txBuffer.Add(HttpHeaderClosing);

                // need to disconnect afterwards?
                if (CloseConnection)
                {
                    txBuffer.Add(HttpConnectionCloseHeader);
                }

                // end headers section
                txBuffer.Add(HttpHeaderClosing);

                txBuffer.Add(json);
            }
            else
            {
                if (haveLocation)
                {
                    txBuffer.Add((new UTF8Encoding(false, true)).GetBytes("Location: " + (isWebSocket ? Location.Replace("http", "ws") : Location) + "\r\n"));
                }

                if (!CloseConnection)
                {
                    // write content length
                    txBuffer.Add(HttpContentLength0Header);

                    // need to disconnect afterwards?
                    if (CloseConnection)
                    {
                        txBuffer.Add(HttpConnectionCloseHeader);
                    }
                }

                txBuffer.Add(HttpHeaderClosing);
            }
        }

        #endregion

        #endregion
    }
}
