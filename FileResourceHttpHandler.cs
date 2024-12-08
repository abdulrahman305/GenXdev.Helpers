using System.Text;
using Ninject;
using GenXdev.MemoryManagement;
using GenXdev.Configuration;
using GenXdev.Buffers;
using GenXdev.AsyncSockets.Arguments;

namespace GenXdev.AsyncSockets.Handlers
{
    public class FileResourceHttpHandler : SocketHandlerBase, IHttpResponseHandler
    {
        #region Initialization

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors"), Inject]
        public FileResourceHttpHandler(
            IKernel Kernel,
            IMemoryManagerConfiguration MemoryConfiguration,
            IServiceMemoryManager MemoryManager,
            String ServiceName

#if (Logging)
, IServiceLogger Logger
#endif
, bool KeepAlive = false
, object Pool = null

)
            : base(Kernel, MemoryConfiguration, MemoryManager, ServiceName
#if (Logging)
, Logger
#endif
, Pool
)
        {
            this.KeepAlive = KeepAlive;
        }

        protected override void SetupHandlers()
        {
            this.OnHandleInitialize += FileResourceHttpHandler_OnHandleInitialize;
            this.OnHandleSend += FileResourceHttpHandler_OnHandleSend;
            this.OnHandleAsyncAction += FileResourceHttpHandler_OnHandleAsyncAction;
            this.OnHandleReset += FileResourceHttpHandler_OnHandleReset;
        }

        #endregion

        #region Fields

        bool sendingHttpError;

        bool KeepAlive;
        bool SendFileNameHeader;
        long BytesToSendToClient;
        string ContentType = "";
        long ContentLength = 0;
        FileStream stream = null;
        byte[] buffer;

        // encodings
        UTF8Encoding UTF8Encoding = new UTF8Encoding(false, true);

        // frequently used constants
        static byte[] HttpHeaderOpening = (new UTF8Encoding(false, true)).GetBytes("HTTP/1.1 ");
        static byte[] HttpCacheControlHeader = (new UTF8Encoding(false, true)).GetBytes("Cache-Control: no-cache\r\n");
        static byte[] HttpACAllowOriginHeaderOpening = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Origin: ");
        static byte[] HttpACAllowMethodsHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Methods: GET, POST, OPTIONS, HEAD, PUT, CONNECT\r\n");
        static byte[] HttpACMaxAgeHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Max-Age: 86400\r\n");
        static byte[] HttpACAllowHeadersHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Headers: x-requested-with, content-type, accept, connection\r\n");
        static byte[] HttpACAllowHeadersWithWebsocketSupportHeader = (new UTF8Encoding(false, true)).GetBytes("Access-Control-Allow-Headers: x-requested-with, content-type, accept, connection, upgrade, sec-websocket-key, sec-websocket-protocol, sec-websocket-version\r\n");
        static byte[] HttpHeaderClosing = (new UTF8Encoding(false, true)).GetBytes("\r\n");
        static byte[] HttpConnectionKeepAlifeHeader = (new UTF8Encoding(false, true)).GetBytes("Connection: Keep-Alive\r\n");
        static byte[] HttpConnectionCloseHeader = (new UTF8Encoding(false, true)).GetBytes("Connection: Close\r\n");

        #endregion

        #region Properties

        public IHttpRequestSocketHandler HttpRequestSocketHandler { get; set; }
        public string Location { get; set; }

        #endregion

        #region Handling

