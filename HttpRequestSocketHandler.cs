/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Text;
using Ninject;
using GenXdev.MemoryManagement;
using GenXdev.AsyncSockets.Containers;
using GenXdev.AsyncSockets.Configuration;
using GenXdev.Configuration;
using GenXdev.AsyncSockets.Arguments;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;
using Org.BouncyCastle.Asn1.X509;

namespace GenXdev.AsyncSockets.Handlers
{
    public enum HttpRequestState { DeterminingIfClientRequestsTLS, NewRequest, ReadingHeaders, Sending100Continue, GotRequest, ReadingBody, SendingResponseStatusHeader, SendingResponseHeaders, SendingResponseBody, Websocket, ServerSendEventDispatcher }

    public enum SuitableContentEncoding { none, gzip, deflate };

    public class HttpRequestSocketHandler : SocketHandlerBase, IHttpRequestSocketHandler
    {
        #region Initialization

        [Inject]
        public HttpRequestSocketHandler(
            IKernel Kernel,
            IMemoryManagerConfiguration MemoryConfiguration,
            IServiceMemoryManager MemoryManager,
            String ServiceName

#if (Logging)
, IServiceLogger Logger
#endif
, object Pool = null
, ISocketHandlerTLSConfiguration TLSConfiguration = null
)
            : base(Kernel, MemoryConfiguration, MemoryManager, ServiceName
#if (Logging)
, Logger
#endif

, Pool
            )
        {
            this.TLSConfiguration = TLSConfiguration != null ? TLSConfiguration
                : Kernel.Get<ISocketHandlerTLSConfiguration>();

            if (String.IsNullOrWhiteSpace(this.TLSConfiguration.UniqueConfigurationName))
            {

                this.TLSConfiguration.UniqueConfigurationName = ServiceName;
            }


            this.HttpRequestHeaders = new HttpRequestHeaders(Kernel);
            IsFirstRequest = true;
        }

        protected override void SetupHandlers()
        {
            this.OnHandleInitialize += HttpRequestSocketHandler_OnHandleInitialize;

            this.OnHandleReceive += HttpRequestSocketHandler_OnHandleReceive;

            this.OnHandleSend += HttpRequestSocketHandler_OnHandleSend;

            this.OnHandleReset += HttpRequestSocketHandler_OnHandleReset;
        }

        #endregion

        #region Fields

        // misc
        int lastEOHsearchIndex;

        // state of handler
        protected HttpRequestState HttpHandlerState;

        // frequently used sequence
        static byte[] EndOfHeadersSequence = new byte[4] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };
        static byte[] Continue100Header = (new UTF8Encoding(false, true)).GetBytes("HTTP/1.1 100 Continue\r\n\r\n");

        // flags
        public bool IsFirstRequest { get; private set; }
        bool TlsHandlerAssigned;

        public ISocketHandlerTLSConfiguration TLSConfiguration { get; private set; }

        #endregion

        #region Properties

        // headers
        public IHttpRequestHeaders HttpRequestHeaders { get; private set; }

        // encoding
        public UTF8Encoding UTF8Encoding { get; private set; }

        // timer
        public double RequestStartTimestamp { get; private set; }

        // configuration
        [Inject]
        public IHttpSocketHandlerConfiguration HttpConfiguration { get; set; }

        [Inject]
        protected IWebResourceAssignmentHandler WebResourceAssignmentHandler { get; set; }

        #endregion

        #region Handling

        void HttpRequestSocketHandler_OnHandleInitialize(object sender, HandleSocketBEventArgs e)
        {
            if (!TlsHandlerAssigned)
            {
                this.OnHandleAsyncAction += HttpRequestSocketHandler_OnHandleAsyncAction;
                TlsHandlerAssigned = true;
            }

            // create new statefull utf8 encoder
            this.UTF8Encoding = new UTF8Encoding(false, true);

            if (IsFirstRequest)
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Http handler's 1st request"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                switch (TLSConfiguration.TLS_ActivationOptions)
                {
                    case SocketHandlerTLSActivationOptions.TLSAutoDetect:
                    case SocketHandlerTLSActivationOptions.TLSRequired:
                        HttpHandlerState = HttpRequestState.DeterminingIfClientRequestsTLS; break;

                    default:
                        HttpHandlerState = HttpRequestState.NewRequest; break;
                }

                // Set request start timestamp
                RequestStartTimestamp = Timer.Duration;

                // TCP keep-alives enabled?
                if (HttpConfiguration.EnableSocketKeepAlives)
                {
                    // set TCP timeout values
                    SetTcpKeepAlive(HttpConfiguration.SocketKeepAliveTimeoutMiliSeconds, HttpConfiguration.SocketKeepAliveProbeIntervalMiliSeconds);
                }
                else
                {
                    // todo
                    // SetTcpKeepAlive(0, HttpConfiguration.SocketKeepAliveTimeoutMiliSeconds);
                }

                // create statefull new encoding 
                this.UTF8Encoding = new UTF8Encoding(false, true);

                // set socket properties
                SetReceiveBufferSize(MemoryConfiguration.ReceiveBufferSize);
                SetSendBufferSize(MemoryConfiguration.SendBufferSize);
                SetNoDelay(true);
                SetBlocking(false);

                // set timeout
                SetCurrentStageTimeoutSeconds(HttpConfiguration.HttpReadingHeadersTimeoutSeconds, false);
            }
            else
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Http handler continuing listening for next request"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // set socket properties
                SetReceiveBufferSize(MemoryConfiguration.ReceiveBufferSize);
                SetSendBufferSize(MemoryConfiguration.SendBufferSize);

                // return request headers memory
                ReturnRequestHeadersMemory();

                // set state
                HttpHandlerState = HttpRequestState.NewRequest;