        void FileResourceHttpHandler_OnHandleInitialize(object sender, HandleSocketBEventArgs e)
        {
            // control returning from EmptyResponseHandler?
            if (sendingHttpError)
            {
                e.NextAction = NextRequestedHandlerAction.ReleaseControl;
                return;
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "got new request";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetCurrentStageTimeoutSeconds(60, true);

            stream = new FileStream(Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            buffer = new byte[MemoryConfiguration.SendBufferSize];

            BytesToSendToClient = (new System.IO.FileInfo(Location)).Length;
            ContentLength = BytesToSendToClient;
            SetContentType();

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogHandlerFlowMessage(saeaHandler, () =>
            {
                return " >> FILE >> " + HttpRequestSocketHandler.HttpRequestHeaders.Location.OriginalString;
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            e.NextAction = StartSendingResponse(e.TxBuffer);
        }

        void FileResourceHttpHandler_OnHandleSend(object sender, HandleSocketBEventArgs e)
        {
            e.NextAction = NextRequestedHandlerAction.AsyncAction;
        }

        async Task FileResourceHttpHandler_OnHandleAsyncAction(object sender, HandleSocketBEventArgs e)
        {
            // still sending headers?
            if (e.TxBuffer.Count > 0)
            {
                // continue
                e.NextAction = NextRequestedHandlerAction.Send;
                return;
            }
            else
            {
                if (HttpRequestSocketHandler.HttpRequestHeaders.Method.ToUpper() != "HEAD")
                {
                    // still sending headers?
                    if (BytesToSendToClient > 0)
                    {
                        // add next block
                        var l = await stream.ReadAsync(buffer, 0, buffer.Length);
                        e.TxBuffer.Add(buffer, 0, l);
                        BytesToSendToClient -= l;

                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogSocketFlowMessage(saeaHandler, () =>
                        {
                            return "" + BytesToSendToClient.ToString() + " bytes left";
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------

                        // continue
                        e.NextAction = NextRequestedHandlerAction.Send;
                        return;
                    }
                }
            }

            // restore and continue http handler
            e.NextAction = NextRequestedHandlerAction.ReleaseControl;
        }


        void FileResourceHttpHandler_OnHandleReset(object sender, EventArgs e)
        {
            // reset flags
            sendingHttpError = false;

            // dereference
            HttpRequestSocketHandler = null;
            CloseStream();
        }

        private void SetContentType()
        {
            SendFileNameHeader = false;

            switch (Path.GetExtension(Location).ToLower())
            {
                case ".css":
                    ContentType = "text/css";
                    break;
                case ".html":
                    ContentType = "text/html";
                    break;
                case ".png":
                    ContentType = "image/png";
                    break;
                case ".js":
                    ContentType = "text/javascript";
                    break;
                case ".cer":
                    SendFileNameHeader = true;
                    ContentType = "application/x-x509-ca-cert";
                    break;
                case ".ico":
                    ContentType = "image/icon";
                    break;
                default:
                    SendFileNameHeader = true;
                    ContentType = "application/octet-stream";
                    break;
            }
        }

        void CloseStream()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        NextRequestedHandlerAction StartSendingResponse(DynamicBuffer TxBuffer)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Starting sending of response headers";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            // initialize response
            InitializeHttpResponseHeaders(TxBuffer);

            return NextRequestedHandlerAction.Send;
        }

        #endregion

        #region Private

        #region Response

        void InitializeHttpResponseHeaders(DynamicBuffer TxBuffer)
        {
            // write http version and result code
            TxBuffer.Add(HttpHeaderOpening);
            TxBuffer.Add(UTF8Encoding.GetBytes("200 OK"));
            TxBuffer.Add(HttpHeaderClosing);

            // set caching                
            TxBuffer.Add(HttpCacheControlHeader);

            // add crossdomain security policy if requested
            if (HttpRequestSocketHandler.HttpRequestHeaders != null)
            {
                String origin = (String)HttpRequestSocketHandler.HttpRequestHeaders["origin"];
                if (!String.IsNullOrWhiteSpace(origin))
                {
                    // allow origin
                    TxBuffer.Add(HttpACAllowOriginHeaderOpening);
                    TxBuffer.Add(UTF8Encoding.GetBytes(origin));
                    TxBuffer.Add(HttpHeaderClosing);
                    if (HttpRequestSocketHandler.HttpRequestHeaders.Method == "OPTIONS")
                    {
                        // allow methods
                        TxBuffer.Add(HttpACAllowMethodsHeader);

                        TxBuffer.Add(HttpACAllowHeadersHeader);

                        // max age
                        TxBuffer.Add(HttpACMaxAgeHeader);
                    }
                }
            }

            TxBuffer.Add((new UTF8Encoding(false, true)).GetBytes("Content-Type: " + ContentType + "\r\n"));
            if (KeepAlive)
            {
                TxBuffer.Add(HttpConnectionKeepAlifeHeader);
            }
            else
            {
                TxBuffer.Add(HttpConnectionCloseHeader);
            }

            TxBuffer.Add((new UTF8Encoding(false, true)).GetBytes("Content-Length: " + ContentLength.ToString() + "\r\n"));
            if (SendFileNameHeader)
            {
                TxBuffer.Add((new UTF8Encoding(false, true)).GetBytes("Content-Disposition: inline; filename=\"" + Path.GetFileName(this.Location) + "\"\r\n"));
            }

            TxBuffer.Add(HttpHeaderClosing);
        }

        #endregion

        #endregion

        #region Misc

        void SendHttpErrorMessage(HandleSocketBEventArgs e, int HttpResultCode, string HttpStatusMessage, bool ForceDisconnect = true)
        {
            // set flag
            sendingHttpError = true;

            // create new socket handler
            var newHandler = Kernel.Get<EmptyHttpResponseHandler>();

            // set handler properties
            newHandler.HttpRequestSocketHandler = HttpRequestSocketHandler;
            newHandler.HttpResultCode = HttpResultCode;
            newHandler.HttpStatusMessage = HttpStatusMessage;
            newHandler.ForceDisconnect = ForceDisconnect;

            // delegate
            e.NextAction = newHandler.TakeOverConrolOfSocket(saeaHandler);
        }

        #endregion
    }
}