                // Set keep alife timeout if timeout is now set to readingheaders timeout
                SetCurrentStageTimeoutSeconds(HttpConfiguration.HttpKeepAliveTimeoutSeconds, true);
                SetActivityTimestamp(false);
            }

            // Start receiving
            if (e.RxBuffer.Length > 0)
            {
                InitNewRequestState();

                HandleReceive_ReadingHeaders(sender, e);
                return;
            }

            e.Count = Math.Max(Math.Max(1, NrOfBytesSocketHasAvailable), 5);
            e.NextAction = NextRequestedHandlerAction.Receive;
        }

        void HttpRequestSocketHandler_OnHandleReceive(object sender, HandleSocketBEventArgs e)
        {
            switch (HttpHandlerState)
            {
                case HttpRequestState.DeterminingIfClientRequestsTLS:
                    HandleReceive_DeterminingIfClientRequestsTLS(sender, e);
                    return;

                case HttpRequestState.NewRequest:
                    HandleReceive_NewRequest(sender, e);
                    return;

                case HttpRequestState.ReadingHeaders:
                    HandleReceive_ReadingHeaders(sender, e);
                    return;
            }
        }

        void HttpRequestSocketHandler_OnHandleSend(object sender, HandleSocketBEventArgs e)
        {
            if (e.TxBuffer.Count > 0)
            {
                e.NextAction = NextRequestedHandlerAction.Send;
                return;
            }

            // update state
            HttpHandlerState = HttpRequestState.GotRequest;

            // prevent timeout
            SetActivityTimestamp();

            try
            {
                DelegateControl(sender, e);
                return;
            }
            finally
            {
                IsFirstRequest = false;
            }
        }

        private void DelegateControl(object sender, HandleSocketBEventArgs e)
        {
            WebResourceAssignmentHandler.AssignWebResourceHandler(this, e, saeaHandler);
        }

        void HttpRequestSocketHandler_OnHandleReset(object sender, EventArgs e)
        {
            // return request headers memory
            ReturnRequestHeadersMemory();

            // reset flags
            IsFirstRequest = true;

            // set state
            HttpHandlerState = HttpRequestState.NewRequest;
        }

        async Task HttpRequestSocketHandler_OnHandleAsyncAction(object sender, HandleSocketBEventArgs e)
        {
            await _OnHandleStartTransportSecurity(e);
        }

        async Task _OnHandleStartTransportSecurity(HandleSocketBEventArgs e)
        {
            if (!SslStarted)
            {
                await StartSslAsServer(
                     Dns.GetHostName().ToLower(),
                     TLSConfiguration.TLS_EnabledProtocols,
                     System.Net.Security.EncryptionPolicy.RequireEncryption,
                     false,
                     new System.Net.Security.LocalCertificateSelectionCallback(OnLocalCertificateSelection),
                     false,
                     new System.Net.Security.RemoteCertificateValidationCallback(OnRemoteCertificateValidation),
                     GetServerCertificate(),
                     null
               );
            }

            e.Count = Math.Max(1, NrOfBytesSocketHasAvailable);
            e.NextAction = NextRequestedHandlerAction.Receive;
        }

        public event EventHandler<ProvideCertificateEventArgs> OnProvideCertificate;
        ProvideCertificateEventArgs _OnProvideCertificate_args;

        public X509Certificate2 GetServerCertificate()
        {
            switch (TLSConfiguration.TLS_CertificateUsage)
            {
                case SocketHandlerTLSCertificateUsage.AutoGenerate:
                    return LoadOrCreateServerCertificate(
                                 TLSConfiguration.UniqueConfigurationName,
                                 Dns.GetHostName().ToLower(),
                                 TLSConfiguration.TLS_AdditionalHostNames,
                                 new[] { KeyPurposeID.id_kp_serverAuth, KeyPurposeID.id_kp_clientAuth },
                                 TLSConfiguration.TLS_Default_CACertificate_AuthorityName,
                                 TLSConfiguration.TLS_ServerCertificate_PfxFilename,
                                 TLSConfiguration.TLS_CACertificate_PfxFilename,
                                 TLSConfiguration.TLS_ServerCertificate_Password,
                                 TLSConfiguration.TLS_CACertificate_Password,
                                 true
                             );

                case SocketHandlerTLSCertificateUsage.FromFileOnly:
                    return LoadOrCreateServerCertificate(
                                 TLSConfiguration.UniqueConfigurationName,
                                 Dns.GetHostName().ToLower(),
                                 TLSConfiguration.TLS_AdditionalHostNames,
                                 new[] { KeyPurposeID.id_kp_serverAuth, KeyPurposeID.id_kp_clientAuth },
                                 TLSConfiguration.TLS_Default_CACertificate_AuthorityName,
                                 TLSConfiguration.TLS_ServerCertificate_PfxFilename,
                                 TLSConfiguration.TLS_CACertificate_PfxFilename,
                                 TLSConfiguration.TLS_ServerCertificate_Password,
                                 TLSConfiguration.TLS_CACertificate_Password,
                                 false
                             );

                case SocketHandlerTLSCertificateUsage.ManualAssignmentByEvent:

                    var handler = OnProvideCertificate;

                    if (handler != null)
                    {
                        if (_OnProvideCertificate_args == null)
                        {
                            _OnProvideCertificate_args = new ProvideCertificateEventArgs();
                        }
                        _OnProvideCertificate_args.Result = null;

                        handler(this, _OnProvideCertificate_args);

                        return _OnProvideCertificate_args.Result;
                    }

                    throw new InvalidOperationException("TLS configuration is set to manual assignment for server certificate, but no OnProvideCertificate handler is assigned");
            }

            return null;
        }

        X509Certificate OnLocalCertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return GetServerCertificate();
        }


        bool OnRemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion

        #region Public

        public SuitableContentEncoding GetSuitableContentEncoding()
        {
            if (SslStarted)
                return SuitableContentEncoding.none;

            // find accepted encodings in http headers send by client
            String acceptEncoding = (String)HttpRequestHeaders["accept-encoding"];

            // found it?
            if (acceptEncoding != null && acceptEncoding.Length != 0)
            {
                // make lowercase
                acceptEncoding = acceptEncoding.ToLower();

                if (HttpConfiguration.EnableHttpDeflateCompressionSupport && (acceptEncoding.Contains("deflate") || acceptEncoding == "*"))
                {
                    return SuitableContentEncoding.deflate;
                }
                else
                    if (HttpConfiguration.EnableHttpGzipCompressionSupport && acceptEncoding.Contains("gzip"))
                {
                    return SuitableContentEncoding.gzip;
                }
            }

            return SuitableContentEncoding.none;
        }

        public Stream GetCompressionStream(SuitableContentEncoding encoding, Stream ResponseStream)
        {
            switch (encoding)
            {
                case SuitableContentEncoding.gzip:
                    return new Ionic.Zlib.GZipStream(ResponseStream, Ionic.Zlib.CompressionMode.Compress, HttpConfiguration.GzipCompressionLevel, true);

                case SuitableContentEncoding.deflate:
                    return new Ionic.Zlib.DeflateStream(ResponseStream, Ionic.Zlib.CompressionMode.Compress, HttpConfiguration.DeflateCompressionLevel, true);
            }

            return null;
        }

        #endregion

        #region Private

        #region Allocations

        void ReturnRequestHeadersMemory()
        {
            // de-reference
            ((HttpRequestHeaders)this.HttpRequestHeaders).Reset();
        }

        #endregion

        #region I/O State Handling

        #region Receiving

        #region State NewQuest

        void HandleReceive_DeterminingIfClientRequestsTLS(object sender, HandleSocketBEventArgs e)
        {
            if (e.RxBuffer.Count == 0)
            {
                e.Count = MemoryConfiguration.DynamicBufferFragmentSize;
                e.NextAction = NextRequestedHandlerAction.Receive;
                return;
            }

            if (e.RxBuffer[0] == 22)
            {
                e.NextAction = NextRequestedHandlerAction.AsyncAction;
                return;
            }

            HttpHandlerState = HttpRequestState.NewRequest;

            HttpRequestSocketHandler_OnHandleReceive(sender, e);
        }

        void HandleReceive_NewRequest(object sender, HandleSocketBEventArgs e)
        {
            InitNewRequestState();

            HttpRequestSocketHandler_OnHandleReceive(sender, e);
        }

        void InitNewRequestState()
        {
            // prevent timeout
            SetActivityTimestamp();

            // set Timestamp
            RequestStartTimestamp = Timer.Duration;

            // set readingheaders timeout if now set to keep-alife timeout
            Interlocked.CompareExchange(
                ref CurrentStageTimeout,
                BitConverter.DoubleToInt64Bits(HttpConfiguration.HttpReadingHeadersTimeoutSeconds),
                BitConverter.DoubleToInt64Bits(HttpConfiguration.HttpKeepAliveTimeoutSeconds)
            );

            // update state and recurse once
            HttpHandlerState = HttpRequestState.ReadingHeaders;
        }

        #endregion

        #region State ReadingHeaders

        void HandleReceive_ReadingHeaders(object sender, HandleSocketBEventArgs e)
        {
            // find end of headers
            int? count = null;
            int HeadersEndOffset = e.RxBuffer.IndexOf(ref lastEOHsearchIndex, ref count, SslStarted, EndOfHeadersSequence);

            // got the end of the headers?
            if (HeadersEndOffset >= 0)
            {
                // reset
                lastEOHsearchIndex = 0;

                // create headers
                ((HttpRequestHeaders)HttpRequestHeaders).Init(
                    SslStarted,
                    PortNumber, PortNumber,
                    e.RxBuffer,
                    HeadersEndOffset + EndOfHeadersSequence.Length
                 );

                // prevent timeout
                SetActivityTimestamp();

                // check for 100-continue
                string expect = HttpRequestHeaders["expect"];

                // expects 100-continue?
                if ((!String.IsNullOrEmpty(expect)) && (expect.ToLower() == "100-continue"))
                {
                    // write continue
                    Buffer.BlockCopy(Continue100Header, 0, saeaHandler.Buffer, 0, Continue100Header.Length);

                    // update state
                    HttpHandlerState = HttpRequestState.Sending100Continue;

                    // send
                    e.NextAction = NextRequestedHandlerAction.Send;
                    return;
                }

                // update state
                HttpHandlerState = HttpRequestState.GotRequest;

                try
                {
                    // assign new handler to handle this request

                    DelegateControl(sender, e);
                    return;
                }
                finally
                {
                    IsFirstRequest = false;
                }
            }
            else
            {
                // Too large request header?
                if (e.RxBuffer.Count >= HttpConfiguration.MaxIncomingRequestHeaderSize)
                {
                    SendHttpErrorMessage(e, 413, "Request Entity Too Large", true);
                    return;
                }
            }

            e.Count = count.HasValue ? Math.Max(Math.Max(1, NrOfBytesSocketHasAvailable), count.Value) : MemoryConfiguration.ReceiveBufferSize;
            e.NextAction = NextRequestedHandlerAction.Receive;
            return;
        }

        #endregion

        #endregion

        #endregion

        #region Misc

        void SendHttpErrorMessage(HandleSocketBEventArgs e, int HttpResultCode, string HttpStatusMessage, bool ForceDisconnect = true)
        {
            // create new socket handler
            var newHandler = Kernel.Get<EmptyHttpResponseHandler>();

            // set handler properties
            newHandler.HttpRequestSocketHandler = this;
            newHandler.HttpResultCode = HttpResultCode;
            newHandler.HttpStatusMessage = HttpStatusMessage;
            newHandler.ForceDisconnect = ForceDisconnect;

            // delegate
            newHandler.TakeOverConrolOfSocket(saeaHandler);
            e.NextAction = NextRequestedHandlerAction.Handled;
        }

        #endregion

        #endregion
    }
}
