#region Using

using System.Text;
using System.Net.Sockets;
using Ninject;
using GenXdev.AsyncSockets.Additional;
using GenXdev.Configuration;
using System.Collections.Concurrent;
using GenXdev.MemoryManagement;
using GenXdev.AsyncSockets.Containers;
using GenXdev.Additional;
using System.Net.Security;
using GenXdev.Buffers;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography.X509Certificates;
using GenXdev.Helpers;
using System.Reflection;
using GenXdev.AsyncSockets.Arguments;
using System.Security.Authentication;
using System.Net;

#endregion

namespace GenXdev.AsyncSockets.Handlers
{
    public delegate void LockCallback();

    /// <summary>
    /// Baseclass for asynchronous socket communication controllers
    /// </summary>
    public class SocketHandlerBase : IDisposable
    {
        #region Initialization / Finalization

        /// <summary>
        /// Reference to Ninject service kernel
        /// </summary>
        protected IKernel Kernel { get; private set; }

        /// <summary>
        /// Reference to global memory manager
        /// </summary>
        protected IServiceMemoryManager MemoryManager { get; private set; }

        /// <summary>
        /// Assigned memory configuration for this socket handler 
        /// </summary>
        public IMemoryManagerConfiguration MemoryConfiguration { get; private set; }

        /// <summary>
        /// Servicename category of this socket handler 
        /// </summary>
        public String ServiceName { get; private set; }
        internal ISocketIOHandler SocketIO;

#if(Logging)
        /// <summary>
        /// Servicelogger for debug logging
        /// </summary>
        protected internal IServiceLogger Logger { get; private set; }
#endif

        /// <summary>
        /// The socket handler pool, if any, associated with this socket handler
        /// </summary>
        protected object Pool { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors"), Inject]
        public SocketHandlerBase(
              IKernel Kernel
            , IMemoryManagerConfiguration MemoryConfiguration
            , IServiceMemoryManager MemoryManager
            , String ServiceName
#if (Logging)
, IServiceLogger Logger
#endif
, object Pool = null
)
        {
            this.Kernel = Kernel;
            this.MemoryConfiguration = MemoryConfiguration;
            this.MemoryManager = MemoryManager;
            this.ServiceName = ServiceName;
            this.Pool = Pool;

            LastActivityTimeStamp = BitConverter.DoubleToInt64Bits(Timer.Duration);

#if (Logging)
            this.Logger = Logger;
#endif

            SetActivityTimestamp();
            PollTimer = new System.Threading.Timer(new TimerCallback(ClearPollIngStateAndContinue), this, Timeout.Infinite, Timeout.Infinite);
            this.rxBuffer = Kernel.Get<DynamicBuffer>();
            this.txBuffer = Kernel.Get<DynamicBuffer>();
            this.SocketIO = new SocketIOHandler();

            SetupHandlers();
        }

        /// <summary>
        /// Descendents classes should implement this method to assign the initial eventhandlers for handling this socket
        /// </summary>
        protected virtual void SetupHandlers() { }

        /// <summary>
        /// Used internally to create a new socket event argument instance
        /// </summary>
        internal protected void CreateSaea()
        {
            DisposeSaea();

            //            var saeaBufferSize = Math.Max(MemoryConfiguration.ReceiveBufferSize, MemoryConfiguration.SendBufferSize);
            saeaHandler = new SocketAsyncEventArgs();
            saeaHandler.SetHandler(this);
            //            saeaHandler.SetBuffer(0, saeaBufferSize);
        }

        internal protected void DisposeSaea()
        {
            if (saeaHandler != null)
            {
                SocketIO.Unregister(saeaHandler);
                saeaHandler.SetHandler(null);
                saeaHandler.Dispose();
                saeaHandler = null;
            }
        }


        /// <summary>
        /// Disposes this socket handler instance 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                IsDisposed = true;

                if (saeaHandler != null)
                {
                    DisposeSaea();
                }

                this.SocketIO = null;
                this.OnHandlerClosed = null;

                StopSsl();

                if (PollTimer != null)
                {
                    PollTimer.Dispose();
                    PollTimer = null;
                }

                if (SslAdapter != null)
                {
                    SslAdapter.Dispose();
                    SslAdapter = null;
                }

                if (rxBuffer != null)
                {
                    rxBuffer.Dispose();
                    rxBuffer = null;
                }

                if (txBuffer != null)
                {
                    txBuffer.Dispose();
                    txBuffer = null;
                }

                OnHandleCapturedSocketDisconnectSuccess = null;
                OnHandleCapturedSocketInitialize = null;
                OnHandleCapturedSocketReceiveSuccess = null;
                OnHandleCapturedSocketReset = null;
                OnHandleCapturedSocketSendSuccess = null;
                OnHandleCaptureRelease = null;
                OnHandleConnect = null;
                OnHandleDisconnect = null;
                OnHandleException = null;
                OnHandleInitialize = null;
                OnHandleNewMessage = null;
                OnHandlePoll = null;
                OnHandleAsyncAction = null;
                OnHandleReceive = null;
                OnHandleReset = null;
                OnHandleSend = null;
            }
        }

        ~SocketHandlerBase()
        {
            Dispose(false);
        }

        #endregion

        #region Fields

        public static GenXdev.Additional.HiPerfTimer.NativeMethods Timer = new GenXdev.Additional.HiPerfTimer.NativeMethods(true);

        #region Private

        internal static long _NextUniqueIdx = 0;
        private object _UniqueSocketIdLock = new object();
        internal protected long UniqueSocketId
        {
            get
            {
                lock (_UniqueSocketIdLock)
                {

                    return _UniqueSocketId;
                }
            }

            internal set
            {
                lock (_UniqueSocketIdLock)
                {
                    _UniqueSocketId = value;
                }
            }
        }

        bool IsDisposed;
        SslStream SslConversionStream;
        AsyncSocketHandlerTlsStreamAdapter SslAdapter;
        public DynamicBuffer rxBuffer { get; private set; }
        public DynamicBuffer txBuffer { get; private set; }

        internal Queue<SocketHandlerBase> CaptureRequestQueue = new Queue<SocketHandlerBase>(4);
        internal Queue<object> MessageQueue = new Queue<object>(4);
        internal volatile bool IsCaptured;
        internal volatile SocketHandlerBase _CapturingHandler;

        internal volatile bool IsInAsyncCall;
        public event EventHandler<HandlerClosedEventArgs> OnHandlerClosed;
        volatile public SocketAsyncEventArgs saeaHandler;
        static byte[] dummyPacket = new byte[1];
        internal protected long CurrentStageTimeout = BitConverter.DoubleToInt64Bits(20d);
        internal protected long LastActivityTimeStamp;

        long _HandlerState = 3;

        protected Double HandlerStartTimestamp { get; private set; }
        protected internal volatile bool AutoResetTimeouts;
        object PollingStateLock = new object();
        System.Threading.Timer PollTimer;
        long _PollingState = 0;
        ConcurrentDictionary<string, List<SocketHandlerBase>> SignalWaiteners =
             new ConcurrentDictionary<string, List<SocketHandlerBase>>();
        ConcurrentDictionary<string, List<SocketHandlerBase>> SignalSuppliers =
             new ConcurrentDictionary<string, List<SocketHandlerBase>>();

        #endregion

        #endregion

        # region Transport security

        /// <summary>
        /// Starts TLS with a auto generated, and then cached, self-signed certificate
        /// </summary>
        public async Task StartSslAsServer()
        {
            if (SslStarted)
                return;

            // init
            string issuerName = "Self-signed";

            // determine what assembly to use for companyname
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                assembly = Assembly.GetAssembly(this.GetType());

            // get companyname
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length > 0)
            {
                var attribute = attributes[0] as AssemblyCompanyAttribute;
                issuerName = attribute.Company;
            }

            // delegate
            await StartSslAsServer(ServiceName, issuerName);
        }

        /// <summary>
        /// Starts TLS with a auto generated, and then cached, self-signed certificate
        /// </summary>
        /// <param name="targetHost">The name of the self-signed authority</param>
        /// <param name="issuerName">The subject for this certificate, e.g. domainname</param>
        public async Task StartSslAsServer(string targetHost, string issuerName)
        {
            if (SslStarted)
                return;

            // delegate
            await _StartSslAsServer(targetHost, issuerName);
        }

        /// <summary>
        /// Starts TLS with a auto generated, and then cached, self-signed certificate
        /// </summary>
        /// <param name="targetHost">The name of the self-signed authority</param>
        /// <param name="issuerName">The subject for this certificate, e.g. domainname</param>
        async Task _StartSslAsServer(string targetHost, string issuerName, SslProtocols enabledSslProtocols = SslProtocols.Tls12)
        {
            if (SslStarted)
                return;

            // prevent timeout
            SetCurrentStageTimeoutSeconds(50, true);
            SetActivityTimestamp();

            // require encryption
            var EncryptionPolicy = System.Net.Security.EncryptionPolicy.RequireEncryption;

            // init flags
            bool clientCertificateRequired = false;
            bool checkCertificateRevocation = false;

            // load or create server certificate
            serverCertificate = LoadOrCreateServerCertificate(ServiceName, targetHost,
                new[] { Dns.GetHostName().ToLower(), GenXdev.Helpers.Network.GetPublicExternalHostname(((IPEndPoint)saeaHandler.AcceptSocket.LocalEndPoint).ToString()) },
                new[] { KeyPurposeID.id_kp_serverAuth, KeyPurposeID.id_kp_emailProtection, KeyPurposeID.id_kp_clientAuth },
                issuerName
            );

            // delegate
            await StartSslAsServer(targetHost, enabledSslProtocols, EncryptionPolicy, clientCertificateRequired, OnSelectLocalCertificate,
                checkCertificateRevocation, OnValidateRemoteCertificate, serverCertificate, null);
        }

        /// <summary>
        /// Starts TLS with a auto generated, and then cached, self-signed certificate
        /// </summary>
        /// <param name="PFXFilePath">Filepath to a PFX certificate file</param>
        /// <param name="Password">The password for this certificate</param>
        public async Task StartSslAsServerWithCertificateFromFile(string PFXFilePath, string Password = null)
        {
            if (SslStarted)
                return;

            // prevent timeout
            SetCurrentStageTimeoutSeconds(50, true);
            SetActivityTimestamp();

            lock (sslCertificates)

                // try to find certificate from the local memory cache
                if (!sslCertificates.TryGetValue(PFXFilePath, out serverCertificate))
                {
                    // not found!

                    // setup file path
                    if (!Path.IsPathRooted(PFXFilePath))
                    {
                        PFXFilePath = Path.Combine(GenXdev.Helpers.Environment.GetAssemblyRootDirectory(), PFXFilePath);
                    }

                    // does not exist?
                    if (!File.Exists(PFXFilePath))
                    {
                        throw new InvalidOperationException("Could not find certificate file \"" + PFXFilePath + "\"");
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () => { return "Loading server certificate"; });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    serverCertificate = new X509Certificate2(
                        File.ReadAllBytes(PFXFilePath),
                        Password,
                        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
                    );

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () => { return "Server certificate loaded"; });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    // add to local memory cache
                    sslCertificates.AddOrUpdate(PFXFilePath, serverCertificate, (key, oldValue) => serverCertificate);
                }
                else
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () => { return "Server certificate loaded from cache"; });
#endif
                    #endregion ------------------------------------------------------------------------------------------------
                }

            // failed?
            if (serverCertificate == null)
            {
                throw new InvalidOperationException("Could not find any certificate in file \"" + PFXFilePath + "\"");
            }

            // by default only the latest version
            var enabledSslProtocols = SslProtocols.Tls12;

            // require encryption
            var EncryptionPolicy = System.Net.Security.EncryptionPolicy.RequireEncryption;

            // init flags
            bool clientCertificateRequired = false;
            bool checkCertificateRevocation = false;

            // delegate
            await StartSslAsServer(serverCertificate.Subject, enabledSslProtocols, EncryptionPolicy, clientCertificateRequired, OnSelectLocalCertificate,
                checkCertificateRevocation, OnValidateRemoteCertificate, serverCertificate, null);
        }

        /// <summary>
        /// Called by servers to begin an asynchronous operation to authenticate the server and optionally the client using the specified certificates, requirements and security protocol.
        /// </summary>
        /// <param name="targetHost"></param>
        /// <param name="enabledSslProtocols">The System.Security.Authentication.SslProtocols value that represents the protocol used for authentication</param>
        /// <param name="EncryptionPolicy">The EncryptionPolicy to use</param>
        /// <param name="clientCertificateRequired">A System.Boolean value that specifies whether the client must supply a certificate for authentication.</param>
        /// <param name="OnSelectLocalCertificate">Selects the local Secure Sockets Layer (SSL) certificate used for authentication.</param>
        /// <param name="checkCertificateRevocation">Indicates if the remote certificate needs to be checked against a official certificate authority</param>
        /// <param name="OnValidateRemoteCertificate">Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.</param>
        /// <param name="serverCertificate">The X509Certificate used to authenticate the server.</param>
        /// <param name="certificatePassword">The password for the private key of the provided server certificate</param>
        /// <returns>An System.IAsyncResult object that indicates the status of the asynchronous operation.</returns>
        public async Task StartSslAsServer(

            string targetHost,
            System.Security.Authentication.SslProtocols enabledSslProtocols,
            EncryptionPolicy EncryptionPolicy,
            bool clientCertificateRequired,
            LocalCertificateSelectionCallback OnSelectLocalCertificate,
            bool checkCertificateRevocation,
            RemoteCertificateValidationCallback OnValidateRemoteCertificate,
            System.Security.Cryptography.X509Certificates.X509Certificate2 serverCertificate,
            string certificatePassword = null
        )
        {
            if (SslStarted)
                return;

            await _StartSslAsServer(enabledSslProtocols, EncryptionPolicy, clientCertificateRequired, OnSelectLocalCertificate, checkCertificateRevocation,
                OnValidateRemoteCertificate, serverCertificate);
        }

        /// <summary>
        /// Called by servers to begin an asynchronous operation to authenticate the server and optionally the client using the specified certificates, requirements and security protocol.
        /// </summary>
        /// <param name="targetHost"></param>
        /// <param name="enabledSslProtocols">The System.Security.Authentication.SslProtocols value that represents the protocol used for authentication</param>
        /// <param name="EncryptionPolicy">The EncryptionPolicy to use</param>
        /// <param name="clientCertificateRequired">A System.Boolean value that specifies whether the client must supply a certificate for authentication.</param>
        /// <param name="OnSelectLocalCertificate">Selects the local Secure Sockets Layer (SSL) certificate used for authentication.</param>
        /// <param name="checkCertificateRevocation">Indicates if the remote certificate needs to be checked against a official certificate authority</param>
        /// <param name="OnValidateRemoteCertificate">Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.</param>
        /// <param name="serverCertificate">The X509Certificate used to authenticate the server.</param>
        /// <param name="certificatePassword">The password for the private key of the provided server certificate</param>
        /// <returns>An System.IAsyncResult object that indicates the status of the asynchronous operation.</returns>
        async Task _StartSslAsServer(System.Security.Authentication.SslProtocols enabledSslProtocols, EncryptionPolicy EncryptionPolicy, bool clientCertificateRequired, LocalCertificateSelectionCallback OnSelectLocalCertificate, bool checkCertificateRevocation, RemoteCertificateValidationCallback OnValidateRemoteCertificate, System.Security.Cryptography.X509Certificates.X509Certificate2 serverCertificate)
        {
            if (SslStarted)
                return;

            // set flag
            SslStarted = true;

            // prevent timeout
            SetCurrentStageTimeoutSeconds(50, true);
            SetActivityTimestamp();

            if (SslAdapter == null)
            {
                // create asynchronous SslStream adapter
                SslAdapter = new AsyncSocketHandlerTlsStreamAdapter(Kernel, this);
            }

            SslAdapter.Initialize();

            // create SslStream
            SslConversionStream = new SslStream(SslAdapter, true, OnValidateRemoteCertificate, OnSelectLocalCertificate, EncryptionPolicy);

            // delegate
            await SslConversionStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation,
                null, null);

            // fire user event
            HandleTransportSecurityInitiatationCompleted();
        }

        /// <summary>
        /// Continue without using transport security
        /// </summary>
        /// <returns></returns>
        public async Task StartWithoutSsl()
        {
            await Task.Factory.StartNew(() => { });
        }

        /// <summary>
        /// Starts TLS transport security as client
        /// </summary>
        /// <param name="targetHost"></param>
        /// <returns></returns>
        public async Task StartSslAsClient(string targetHost = "Self-signed")
        {
            if (SslStarted)
                return;

            // set flag
            SslStarted = true;

            // prevent timeout
            SetCurrentStageTimeoutSeconds(50, true);
            SetActivityTimestamp();

            // create asynchronous SslStream adapter
            SslAdapter = new AsyncSocketHandlerTlsStreamAdapter(Kernel, this);

            // create SslStream
            SslConversionStream = new SslStream(
                SslAdapter,
                true,
                OnValidateRemoteCertificate
             );

            // start authenticating
            await SslConversionStream.AuthenticateAsClientAsync(targetHost);

            // fire user event
            HandleTransportSecurityInitiatationCompleted();
        }

        public async Task StartSslAsClient(
            string targetHost,
            System.Security.Authentication.SslProtocols enabledSslProtocols,
            EncryptionPolicy EncryptionPolicy,
            X509CertificateCollection clientCertificates,
            LocalCertificateSelectionCallback OnSelectLocalCertificate,
            bool checkCertificateRevocation,
            RemoteCertificateValidationCallback OnValidateRemoteCertificate
        )
        {
            if (SslStarted)
                return;

            SslStarted = true;

            SetCurrentStageTimeoutSeconds(50, true);
            SetActivityTimestamp();

            if (SslAdapter == null)
            {
                SslAdapter = new AsyncSocketHandlerTlsStreamAdapter(Kernel, this);
            }

            SslAdapter.Initialize();

            SslConversionStream = new SslStream(SslAdapter, true, OnValidateRemoteCertificate, OnSelectLocalCertificate, EncryptionPolicy);

            await SslConversionStream.AuthenticateAsClientAsync(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, null, null);

            HandleTransportSecurityInitiatationCompleted();
        }

        public void StopSsl()
        {
            if (!SslStarted)
                return;

            SslStarted = false;

            if (SslConversionStream != null)
            {
                SslConversionStream.Dispose();
                SslConversionStream = null;
            }

            if (SslAdapter == null)
                return;

            SslAdapter.Finalize(rxBuffer);
        }

        int receiveCount;

        async Task PerformReceiveSSL(SocketAsyncEventArgs saea)
        {
            var count = receiveCount;

            var buffer = MemoryManager.TakeBuffer(count);
            try
            {
                int read = 0;

                read += await SslConversionStream.ReadAsync(buffer, 0, count);

                rxBuffer.Add(buffer, 0, read);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () => { return count.ToString() + " requested bytes were decrypted and placed into receivebuffer (" + rxBuffer.Count.ToString() + " bytes)"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                if (IsCaptured)
                {
                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync,
                        TranslateHandlerAction(HandleCapturedSocketReceiveSuccess(saea, rxBuffer, txBuffer))
                    );
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync,
                        TranslateHandlerAction(HandleReceive())
                    );
                }
            }
            finally
            {
                MemoryManager.ReturnBuffer(buffer);
            }
        }

        async Task PerformSendSSL(SocketAsyncEventArgs saea)
        {
            var buffer = MemoryManager.TakeBuffer(MemoryConfiguration.DynamicBufferFragmentSize);
            try
            {
                while (txBuffer.Count > 0)
                {
                    // remove
                    var read = txBuffer.Remove(buffer, 0, buffer.Length);

                    SocketIO.SetReceiveBufferSize(saea, MemoryConfiguration.ReceiveBufferSize);

                    // send async
                    await SslConversionStream.WriteAsync(buffer, 0, read);

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () => { return read.ToString() + " bytes from sendbuffer were encrypted and sent"; });
#endif
                    #endregion ------------------------------------------------------------------------------------------------
                }
                if (IsCaptured)
                {
                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync,
                        TranslateHandlerAction(HandleCapturedSocketSendSuccess(saea, rxBuffer, txBuffer))
                    );
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(HandleSend()));
                }
            }
            finally
            {
                MemoryManager.ReturnBuffer(buffer);
            }
        }

        System.Security.Cryptography.X509Certificates.X509Certificate2 OnSelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, System.Security.Cryptography.X509Certificates.X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return null;
        }

        bool OnValidateRemoteCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () => { return "OnValidateRemoteCertificate, returning true"; });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            return true;
        }

        System.Security.Cryptography.X509Certificates.X509Certificate2 serverCertificate;
        static ConcurrentDictionary<string, System.Security.Cryptography.X509Certificates.X509Certificate2> sslCertificates = new ConcurrentDictionary<string, System.Security.Cryptography.X509Certificates.X509Certificate2>();

        public static void RemoveCertificateFromCache(string id)
        {
            sslCertificates.TryRemove(id, out X509Certificate2 cert);
        }

        public static void ClearCertificateMemoryCache(string id)
        {
            sslCertificates.Clear();
        }

        public static System.Security.Cryptography.X509Certificates.X509Certificate2 LoadOrCreateServerCertificate(
            string uniqueName,
            string certSubjectName,
            string[] certSubjectAltNames,
            KeyPurposeID[] usages,
            string issuerName,
            string pfxFilePath = null,
            string certAuthorityCertificatePfxFilePath = null,
            string pfxFilePassword = null,
            string certAuthorityCertificatePfxPassword = null,
            bool autoGenerate = true,
            bool dontGenerateCACertificate = false
        )
        {
            lock (sslCertificates)
            {
                StringBuilder subjectNames = new StringBuilder();
                subjectNames.Append(certSubjectName + "_");
                if (certSubjectAltNames != null)
                {
                    foreach (var s in certSubjectAltNames)
                        subjectNames.Append(s + "_");
                }
                var subjectNamesHash = "h" + GenXdev.Security.MD5.CalculateMD5Hash(subjectNames.ToString()).Substring(0, 9);

                bool forceRegeneration = false;
                X509Certificate2 issuerCertificate =
                    LoadOrCreateCertAuthorityCertificate(issuerName, certAuthorityCertificatePfxFilePath, certAuthorityCertificatePfxPassword, autoGenerate && !dontGenerateCACertificate);

                var id = new StringBuilder();
                id.Append(uniqueName);

                if (issuerCertificate != null)
                {
                    id.Append("_" + issuerCertificate.GetCertHashString());
                }

                bool found = sslCertificates.TryGetValue(id.ToString(), out X509Certificate2 serverCertificate);

                if (found)
                {
                    forceRegeneration =
                        autoGenerate && (
                        (System.DateTime.UtcNow > serverCertificate.NotAfter)
                        ||
                        !serverCertificate.SubjectName.Name.Replace("\"", "").Contains("L=" + subjectNamesHash)
                        );
                }

                if (!found || forceRegeneration)
                {
                    found = !forceRegeneration && TryLoadCertificateFromFile(id.ToString(), pfxFilePath, pfxFilePassword, out serverCertificate);

                    if (found)
                    {
                        forceRegeneration =
                            autoGenerate && (
                            (System.DateTime.UtcNow > serverCertificate.NotAfter)
                            ||
                            !serverCertificate.SubjectName.Name.Replace("\"", "").Contains("L=" + subjectNamesHash)
                            );

                        try
                        {
                            GenXdev.Helpers.Pki.WriteCertificateToPemFormat(serverCertificate, Path.ChangeExtension(pfxFilePath, ".public.cer"));
                        }
                        catch { }

                        if (!forceRegeneration)
                        {
                            return serverCertificate;
                        }
                    }

                    if (!found || forceRegeneration)
                    {
                        // re-order and optimize
                        GenXdev.Helpers.Pki.OptimizeAndReorganizeDNSNames(ref certSubjectName, ref certSubjectAltNames);

                        serverCertificate = GenXdev.Helpers.Pki.IssueCertificate(
                            "CN=" + certSubjectName + ", L=" + subjectNamesHash,
                            issuerCertificate,
                            certSubjectAltNames,
                            usages
                        );

                        sslCertificates.AddOrUpdate(id.ToString(), serverCertificate, (key, oldValue) => serverCertificate);

                        if (!String.IsNullOrWhiteSpace(pfxFilePath))
                            try
                            {
                                GenXdev.Helpers.Pki.WriteCertificate(serverCertificate, pfxFilePath);
                                GenXdev.Helpers.Pki.WriteCertificateToPemFormat(serverCertificate, Path.ChangeExtension(pfxFilePath, ".public.cer"));
                            }
                            catch { }
                    }

                    return serverCertificate;
                }


                return serverCertificate;
            }
        }

        public static bool TryLoadCertificateFromFile(
            string id,
            string pfxFilePath,
            string pfxPassword,
            out System.Security.Cryptography.X509Certificates.X509Certificate2 serverCertificate
        )
        {
            serverCertificate = null;

            if (!String.IsNullOrWhiteSpace(pfxFilePath) && File.Exists(pfxFilePath))
            {
                try
                {
                    serverCertificate = GenXdev.Helpers.Pki.LoadCertificate(pfxFilePath, pfxPassword);

                    if (System.DateTime.UtcNow <= serverCertificate.NotAfter)
                    {
                        var sc = serverCertificate;

                        sslCertificates.AddOrUpdate(id, serverCertificate, (key, oldValue) => sc);

                        return true;
                    }
                }
                catch (Exception e)
                {
                    serverCertificate = null;
                    GenXdev.Helpers.FileSystem.ForciblyMoveFile(pfxFilePath, pfxFilePath + ".rejected.could.not.open.pfx", false);
                    try
                    {
                        var txtPath = pfxFilePath + ".rejected.could.not.open.txt";
                        GenXdev.Helpers.FileSystem.ForciblyPrepareTargetFilePath(txtPath);
                        File.WriteAllText(txtPath, e.Message, new UTF8Encoding(false));
                    }
                    catch { }
                }
            }

            return false;
        }

        public static X509Certificate2 LoadOrCreateCertAuthorityCertificate(
            string certSubjectName,
            string certAuthorityCertificatePfxFilePath,
            string certAuthorityCertificatePfxPassword,
            bool autoGenerate = true
        )
        {
            X509Certificate2 certAuthorityCertificate = null;

            lock (sslCertificates)
            {
                if (!sslCertificates.TryGetValue(certSubjectName, out certAuthorityCertificate))
                {
                    bool forceRegeneration = false;
                    var found = TryLoadCertificateFromFile(certSubjectName, certAuthorityCertificatePfxFilePath, certAuthorityCertificatePfxPassword, out certAuthorityCertificate);

                    if (found)
                    {
                        forceRegeneration =
                            autoGenerate && (
                            (System.DateTime.UtcNow > certAuthorityCertificate.NotAfter)
                            ||
                            (certAuthorityCertificate.SubjectName.Name.Replace("\"", "") != "CN=" + certSubjectName.Trim() + " CA")
                            );
                    }

                    if (!found || forceRegeneration)
                    {
                        certAuthorityCertificate = GenXdev.Helpers.Pki.CreateCertificateAuthorityCertificate("CN=" + certSubjectName.Trim() + " CA", null, null);

                        sslCertificates.AddOrUpdate(certSubjectName, certAuthorityCertificate, (key, oldValue) => certAuthorityCertificate);

                        if (!String.IsNullOrWhiteSpace(certAuthorityCertificatePfxFilePath))
                            try
                            {
                                GenXdev.Helpers.Pki.WriteCertificate(certAuthorityCertificate, certAuthorityCertificatePfxFilePath, certAuthorityCertificatePfxPassword);
                                GenXdev.Helpers.Pki.WriteCertificateToPemFormat(certAuthorityCertificate, Path.ChangeExtension(certAuthorityCertificatePfxFilePath, ".cer"));
                            }
                            catch { }
                    }
                    else
                    {
                        try
                        {
                            GenXdev.Helpers.Pki.WriteCertificateToPemFormat(certAuthorityCertificate, Path.ChangeExtension(certAuthorityCertificatePfxFilePath, ".public.cer"));
                        }
                        catch { }
                    }

                    try
                    {
                        // break-point here? you should definitely debug with administrator privileges!

                        X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

                        store.Open(OpenFlags.ReadWrite);

                        var deleteList = new List<X509Certificate2>();

                        foreach (var cert in store.Certificates)
                        {
                            if (cert.SubjectName.Name == certAuthorityCertificate.SubjectName.Name)
                            {
                                deleteList.Add(cert);
                            }
                        }

                        foreach (var cert in deleteList)
                        {
                            store.Remove(cert);
                        }

                        X509Certificate2Collection collection = new X509Certificate2Collection();
                        byte[] encodedCert = certAuthorityCertificate.GetRawCertData();
                        store.Add(certAuthorityCertificate);
                        store.Close();
                    }
                    catch { }
                }
            }

            return certAuthorityCertificate;
        }

        static SocketHandlerBase()
        {
            GenXdev.Helpers.Pki.EnableTlsProtocols();
        }

        #endregion

        #region Properties

        public byte[] SslLocalCertificateHash
        {
            get
            {
                if (SslStarted && (SslConversionStream != null) && (SslConversionStream.LocalCertificate != null))
                {
                    //System.Diagnostics.Debug.WriteLine(this.GetType().Name + " -> LOCAL -> " +
                    //    GenXdev.Helpers.Hash.FormatBytesAsBase64(GenXdev.Helpers.Hash.GetSha256Bytes(SslConversionStream.LocalCertificate.GetPublicKey())));

                    return GenXdev.Helpers.Hash.GetSha256Bytes(SslConversionStream.LocalCertificate.GetPublicKey());
                }

                return new byte[0];
            }
        }

        public byte[] SslRemoteCertificateHash
        {
            get
            {
                if (SslStarted && (SslConversionStream != null) && (SslConversionStream.RemoteCertificate != null))
                {
                    //System.Diagnostics.Debug.WriteLine(this.GetType().Name + " -> REMOTE -> " +
                    //    GenXdev.Helpers.Hash.FormatBytesAsBase64(GenXdev.Helpers.Hash.GetSha256Bytes(SslConversionStream.RemoteCertificate.GetPublicKey())));

                    return GenXdev.Helpers.Hash.GetSha256Bytes(SslConversionStream.RemoteCertificate.GetPublicKey());
                }

                return new byte[0];
            }
        }

        public bool SslStarted { get; private set; }

        public bool Idle
        {
            get
            {
                return (Interlocked.Read(ref _HandlerState) == 0);
            }
            //internal protected set
            //{
            //    // set state to active
            //    Interlocked.Exchange(ref _HandlerState, 1);
            //}
        }
        public bool Active
        {
            get
            {
                return (Interlocked.Read(ref _HandlerState) == 1);
            }
        }
        public bool Closing
        {
            get
            {
                return (Interlocked.Read(ref _HandlerState) >= 2);
            }
        }
        public bool Closed
        {
            get
            {
                return (Interlocked.Read(ref _HandlerState) == 3);
            }
        }

        // port
        public int PortNumber { get; private set; }

        SocketHandlerBase CapturingHandler
        {
            get
            {
                lock (CaptureRequestQueue)
                {
                    return _CapturingHandler;
                }
            }
        }

        internal SocketAsyncEventArgs CapturingSaea
        {
            get
            {
                lock (CaptureRequestQueue)
                {
                    return _CapturingHandler.saeaHandler;
                }
            }
        }

        volatile SocketHandlerBase __ParentHandler;
        volatile SocketHandlerBase __ChildHandler;
        internal SocketHandlerBase ParentHandler { get { return __ParentHandler; } private set { __ParentHandler = value; } }
        internal SocketHandlerBase ChildHandler { get { return __ChildHandler; } private set { __ChildHandler = value; } }
        public SocketHandlerBase ControllingHandler
        {
            get
            {
                SocketHandlerBase controller = this;
                do
                {
                    var child = controller.ChildHandler;

                    if (child != null)
                        controller = child;

                } while (controller.ChildHandler != null);

                return controller;
            }
        }


        /// <summary>
        /// Indicates whether there is more data ready to be received 
        /// </summary>
        public bool SocketHasDataAvailable
        {
            get
            {
                return
                       (SslAdapter != null && SslAdapter.DataAvailable) ||
                       (SocketIO != null && SocketIO.SocketHasDataAvailable(saeaHandler));
            }
        }

        /// <summary>
        /// Indicates the minimum amount of bytes that can be received instantaniously
        /// </summary>
        public int NrOfBytesSocketHasAvailable
        {
            get
            {
                if (SslAdapter != null)
                {
                    return SslAdapter.NrOfBytesSocketHasAvailable;
                }

                if (SocketIO != null)
                {
                    return SocketIO.NrOfBytesSocketHasAvailable(saeaHandler);
                };

                return 0;
            }
        }

        /// <summary>
        /// Indicates whether there is more data in the receivebuffer or ready to be received 
        /// </summary>
        public bool DataAvailable
        {
            get
            {
                lock (rxBuffer.SyncRoot)
                {
                    return (rxBuffer != null && rxBuffer.Count > 0) ||
                           (SslAdapter != null && SslAdapter.DataAvailable) ||
                           (SocketIO != null && SocketIO.SocketHasDataAvailable(saeaHandler));
                }
            }
        }

        public bool IsTimedOut
        {
            get
            {
#if (NoSocketTimeouts)
                return false;
#else
                try
                {
                    return (!Closing) && (BitConverter.Int64BitsToDouble(Interlocked.Read(ref CurrentStageTimeout)) > 0) &&
                        (Timer.Duration - BitConverter.Int64BitsToDouble(Interlocked.Read(ref LastActivityTimeStamp)) > BitConverter.Int64BitsToDouble(Interlocked.Read(ref CurrentStageTimeout)));

                }
                catch
                {
                    return true;
                }
#endif
            }
        }

        public double IdleSeconds
        {
            get
            {
#if (NoSocketTimeouts)
                return 0;
#else
                try
                {
                    return Timer.Duration - BitConverter.Int64BitsToDouble(Interlocked.Read(ref LastActivityTimeStamp));
                }
                catch
                {
                    return 0;
                }
#endif
            }
        }

        public string RemoteAddress
        {
            get
            {
                if (saeaHandler == null || saeaHandler.AcceptSocket == null) return "unknown";

                return ((IPEndPoint)saeaHandler.AcceptSocket.RemoteEndPoint).Address.ToString();
            }
        }

        public int RemotePort
        {
            get
            {
                if (saeaHandler == null || saeaHandler.AcceptSocket == null) return 0;

                return ((IPEndPoint)saeaHandler.AcceptSocket.RemoteEndPoint).Port;
            }
        }

        public string RemoteAddressAndPort
        {
            get
            {
                return RemoteAddress + ":" + RemotePort;
            }
        }

        public string LocalAddress
        {
            get
            {
                if (saeaHandler == null || saeaHandler.AcceptSocket == null) return "unknown";

                return ((IPEndPoint)saeaHandler.AcceptSocket.LocalEndPoint).Address.ToString() + ":" +
                    ((IPEndPoint)saeaHandler.AcceptSocket.LocalEndPoint).Port.ToString();
            }
        }

        public int LocalPort
        {
            get
            {
                if (saeaHandler == null || saeaHandler.AcceptSocket == null) return 0;

                return ((IPEndPoint)saeaHandler.AcceptSocket.LocalEndPoint).Port;
            }
        }

        public string LocalAddressAndPort
        {
            get
            {
                return LocalAddress + ":" + LocalPort;
            }
        }

        #endregion

        #region Handler Capturing

        //public bool isCH = false;
        void SetCaptured(SocketHandlerBase Registree)
        {
            if (IsCaptured)
                throw new InvalidOperationException();
            lock (CaptureRequestQueue)
            {
                IsCaptured = true;
                _CapturingHandler = Registree;

                SocketIO.Completed -= OnSocketIOCompleted;
                SocketIO.Completed -= OnCapturedSocketIOCompleted;
                SocketIO.Completed += OnCapturedSocketIOCompleted;
            }
        }

        SocketHandlerBase ClearCapture()
        {
            if (!IsCaptured)
                throw new InvalidOperationException();
            lock (CaptureRequestQueue)
            {
                SocketHandlerBase handler = _CapturingHandler;

                //_CapturingSaea.Handler().isCH = false;

                IsCaptured = false;
                _CapturingHandler = null;

                SocketIO.Completed -= OnSocketIOCompleted;
                SocketIO.Completed -= OnCapturedSocketIOCompleted;
                SocketIO.Completed += OnSocketIOCompleted;

                return handler;
            }
        }

        // enqueues a new capture request if the queue is not full, returns if this is the first request, all as one atomic operation
        bool TryAddCaptureRequest(SocketHandlerBase Registree, int MaxCaptureRegistrees, out bool IsFirstQueuedCapture)
        {
            lock (CaptureRequestQueue)
            {
                if ((CaptureRequestQueue.Count >= MaxCaptureRegistrees) && (!Closing))
                {
                    IsFirstQueuedCapture = false;
                    return false;
                }

                CaptureRequestQueue.Enqueue(Registree);
                IsFirstQueuedCapture = CaptureRequestQueue.Count == 1;
            }

            return true;
        }

        // tries to dequeue the next capture request registree as one atomic operation
        bool TryGetNextCaptureRequest(out SocketHandlerBase Registree)
        {
            lock (CaptureRequestQueue)
            {
                if (CaptureRequestQueue.Count > 0)
                {
                    Registree = CaptureRequestQueue.Dequeue();

                    return true;
                }
            }
            Registree = null;
            return false;
        }

        // returns true if one or more captures are queued
        protected bool CaptureWaiting()
        {
            lock (CaptureRequestQueue)
            {
                return (CaptureRequestQueue.Count > 0);
            }
        }

        // tries to register a new capture request and starts it if this handler is IDLE
        public virtual NextRequestedHandlerAction EnqueueCapture(SocketHandlerBase Registree, int MaxCaptureRegistrees)
        {
            /*
             * Thread/Synchronization/EventChain info:
             * 
             * When this is the first queued capture and this handler is IDLE, 
             * processing starts right away, if at the same moment
             * this handler's state becomes closed or closing
             * processing of this captured event chain, will stop when:
             * - in async socket operation: the moment the socket get's closed
             * - starting an async operation
             * - somewhere in between when both handler's state get checked (see methods region "Other Handler Captured state handling")
             * - the CloseAndRemoveHandler method calls ReleaseAllCaptures
             */

            // Check HandlerState
            if (!Closing)
            {

                // See if there is enough queue space, if so, add this registree, then check HandlerState again
                // to prevent race conditions
                if (TryAddCaptureRequest(Registree, MaxCaptureRegistrees, out bool IsFirstQueuedCapture) && (!Closing))
                {
                    // First one in line?
                    if (IsFirstQueuedCapture)
                    {
                        // Start handling if we are in IDLE state
                        TryStartNextCaptureRequest(true, false);
                    }

                    // the flow of the calling chain should be terminated
                    return NextRequestedHandlerAction.Handled;
                }
            }

            // unrequested capture reset
            return Registree.HandleCapturedSocketReset(false);
        }

        // set's and start a new captured state or clears a possible previous one
        protected bool TryStartNextCaptureRequest(bool OnlyAllowWhenInIdleState = false, bool OnSetIdle = true)
        {
            /*
             * Thread/Synchronization/EventChain info:
             * 
             * 
             */

            // Set handler's state to active if it was IDLE, check current state, continue if all OK
            if (Interlocked.CompareExchange(ref _HandlerState, 1, 0) < (OnlyAllowWhenInIdleState ? 1 : 2))
            {

                // try to get next enqueued capture registree
                if (TryGetNextCaptureRequest(out SocketHandlerBase Registree))
                {
                    // Set captering handler state to active if it was IDLE, continue if not in closing state
                    if (Interlocked.CompareExchange(ref Registree._HandlerState, 1, 0) < 2)
                    {
                        // make sure not waiting anymore or awaiting polltimer
                        ClearPollIngState();

                        // Set captured
                        SetCaptured(Registree);

                        // Start handling
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, FireUserEventAndDetermineNextAction(SocketHandlerActions.CaptureAccquired));

                        // the flow of the calling chain should be terminated here
                        // without it changing it state
                        return true;
                    }
                    else
                    {
                        // close this captering handler
                        Registree.CloseAndReturnHandler();

                        // try again
                        return TryStartNextCaptureRequest(OnlyAllowWhenInIdleState, OnSetIdle);
                    }
                }
            }

            // the flow of the calling chain should be terminated
            // after changing the handler's state to IDLE
            // this will do that
            if (OnSetIdle)
            {
                // set state IDLE (if still active and not marked for termination) otherwise if closing return handler
                if (TrySetIdle())
                {
                    CloseAndReturnHandler();
                }
            }

            return false;
        }

        protected bool TrySetIdle()
        {
            return Interlocked.CompareExchange(ref _HandlerState, 0, 1) > 1;
        }

        // releases the current capture
        protected virtual void ReleaseCapture(bool OnClosingSocket = false, bool Requested = false)
        {
            /*
             * Thread/Synchronization/EventChain info:
             * 
             * This method is called when terminating this handler or when the captured handler 
             * is closed while it has captured this handler.
             * 
             * Since this method is only executed in the captured event chain of this handler
             * we don't need to protect access to the CapturedHandler member
             * 
             */

            // check
            if (!IsCaptured)
                throw new InvalidOperationException();

            // init 
            SocketHandlerActions NextCapturingHandlerAction = SocketHandlerActions.Dispose;

            // inspect captering handler state
            switch (Interlocked.Read(ref CapturingHandler._HandlerState))
            {
                // IDLE, should not be possible
                case 0:
                    throw new InvalidOperationException();

                // Active, release was requested, let's determine what it wants next
                case 1:
                    NextCapturingHandlerAction =
                        FireUserEventAndDetermineNextAction(
                            Requested ? SocketHandlerActions.CaptureReleaseRequest
                                      : SocketHandlerActions.CaptureReleasedUnRequested
                        );
                    break;

                // Closing, it's socket is timed out, so dispose captering handler
                case 2:
                    break;

                // Closed, should normally not be possible
                case 3:
                    if (!OnClosingSocket)
                        throw new InvalidOperationException();
                    break;
            }

            // reference and clear
            SocketHandlerBase Registree = ClearCapture();

            // continue own event chain if not closing
            if (!OnClosingSocket)
            {
                // let the handler decide what to do next;
                ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, FireUserEventAndDetermineNextAction(SocketHandlerActions.CaptureReleased));
            }

            // now let the event chain of the now not so much capturing handler continue
            ThreadPool.QueueUserWorkItem(Registree.PerformNextHandlerActionAsync, NextCapturingHandlerAction);
        }

        // stops any captured event chain and removes all capture requests from the queue
        void ReleaseAllCaptures()
        {
            /*
             * Thread/Synchronization/EventChain info:
             * 
             * Only called when this handler is closed
             */

            // check
            if (!Closed)
                throw new InvalidOperationException();

            // remove all enqueued requests
            while (TryGetNextCaptureRequest(out SocketHandlerBase Registree))
            {
                // inspect registrees handler state
                switch (Interlocked.Read(ref Registree._HandlerState))
                {

                    // IDLE, should not be possible
                    case 0:
                        throw new InvalidOperationException();

                    // Active, release was unrequested, let it's event chain continue
                    case 1:
                        try
                        {
                            ThreadPool.QueueUserWorkItem(Registree.PerformNextHandlerActionAsync,
                                Registree.TranslateHandlerAction(
                                    Registree.HandleCapturedSocketReset(false)
                                )
                            );
                        }
                        catch
                        {
                            Registree.CloseAndReturnHandler();
                        }
                        break;

                    // Closing, it's socket is timed-out, let it's event chain continue
                    case 2:
                        Registree.CloseAndReturnHandler();
                        break;

                        // Closed, should not be possible
                        // case 3: throw new InvalidOperationException();
                }
            }

            // clear any current captured state
            if (IsCaptured)
            {
                ReleaseCapture(true);
            }
        }

        #endregion

        #region Messages

        protected void ClearAllMessages()
        {
            lock (MessageQueue)
            {
                MessageQueue.Clear();
            }
        }

        public void AddMessage(object Message)
        {
            if (Closing || Closed)
                return;
            if (Message == null)
                return;

            lock (MessageQueue)
            {
                MessageQueue.Enqueue(Message);
            }

            ClearPollIngStateAndContinue(this);
        }

        public bool HaveMessages
        {
            get
            {
                lock (MessageQueue)
                {
                    return MessageQueue.Count > 0;
                }
            }
        }

        protected bool TryGetNextMessage(out object Message)
        {
            lock (MessageQueue)
            {
                if (MessageQueue.Count == 0)
                {
                    Message = null;
                    return false;
                }
                Message = MessageQueue.Dequeue();
                return true;
            }
        }

        long connectedTimestamp;

        public TimeSpan TimeConnected
        {
            get
            {
                return TimeSpan.FromSeconds(Timer.Duration - BitConverter.Int64BitsToDouble(Interlocked.Read(ref connectedTimestamp)));
            }
        }

        #endregion

        #region Handler switching

        public virtual void RegisterHandler(SocketAsyncEventArgs saea, EventHandler<HandlerClosedEventArgs> HandlerClosedCallback = null)
        {
            UniqueSocketId = Interlocked.Increment(ref _NextUniqueIdx);
            saeaHandler = saea;
            SocketIO.Completed -= OnSocketIOCompleted;
            SocketIO.Completed -= OnCapturedSocketIOCompleted;
            SocketIO.Completed += OnSocketIOCompleted;

            if (HandlerClosedCallback != null)
            {
                OnHandlerClosed -= HandlerClosedCallback;
                OnHandlerClosed += HandlerClosedCallback;
            }
        }

        internal void UnRegisterHandler(SocketAsyncEventArgs saea)
        {
            saeaHandler = null;
            if (saea == null)
                return;

            SocketIO.Unregister(saea);
            SocketIO.Completed -= OnSocketIOCompleted;
            SocketIO.Completed -= OnCapturedSocketIOCompleted;
        }

        // For maintenance
        static long _startCounter;

        public void StartHandlingSocket(object state)
        {
            try
            {
                Interlocked.Increment(ref _startCounter);
                if (Interlocked.Increment(ref _startCounter) % 10000 == 0)
                {
                    GC.Collect();
                }

                // state not active any more?
                if (Interlocked.Read(ref _HandlerState) != 1)
                {
                    CloseAndReturnHandler();
                    return;
                }

                // init timestamp
                HandlerStartTimestamp = Timer.Duration;

                // prevent timeout
                SetCurrentStageTimeoutSeconds(20, true);
                SetActivityTimestamp();

                // init socket  
                SetNoDelay(true);
                PortNumber = SocketIO.GetLocalPort(saeaHandler);
                SocketIO.SetBlocking(saeaHandler, false);
                SocketIO.SetSendBufferSize(saeaHandler, MemoryConfiguration.SendBufferSize);
                SocketIO.SetReceiveBufferSize(saeaHandler, MemoryConfiguration.ReceiveBufferSize);

                PerformFirstHandlerAction();
            }
            catch (Exception e)
            {
                OnException(e);
                this.CloseAndReturnHandler();
            }
        }

        void PerformFirstHandlerAction()
        {
            // start
            ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, FireUserEventAndDetermineNextAction(SocketHandlerActions.Initialize));
        }

        public NextRequestedHandlerAction TakeOverConrolOfSocket(SocketAsyncEventArgs saea)
        {
            // check
            if ((ChildHandler != null) || (ParentHandler != null))
                throw new InvalidOperationException();

            // set state to active
            Interlocked.Exchange(ref _HandlerState, 1);

            // prevent timeout
            SetActivityTimestamp();
            SetCurrentStageTimeoutSeconds(20);

            lock (CaptureRequestQueue)
            {
                // store new handler
                ParentHandler = saea.Handler();
                ParentHandler.ChildHandler = this;

                // switch control
                saea.SetHandler(this);
                ParentHandler.UnRegisterHandler(saea);
                RegisterHandler(saea);
                saeaHandler = saea;

                SwitchSocketResourcesWithParentHandler(false);
            }

            // start handling
            ThreadPool.QueueUserWorkItem(StartHandlingSocket);

            // stop the current event chain, but don't change the previous handler's state
            return NextRequestedHandlerAction.Handled;
        }

        void SwitchSocketResourcesWithParentHandler(bool restoring)
        {
            // reference
            SslStream _SslConversionStream = SslConversionStream;
            AsyncSocketHandlerTlsStreamAdapter _SslAdapter = SslAdapter;
            DynamicBuffer _rxBuffer = rxBuffer;
            DynamicBuffer _txBuffer = txBuffer;
            bool _SslStarted = SslStarted;

            // switch
            SslConversionStream = ParentHandler.SslConversionStream;
            SslAdapter = ParentHandler.SslAdapter;
            rxBuffer = ParentHandler.rxBuffer;
            txBuffer = ParentHandler.txBuffer;
            SslStarted = ParentHandler.SslStarted;

            ParentHandler.SslConversionStream = _SslConversionStream;
            ParentHandler.SslAdapter = _SslAdapter;
            ParentHandler.rxBuffer = _rxBuffer;
            ParentHandler.txBuffer = _txBuffer;
            ParentHandler.SslStarted = _SslStarted;

            if (restoring)
            {
                if (_SslAdapter != null)
                {
                    _SslAdapter.SetHandler(ParentHandler);
                }
            }
            else
            {
                if (SslAdapter != null)
                {
                    SslAdapter.SetHandler(this);
                }
            }
        }

        internal void ReturnControlOfSocket(SocketAsyncEventArgs saea)
        {
            if (ParentHandler != null)
            {
                ClearAllMessages();

                SocketHandlerBase _ParentHandler;
                lock (CaptureRequestQueue)
                {
                    // switch control
                    saea.SetHandler(ParentHandler);
                    UnRegisterHandler(saea);
                    ParentHandler.RegisterHandler(saea);
                    ParentHandler.ChildHandler = null;

                    SwitchSocketResourcesWithParentHandler(true);

                    // reference
                    _ParentHandler = ParentHandler;
                    _ParentHandler.ChildHandler = null;

                    // reset 
                    ParentHandler = null;
                    saeaHandler = null;
                }

                // start handling
                ThreadPool.QueueUserWorkItem(_ParentHandler.StartHandlingSocket);

                CloseAndReturnHandler();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        void CloseParentHandler(SocketAsyncEventArgs saea)
        {
            // should return control to underlaying handler?
            if (ParentHandler != null)
            {
                // switch control
                saea.SetHandler(ParentHandler);
                UnRegisterHandler(saea);
                ParentHandler.RegisterHandler(saea);
                ParentHandler.ChildHandler = null;
                SwitchSocketResourcesWithParentHandler(true);

                // delegate
                ParentHandler.CloseAndReturnHandler();

                // reset
                ParentHandler = null;
            }
        }

        #endregion

        #region Handler Maintenance

        // called by SocketAsyncEventArgsQueuedPool class
        // to take care of timed-out sockets
        // or when it is disposing due to application shutdown
        public virtual void OnDequeueHandler()
        {
            SetCurrentStageTimeoutSeconds(20, true);
            SetActivityTimestamp();

            // set state to active, if it wasn't closed, throw!
            if (Interlocked.CompareExchange(ref _HandlerState, 1, 3) != 3)
            {
                System.Diagnostics.Debugger.Break();
                throw new InvalidOperationException();
            }

            // prevent timeout
            CreateSaea();
        }

        public virtual void OnEnqueueHandler()
        {

        }

        public void Close()
        {
            /*
             * Thread/Synchronization/EventChain info:
             *
             * Accessing saea.AcceptSocket is tricky,
             * the only moments it get's set is:
             * 
             *  - Before the event chain is running on accepting a new socket
             *  - At CloseAndRemoveHandler() when state is Closed
             *  - in OnHandleInitialize() for outgoing sockets, but at this point a timeout could not yet occur, due to the default 0 timeout value
             * 
             * For these reasons it is safe to access this member when we are in Active state
             * 
             * The possible race condition mentioned below can only occur once, 
             * and will be catched
             * 
             */

            // is called externally when the handler is timed out
            // or when the Handler pool is disposing

            // Set state to "closing" when it is "active", else step into next block
            if (Interlocked.CompareExchange(ref _HandlerState, 2, 1) != 1)
            {
                // Set state to "closing" when it is "idle", if so, step into next block
                if (Interlocked.CompareExchange(ref _HandlerState, 2, 0) == 0)
                {
                    // ok state was idle, now we have set it in closing,
                    // since it is not active, we can close it right away
                    CloseAndReturnHandler();
                    return;
                }
                else
                {
                    // Just to be sure, theoretically a racing condition could occur here
                    // but this one, catches them all
                    if (!Closing)
                    {
                        Close();
                        return;
                    }

                    // at this point, we notice, it was already closing or closed
                }
            }
            else
            {
                // it was "active", now "closing"
                // close socket
                try
                {
                    // we have a socket?
                    SocketIO.Close(ControllingHandler.saeaHandler);
                }
                catch
                {
                }

                try
                {
                    ClearPollIngStateAndContinue(null);
                }
                catch
                {
                    // ignore
                }
            }
        }

        // closes the socket, cleans-up the instance and let the handler queue return it to the pool
        void CloseAndReturnHandler()
        {
            /*
             * Thread/Synchronization/EventChain info:
             * 
             * 
             * 
             */

            // set closed, and step in the next block if it wasn't already
            if (Interlocked.Exchange(ref _HandlerState, 3) != 3)
            {
                //if (isCH) System.Diagnostics.Debugger.Break();


                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Closing socket"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // reset state
                // Release all captures if present
                ReleaseAllCaptures();

                // Release all awaiteners out of there wait state

                lock (SafeBufferAccessLock)
                {
                    ClearPollIngState();
                }
                SignalAllAwaiteners();

                // first close the socket
                CloseAndDereferenceSocket();

                // Reset current stage timeout
                SetCurrentStageTimeoutSeconds(20);

                // close underlaying handler if present
                CloseParentHandler(saeaHandler);

                lock (SafeBufferAccessLock)
                {
                    //Reset the handler
                    HandleReset();
                }

                // Reset flag
                SetActivityTimestamp();

                // Clear all queued messages
                ClearAllMessages();

                // Dispose saea
                DisposeSaea();

                // finally, return handler to the queue
                var callback = OnHandlerClosed;
                if (callback != null)
                {
                    callback(this, new HandlerClosedEventArgs(this));
                }
                else
                {
                    Dispose(true);
                }
            }
            //else if (Closed) System.Diagnostics.Debugger.Break();
        }

        void CloseAndDereferenceSocket()
        {
            // do a shutdown before you close the socket
            try
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () => { return "Shutting down socket"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                //This method closes the socket and releases all resources, both
                //managed and unmanaged. It internally calls Dispose.
                try
                {
                    SocketIO.SetLingerState(saeaHandler, false);
                }
                catch { }
                try
                {
                    SocketIO.Close(saeaHandler);
                }
                catch { }
            }
            // throws if socket was already closed
#if (Logging)
            catch (ObjectDisposedException)
#else
            catch (ObjectDisposedException)
#endif
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                //Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Closing socket, failed, already closed";});
                //Logger.LogException(e, this.GetType().Name + ": Exception occurred");
#endif
                #endregion ------------------------------------------------------------------------------------------------
            }
            // unknown
#if (Logging)
            catch (SocketException)
#else
            catch (SocketException)
#endif
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                //Logger.LogHandlerFlowMessage(saeaHandler, "Shuttingdown socket, failed: " + e.Message);
                //Logger.LogException(e, this.GetType().Name + ": Exception occurred");
#endif
                #endregion ------------------------------------------------------------------------------------------------
            }

            if (saeaHandler != null)
            {
                saeaHandler.AcceptSocket = null;
            }

            // reset flag
            IsInAsyncCall = false;
        }

        #endregion

        #region Socket I/O handling

        internal void OnSocketIOCompleted(object sender, SocketAsyncEventArgs saea)
        {
            try
            {
                // check
                if (saea != saeaHandler)
                    throw new InvalidOperationException();
                if (saea.Handler() != this)
                    throw new InvalidOperationException();
                if (IsCaptured)
                    throw new InvalidOperationException();

                // reset flag
                IsInAsyncCall = false;

                // prevent timeout
                SetActivityTimestamp(true);

                // init action
                SocketHandlerActions Action = SocketHandlerActions.Dispose;

                // still in active (not idle/closing/closed) state?
                if (Interlocked.Read(ref _HandlerState) == 1)
                {
                    var lastOperation = SocketIO.GetLastOperation(saea);

                    // operation successfull?
                    if ((SocketIO.IsConnectionlessSocket(saea) ||
                           SocketIO.IsConnected(saea) ||
                           (lastOperation == SocketAsyncOperation.Disconnect)
                        ) &&
                        (SocketIO.GetSocketError(saea) == SocketError.Success))
                    {
                        // determine next action                    
                        switch (lastOperation)
                        {
                            case SocketAsyncOperation.Connect:

                                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Connected"; });
#endif
                                #endregion ------------------------------------------------------------------------------------------------

                                Action = SocketHandlerActions.Connect;
                                break;

                            case SocketAsyncOperation.Disconnect:

                                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Disconnected"; });
#endif
                                #endregion ------------------------------------------------------------------------------------------------

                                Action = SocketHandlerActions.Disconnect;
                                break;
                            case SocketAsyncOperation.Receive:

                                var bytes = SocketIO.GetBytesTransfered(saeaHandler);
                                if (bytes > 0)
                                {
                                    TriggerOnMeasureBytesReceived(Convert.ToUInt64(bytes));

                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                    Logger.LogSocketFlowMessage(saeaHandler, () => { return "Received " + bytes.ToString() + " bytes"; });
#endif
                                    #endregion ------------------------------------------------------------------------------------------------

                                    Action = SocketHandlerActions.Receive;
                                }
                                else
                                {
                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                    Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Socket disconnected by peer"; });
#endif
                                    #endregion ------------------------------------------------------------------------------------------------
                                }
                                break;

                            case SocketAsyncOperation.Send:

                                bytes = SocketIO.GetBytesTransfered(saeaHandler);
                                if (bytes > 0)
                                {
                                    TriggerOnMeasureBytesSend(Convert.ToUInt64(bytes));

                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                    Logger.LogSocketFlowMessage(saeaHandler, () => { return "Sent " + bytes.ToString() + " bytes"; });
#endif
                                    #endregion ------------------------------------------------------------------------------------------------

                                    Action = SocketHandlerActions.Send;
                                }
                                else
                                {
                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                    Logger.LogHandlerFlowMessage(saeaHandler, () => { return "Socket disconnected by peer"; });
#endif
                                    #endregion ------------------------------------------------------------------------------------------------
                                }
                                break;

                            default:
                                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                Logger.LogSocketFlowMessage(saeaHandler, () =>
                                {
                                    return "Unexpected last-operation: " + Enum.GetName(typeof(SocketAsyncOperation), lastOperation);
                                });
#endif
                                #endregion ------------------------------------------------------------------------------------------------

                                CloseAndReturnHandler();
                                return;
                        }
                    }
                    else
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogSocketFlowMessage(saeaHandler, () =>
                        {
                            return "Operation not successfull, last operation: " + Enum.GetName(typeof(SocketAsyncOperation), lastOperation);
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------
                    }
                }
                else
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () => { return "Handler closed, disposing"; });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    //if (CapturingHandler.Closed) System.Diagnostics.Debugger.Break(); // <--
                    Action = SocketHandlerActions.Dispose;
                }

                // delegate
                ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, FireUserEventAndDetermineNextAction(Action));
            }
            catch
            {
                CloseAndReturnHandler();
            }
        }

        internal void OnCapturedSocketIOCompleted(object sender, SocketAsyncEventArgs saea)
        {
            try
            {
                // check
                if (saea != saeaHandler)
                    throw new InvalidOperationException();
                if (saea.Handler() != this)
                    throw new InvalidOperationException();
                if (!IsCaptured)
                    throw new InvalidOperationException();

                // reset flag
                IsInAsyncCall = false;

                // prevent timeout
                SetActivityTimestamp(true);
                saea.Handler().SetActivityTimestamp(true);

                // init action
                SocketHandlerActions Action = SocketHandlerActions.Dispose;

                // still in active (not idle/closing/closed) state?
                if (Interlocked.Read(ref _HandlerState) == 1)
                {
                    // still in active (not idle/closing/closed) state?
                    if (Interlocked.Read(ref CapturingHandler._HandlerState) == 1)
                    {
                        var lastOperation = SocketIO.GetLastOperation(saea);

                        // operation successfull?
                        if ((SocketIO.IsConnectionlessSocket(saea) ||
                               SocketIO.IsConnected(saea) ||
                               (lastOperation == SocketAsyncOperation.Disconnect)
                            ) &&
                            (SocketIO.GetSocketError(saea) == SocketError.Success))
                        {
                            // determine next action                        
                            switch (lastOperation)
                            {
                                case SocketAsyncOperation.Receive:

                                    // operation successfull?
                                    var bytes = SocketIO.GetBytesTransfered(saea);
                                    if (bytes > 0)
                                    {
                                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                        Logger.LogSocketFlowMessage(saea, () => { return "OnCapturedSocketIOCompleted, Received " + bytes.ToString() + " bytes"; });
#endif
                                        #endregion ------------------------------------------------------------------------------------------------

                                        Action = SocketHandlerActions.CapturedReceive;
                                    }
                                    else
                                    {
                                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                        Logger.LogHandlerFlowMessage(saea, () => { return "OnCapturedSocketIOCompleted, socket disconnected by peer"; });
#endif
                                        #endregion ------------------------------------------------------------------------------------------------
                                    }
                                    break;

                                case SocketAsyncOperation.Send:

                                    // operation successfull?
                                    bytes = SocketIO.GetBytesTransfered(saea);
                                    if (bytes > 0)
                                    {
                                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                        bytes = SocketIO.GetBytesTransfered(saea);

                                        Logger.LogSocketFlowMessage(saea, () => { return "OnCapturedSocketIOCompleted, Sent " + bytes.ToString() + " bytes"; });
#endif
                                        #endregion ------------------------------------------------------------------------------------------------

                                        Action = SocketHandlerActions.CapturedSend;
                                    }
                                    else
                                    {
                                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                        Logger.LogSocketFlowMessage(saea, () => { return "OnCapturedSocketIOCompleted, socket disconnected by peer"; });
#endif
                                        #endregion ------------------------------------------------------------------------------------------------
                                    }
                                    break;

                                case SocketAsyncOperation.Disconnect:

                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                    Logger.LogSocketFlowMessage(saea, () => { return "OnCapturedSocketIOCompleted, Disconnected"; });
#endif
                                    #endregion ------------------------------------------------------------------------------------------------

                                    Action = SocketHandlerActions.CapturedDisconnect;
                                    break;

                                default:
                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                    Logger.LogSocketFlowMessage(saea, () =>
                                    {
                                        return "OnCapturedSocketIOCompleted, Unexpected last-operation: " + Enum.GetName(typeof(SocketAsyncOperation), lastOperation);
                                    });
#endif
                                    #endregion ------------------------------------------------------------------------------------------------

                                    throw new InvalidOperationException();
                            }
                        }
                        else
                        {
                            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                            Logger.LogSocketFlowMessage(saea, () =>
                            {
                                return "OnCapturedSocketIOCompleted, operation not successfull, last operation: " + Enum.GetName(typeof(SocketAsyncOperation), lastOperation);
                            });
#endif
                            #endregion ------------------------------------------------------------------------------------------------
                        }
                    }
                    else
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogSocketFlowMessage(saea, () => { return "OnCapturedSocketIOCompleted, handler closed, disposing"; });
#endif
                        #endregion ------------------------------------------------------------------------------------------------

                        //if (CapturingHandler.Closed) System.Diagnostics.Debugger.Break(); // <--
                        Action = SocketHandlerActions.DisposeCapturingHandler;
                    }
                }
                //else if (Closed) System.Diagnostics.Debugger.Break();

                // delegate
                ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, FireUserEventAndDetermineNextAction(Action));
            }
            catch
            {
                CloseAndReturnHandler();
            }
        }

        #endregion

        #region Polling

        // Called by another handler who wants to await a signal
        // from this handler as an alternative to polling
        // for faster response times
        public void WakeupOnSignalFrom(SocketHandlerBase supplier, string Signal, HandleSocketBEventArgs e, bool UsePollTimeout)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
            Logger.LogSocketFlowMessage(
                 ControllingHandler.saeaHandler, () =>
            {
                return "Awaiting signal " + Signal + " from " + supplier.GetType().Name;
            });

#endif
#endif
            #endregion ------------------------------------------------------------------------------------------------

            e.NextAction = UsePollTimeout ?
                NextRequestedHandlerAction.Poll :
                NextRequestedHandlerAction.WaitForSignal;

            // register signal waitener
            lock (supplier.PollingStateLock)
            {
                supplier.GetSignalWaitenersHandlers(Signal, this);
            }

            // register signal supplier
            lock (PollingStateLock)
            {
                GetSignalSupplierHandlers(Signal, supplier);
            }
        }

        // Signals to other sockethandlers that a condition has occured
        // on which they can synchronize, as an alternative to polling
        // for faster response times
        public void Signal(string Signal)
        {
            SafeAccess(() =>
            {
                SocketHandlerBase[] waiteners;

                lock (PollingStateLock)
                {
                    // todo yield return?
                    var list = GetSignalWaitenersHandlers(Signal);
                    if (list == null) return;
                    waiteners = list.ToArray<SocketHandlerBase>();

                    SignalWaiteners.TryRemove(Signal, out List<SocketHandlerBase> h);
                }

                ThreadPool.QueueUserWorkItem((state) =>
                {
                    foreach (var handler in waiteners)
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogHandlerFlowMessage(saeaHandler, () =>
                                {
                                    return "Signalling awaiting " + handler.GetType().Name + ", with signal: " + Signal;
                                });
#endif
                        #endregion ------------------------------------------------------------------------------------------------

                        handler.ClearPollIngStateAndContinue(Signal);
                    }
                });
            });
        }

        // called when socket closes
        protected void SignalAllAwaiteners()
        {
            // least amount of locking
            while (true)
            {
                // new list
                var waiteners = new Dictionary<string, List<SocketHandlerBase>>();

                // lock
                lock (PollingStateLock)
                {
                    // copy list
                    foreach (var entry in SignalWaiteners)
                    {
                        waiteners[entry.Key] = entry.Value;
                    }
                }

                // no more awaiteners? then were done
                if (waiteners.Count == 0) return;

                // signal all signals
                foreach (var signal in waiteners.Keys)
                {
                    Signal(signal);
                }
            }
        }

        // updates the reference registrations of a signal
        void RemoveSignalWaitReferences()
        {
            List<string> signals = new List<string>();

            lock (PollingStateLock)
            {
                foreach (var signal in SignalSuppliers.Keys)
                {
                    signals.Add(signal);
                }
            }

            foreach (string Signal in signals)
            {
                // get list of other signals it is waiting for
                List<SocketHandlerBase> otherHandlers;
                lock (PollingStateLock)
                {
                    otherHandlers = GetSignalSupplierHandlers(Signal);

                    SignalSuppliers.TryRemove(Signal, out List<SocketHandlerBase> h);
                }

                // found?
                if (otherHandlers != null)
                {
                    // process each one
                    foreach (var otherHandler in otherHandlers)
                    {
                        // get list of other handlers who could have provided this signal
                        lock (otherHandler.PollingStateLock)
                        {
                            // find the container holding the reference
                            var otherSignalHandlers = otherHandler.GetSignalWaitenersHandlers(Signal);

                            // found?
                            if (otherSignalHandlers != null)
                            {
                                // lookup the reference
                                var idx = otherSignalHandlers.IndexOf(this);

                                // found?
                                while (idx >= 0)
                                {
                                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
                                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                                    {
                                        return "Removing signal awaitener registration on " + otherSignalHandlers.GetType().Name + ", for signal: " + Signal;
                                    });
#endif
#endif
                                    #endregion ------------------------------------------------------------------------------------------------

                                    // remove it
                                    otherSignalHandlers.RemoveAt(idx);

                                    // container now empty?
                                    if (otherSignalHandlers.Count == 0)
                                    {
                                        // remove signal 
                                        otherHandler.SignalWaiteners.TryRemove(Signal, out List<SocketHandlerBase> h);
                                        break;
                                    }

                                    idx = otherSignalHandlers.IndexOf(this);
                                }
                            }
                        }
                    }
                }
            }
        }

        // gets a container with all signals, other handlers are waiting on, optionally pre-initialized
        List<SocketHandlerBase> GetSignalWaitenersHandlers(string Signal, SocketHandlerBase signalWaitener = null)
        {
            // init

            // list not found, and we should add something?
            if (!SignalWaiteners.TryGetValue(Signal, out List<SocketHandlerBase> signalHandlers) && signalWaitener != null)
            {
                // create a new list
                signalHandlers = new List<SocketHandlerBase>();

                // set it
                SignalWaiteners[Signal] = signalHandlers;
            }

            // add something?
            if (signalWaitener != null && signalHandlers.IndexOf(signalWaitener) < 0)
            {
                // add registree
                signalHandlers.Add(signalWaitener);
            }

            // return container
            return signalHandlers;
        }

        // gets a container with all signals, this handler is currently waiting for, optionally pre-initialized
        List<SocketHandlerBase> GetSignalSupplierHandlers(string Signal, SocketHandlerBase signalSupplier = null)
        {
            // init

            // list not found, and we should add something?
            if (!SignalSuppliers.TryGetValue(Signal, out List<SocketHandlerBase> signalHandlers) && signalSupplier != null)
            {
                // create a new list
                signalHandlers = new List<SocketHandlerBase>();

                // set it
                SignalSuppliers[Signal] = signalHandlers;
            }

            // add something?
            if (signalSupplier != null && signalHandlers.IndexOf(signalSupplier) < 0)
            {
                // add registree
                signalHandlers.Add(signalSupplier);
            }

            // return container
            return signalHandlers;
        }

        //  custom lock
        object SafeBufferAccessLock = new object();

        // provides a context where it is safe to access transmission buffers
        // from another handler
        public void SafeAccess(LockCallback callback)
        {
            if (Closing || Closed)
            {
                throw new Exception("Socket is closed");
            }
            var sessionId = this.UniqueSocketId;

            lock (SafeBufferAccessLock)
            {
                if (Closing || Closed || this.UniqueSocketId != sessionId)
                {
                    throw new Exception("Socket is closed");
                }

                callback();
            }
        }

        bool ClearPollIngState()
        {
            // try to clear pollingstate from timeractive(1) to disabled(0)
            if (Interlocked.CompareExchange(ref _PollingState, 0, 1) == 1)
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Cleared polling state from timeractive to disabled";
                });
#endif
#endif
                #endregion ------------------------------------------------------------------------------------------------

                lock (PollingStateLock)
                {
                    // deactivate timer
                    PollTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
            else
            {
                // try to clear pollingstate from waiting(2) to disabled(0)
                if (Interlocked.CompareExchange(ref _PollingState, 0, 2) != 2)
                {
                    // not polling (yet) at this time
                    return false;
                }

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Cleared polling state from waiting to disabled";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------
            }

            // remove the references
            RemoveSignalWaitReferences();

            return true;
        }

        void ClearPollIngStateAndContinue(object state)
        {
            if (!ClearPollIngState())
            {
                return;
            }

            if (HaveMessages)
            {
                if (OnHandleNewMessage != null)
                {
                    PerformNextHandlerAction(SocketHandlerActions.ProcessReceivedMessage);
                    return;
                }
                else
                {
                    ClearAllMessages();
                }
            }

            // init next action
            // still in active (not idle/closing/closed) state?
            SocketHandlerActions NextAction = Interlocked.Read(ref _HandlerState) == 1 ?
                NextAction = SocketHandlerActions.Poll :
                SocketHandlerActions.Dispose;

            PerformNextHandlerAction(FireUserEventAndDetermineNextAction(NextAction));
        }

        private long _PollIntervalMs = 500;
        protected void SetPollIntervalMs(long NewInterval)
        {
            Interlocked.Exchange(ref _PollIntervalMs, NewInterval);
        }
        #endregion

        #region Handling

        void PerformNextHandlerActionAsync(object NextAction)
        {
            PerformNextHandlerAction((SocketHandlerActions)NextAction);
        }

        void PerformNextHandlerAction(SocketHandlerActions NextAction)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            if ((NextAction != SocketHandlerActions.Poll) && (NextAction != SocketHandlerActions.Handled))
            {
                Logger.LogSocketFlowMessage(saeaHandler, () => { return "Starting operation: " + Enum.GetName(typeof(SocketHandlerActions), NextAction); });
            }
#endif
            #endregion ------------------------------------------------------------------------------------------------

            if (Closing)
            {
                CloseAndReturnHandler();
                return;
            }

            var HandledAsync = true;
            try
            {
                switch (NextAction)
                {
                    case SocketHandlerActions.None:
                        break;

                    case SocketHandlerActions.Handled:

                        // clean-up handler if closing
                        if (Interlocked.Read(ref _HandlerState) == 2)
                        {
                            CloseAndReturnHandler();
                        }
                        break;

                    case SocketHandlerActions.SetIdle:

                        // try to start next capture, or set state IDLE
                        TryStartNextCaptureRequest();
                        break;

                    case SocketHandlerActions.Connect:

                        // remember connection time
                        Interlocked.Exchange(ref connectedTimestamp, BitConverter.DoubleToInt64Bits(Timer.Duration));

                        // connect
                        saeaHandler.SetBuffer(0, 0);
                        HandledAsync = (IsInAsyncCall = SocketIO.ConnectAsync(saeaHandler));
                        break;

                    case SocketHandlerActions.AsyncAction:
                        Task.Factory.StartNew(async delegate
                        {
                            try
                            {
                                await HandleAsyncAction();
                            }
                            catch (Exception e)
                            {
                                _HandleException(e);

                                CloseAndReturnHandler();
                            }
                        }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

                        return;

                    case SocketHandlerActions.Send:
                    case SocketHandlerActions.CapturedSend:

                        SetActivityTimestamp(true);

                        if (SslStarted)
                        {
                            Task.Factory.StartNew(async delegate
                            {
                                try
                                {
                                    await PerformSendSSL(saeaHandler);
                                }
                                catch (Exception e)
                                {
                                    _HandleException(e);

                                    CloseAndReturnHandler();
                                }

                            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

                            return;
                        }

                        SocketIO.SetReceiveBufferSize(saeaHandler, MemoryConfiguration.ReceiveBufferSize);
                        txBuffer.FillAndSetSendBuffer(saeaHandler, MemoryConfiguration.SendBufferSize, false);

                        HandledAsync = (IsInAsyncCall = SocketIO.SendAsync(saeaHandler));
                        break;

                    case SocketHandlerActions.Receive:
                    case SocketHandlerActions.CapturedReceive:

                        SetActivityTimestamp(true);

                        if (SslStarted)
                        {
                            var t = Task.Factory.StartNew(async delegate
                            {
                                try
                                {
                                    await PerformReceiveSSL(saeaHandler);
                                }
                                catch (Exception e)
                                {
                                    _HandleException(e);

                                    CloseAndReturnHandler();
                                }

                            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

                            return;
                        }

                        if (saeaHandler.Buffer == null)
                        {
                            SetReceiveBuffer(MemoryConfiguration.ReceiveBufferSize);
                        }

                        HandledAsync = (IsInAsyncCall = SocketIO.ReceiveAsync(saeaHandler));

                        break;


                    case SocketHandlerActions.StopTransportSecurity:
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(HandleStopTransportSecurity()));
                        break;

                    case SocketHandlerActions.Disconnect:
                    case SocketHandlerActions.CapturedDisconnect:

                        saeaHandler.SetBuffer(0, 0);
                        SocketIO.Shutdown(saeaHandler, SocketShutdown.Receive);
                        SocketIO.SetLingerTime(saeaHandler, 5000);
                        SocketIO.SetLingerState(saeaHandler, true);
                        SetActivityTimestamp();
                        SetCurrentStageTimeoutSeconds(5, false);

                        HandledAsync = (IsInAsyncCall = SocketIO.DisconnectAsync(saeaHandler));
                        break;

                    case SocketHandlerActions.Poll:

                        if (HaveMessages)
                        {
                            ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, SocketHandlerActions.ProcessReceivedMessage);
                        }
                        else
                        {
                            // start timer
                            lock (PollingStateLock)
                            {
                                PollTimer.Change(Convert.ToInt32(Interlocked.Read(ref _PollIntervalMs)), Timeout.Infinite);
                            }

                            // failed, try to set polling state from disabled to timer
                            if (Interlocked.CompareExchange(ref _PollingState, 1, 0) != 0)
                            {
                                throw new InvalidOperationException();
                            }
                            else
                            {
#if (LogPolling)
                                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                                Logger.LogSocketFlowMessage(saeaHandler, () =>
                                {
                                    return "Setting polling state from disabled to timer";
                                });
#endif
                                #endregion ------------------------------------------------------------------------------------------------
#endif
                            }


                            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)

                            Logger.LogSocketFlowMessage(saeaHandler, () =>
                            {
                                return "Starting polling timer";
                            });
#endif
#endif
                            #endregion ------------------------------------------------------------------------------------------------

                        }

                        break;

                    case SocketHandlerActions.WaitForSignal:

                        if (SignalSuppliers.Count == 0)
                        {
                            throw new Exception("No signals registered to wait for!");
                        }

                        // try to set polling state from disabled to wait
                        if (Interlocked.CompareExchange(ref _PollingState, 2, 0) != 0)
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (LogPolling)
#if (Logging)
                            Logger.LogHandlerFlowMessage(saeaHandler, () =>
                            {
                                return "Setting polling state from disabled to wait";
                            });
#endif
#endif
                            #endregion ------------------------------------------------------------------------------------------------
                        }


                        break;

                    case SocketHandlerActions.WaitForMessage:

                        if (HaveMessages)
                        {
                            ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, SocketHandlerActions.ProcessReceivedMessage);
                            return;
                        }

                        // failed, try to set polling state from disabled to wait
                        if (Interlocked.CompareExchange(ref _PollingState, 2, 0) != 0)
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (LogPolling)
#if (Logging)
                            Logger.LogSocketFlowMessage(saeaHandler, () =>
                            {
                                return "Setting polling state from disabled to wait";
                            });
#endif
#endif
                            #endregion ------------------------------------------------------------------------------------------------
                        }

                        break;

                    case SocketHandlerActions.ProcessReceivedMessage:

                        object message;

                        lock (MessageQueue)
                        {

                            message = MessageQueue.Dequeue();
                        }

                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(HandleOnNewMessage(message)));

                        break;

                    case SocketHandlerActions.Dispose:

                        CloseAndReturnHandler();

                        break;

                    case SocketHandlerActions.ReleaseControl:

                        ReturnControlOfSocket(saeaHandler);

                        break;

                    case SocketHandlerActions.CaptureReleaseRequest:
                        ReleaseCapture(false, true);
                        break;

                    case SocketHandlerActions.CaptureReleasedUnRequested:
                        ReleaseCapture(false, false);
                        break;

                    case SocketHandlerActions.DisposeCapturingHandler:

                        CapturingHandler.Close();
                        ReleaseCapture();

                        break;

                    default:
                        throw new InvalidOperationException();
                }

                if (Closing)
                {
                    CloseAndReturnHandler();
                }

                if (!HandledAsync)
                {
                    if (IsCaptured)
                    {
                        OnCapturedSocketIOCompleted(null, saeaHandler);
                    }
                    else
                    {
                        OnSocketIOCompleted(null, saeaHandler);
                    }
                }
            }
            catch (Exception e)
            {
                OnException(e);
                CloseAndReturnHandler();
            }
        }

        SocketHandlerActions TranslateHandlerAction(NextRequestedHandlerAction NextAction)
        {
            switch (NextAction)
            {
                case NextRequestedHandlerAction.Connect:
                    return SocketHandlerActions.Connect;
                case NextRequestedHandlerAction.AsyncAction:
                    return SocketHandlerActions.AsyncAction;
                case NextRequestedHandlerAction.Receive:
                    return SocketHandlerActions.Receive;
                case NextRequestedHandlerAction.Send:
                    return SocketHandlerActions.Send;
                case NextRequestedHandlerAction.StopTransportSecurity:
                    return SocketHandlerActions.StopTransportSecurity;
                case NextRequestedHandlerAction.Disconnect:
                    return SocketHandlerActions.Disconnect;
                case NextRequestedHandlerAction.Poll:
                    return SocketHandlerActions.Poll;
                case NextRequestedHandlerAction.WaitForSignal:
                    return SocketHandlerActions.WaitForSignal;
                case NextRequestedHandlerAction.WaitForMessage:
                    return SocketHandlerActions.WaitForMessage;
                case NextRequestedHandlerAction.Dispose:
                    return SocketHandlerActions.Dispose;
                case NextRequestedHandlerAction.SetIdle:
                    return SocketHandlerActions.SetIdle;
                case NextRequestedHandlerAction.Handled:
                    return SocketHandlerActions.Handled;
                case NextRequestedHandlerAction.ReleaseControl:
                    return SocketHandlerActions.ReleaseControl;
                default:
                    throw new NotImplementedException();
            }
        }

        SocketHandlerActions TranslateHandlerAction(NextRequestedCapturedHandlerAction NextAction)
        {
            switch (NextAction)
            {
                case NextRequestedCapturedHandlerAction.Handled:
                    return SocketHandlerActions.Handled;
                case NextRequestedCapturedHandlerAction.Receive:
                    return SocketHandlerActions.CapturedReceive;
                case NextRequestedCapturedHandlerAction.Send:
                    return SocketHandlerActions.CapturedSend;
                case NextRequestedCapturedHandlerAction.Disconnect:
                    return SocketHandlerActions.CapturedDisconnect;
                case NextRequestedCapturedHandlerAction.DisposeCapturedHandler:
                    return SocketHandlerActions.Dispose;
                case NextRequestedCapturedHandlerAction.DisposeCapturingHandler:
                    return SocketHandlerActions.DisposeCapturingHandler;
                case NextRequestedCapturedHandlerAction.Release:
                    return SocketHandlerActions.CaptureReleaseRequest;
                default:
                    throw new NotImplementedException();
            }
        }

        internal SocketHandlerActions FireUserEventAndDetermineNextAction(SocketHandlerActions Action)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            if (Action != SocketHandlerActions.Poll)
            {
                Logger.LogSocketFlowMessage(saeaHandler, () => { return "Handling action: " + Enum.GetName(typeof(SocketHandlerActions), Action); });
            }
#endif
            #endregion ------------------------------------------------------------------------------------------------
            try
            {
                switch (Action)
                {
                    case SocketHandlerActions.Initialize:
                        return TranslateHandlerAction(HandleInitialize());

                    case SocketHandlerActions.Connect:
                        return TranslateHandlerAction(HandleConnect());

                    case SocketHandlerActions.AsyncAction:

                        throw new InvalidOperationException();

                    case SocketHandlerActions.Send:

                        if (SslStarted)
                        {
                            SslAdapter.HandleSend();
                            return SocketHandlerActions.Handled;
                        }

                        return TranslateHandlerAction(HandleSend());

                    case SocketHandlerActions.Receive:

                        if (SslStarted)
                        {
                            SslAdapter.HandleReceive();
                            return SocketHandlerActions.Handled;
                        }

                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogSocketFlowMessage(saeaHandler, () => { return "Received (no ssl) " + SocketIO.GetBytesTransfered(saeaHandler).ToString() + " bytes into receivebuffer"; });
#endif
                        #endregion ------------------------------------------------------------------------------------------------

                        SocketIO.GetReceivedData(saeaHandler, rxBuffer);

                        return TranslateHandlerAction(HandleReceive());

                    case SocketHandlerActions.StopTransportSecurity:

                        throw new InvalidOperationException();

                    case SocketHandlerActions.Disconnect:

                        return TranslateHandlerAction(HandleDisconnect());

                    case SocketHandlerActions.Poll:

                        return TranslateHandlerAction(HandlePoll());

                    case SocketHandlerActions.WaitForSignal:

                        throw new InvalidOperationException();

                    case SocketHandlerActions.WaitForMessage:

                        throw new InvalidOperationException();

                    case SocketHandlerActions.ProcessReceivedMessage:

                        object message;

                        lock (MessageQueue)
                        {
                            message = MessageQueue.Dequeue();
                        }

                        return TranslateHandlerAction(HandleOnNewMessage(message));

                    case SocketHandlerActions.ReleaseControl:
                        return SocketHandlerActions.Initialize;

                    case SocketHandlerActions.CaptureReleased:
                        return TranslateHandlerAction(HandleCaptureRelease());

                    case SocketHandlerActions.CaptureAccquired:
                        return TranslateHandlerAction(CapturingHandler.HandleCapturedSocketInitialize(saeaHandler, rxBuffer, txBuffer));

                    case SocketHandlerActions.CaptureReleaseRequest:
                        return TranslateHandlerAction(CapturingHandler.HandleCapturedSocketReset(true));

                    case SocketHandlerActions.CaptureReleasedUnRequested:
                        return TranslateHandlerAction(CapturingHandler.HandleCapturedSocketReset(false));

                    case SocketHandlerActions.CapturedSend:

                        if (SslStarted)
                        {
                            SslAdapter.HandleSend();
                            return SocketHandlerActions.Handled;
                        }

                        return TranslateHandlerAction(CapturingHandler.HandleCapturedSocketSendSuccess(saeaHandler, rxBuffer, txBuffer));

                    case SocketHandlerActions.CapturedDisconnect:
                        return TranslateHandlerAction(CapturingHandler.HandleCapturedSocketDisconnectSuccess(saeaHandler));

                    case SocketHandlerActions.CapturedReceive:

                        if (SslStarted)
                        {
                            SslAdapter.HandleReceive();
                            return SocketHandlerActions.Handled;
                        }

                        SocketIO.GetReceivedData(saeaHandler, rxBuffer);

                        return TranslateHandlerAction(CapturingHandler.HandleCapturedSocketReceiveSuccess(saeaHandler, rxBuffer, txBuffer));

                    default:
                        return Action;
                }
            }
            #region Exception Handling
#if (Logging)
            catch (Exception e)
#else
            catch (Exception e)
#endif
            {
                _HandleException(e);

                throw;
            }
            #endregion
        }

        #endregion

        #region Handling Events

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandleInitialize;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleInitializeAsync;
        NextRequestedHandlerAction HandleInitialize()
        {
            var handler = OnHandleInitialize;
            var asyncHandler = OnHandleInitializeAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleInitialize is not handled, you need to assign the normal or async eventhandler");
            }

            if (_args_Uncaptured == null)
            {
                _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
            }
            else
            {
                _args_Uncaptured.RxBuffer = rxBuffer;
                _args_Uncaptured.TxBuffer = txBuffer;
                _args_Uncaptured.Count = null;
                _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {
                handler(this, _args_Uncaptured);

                CheckArgumentsCountProperty(_args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                    {

                        return this.GetType().Name + " | " +
                            "HandleInitialize -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                            (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                    });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }

                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                    if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                        Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                        {
                            return this.GetType().Name + " | " +
                             "HandleInitialize -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                            (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                        });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandleCaptureRelease;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleCaptureReleaseAsync;
        NextRequestedHandlerAction HandleCaptureRelease()
        {
            var handler = OnHandleCaptureRelease;
            var asyncHandler = OnHandleCaptureReleaseAsync;

            if (handler == null && asyncHandler == null)
            {

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleCaptureRelease -> [DEFAULT] DISPOSE (no handler)";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                return NextRequestedHandlerAction.Dispose;
            }

            if (_args_Uncaptured == null)
            {
                _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
            }
            else
            {
                _args_Uncaptured.RxBuffer = rxBuffer;
                _args_Uncaptured.TxBuffer = txBuffer;
                _args_Uncaptured.Count = null;
                _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {

                handler(this, _args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleCaptureRelease -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }

                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                    if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                        Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                    {
                        return this.GetType().Name + " | " +
                            "HandlCaptureRelease -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandlePoll;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandlePollAsync;
        NextRequestedHandlerAction HandlePoll()
        {
            var handler = OnHandlePoll;
            var asyncHandler = OnHandlePollAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandlePoll is not handled, you need to assign the eventhandler");
            }

            if (_args_Uncaptured == null)
            {
                _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
            }
            else
            {
                _args_Uncaptured.RxBuffer = rxBuffer;
                _args_Uncaptured.TxBuffer = txBuffer;
                _args_Uncaptured.Count = null;
                _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {
                handler(this, _args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandlePoll -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }

                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                    if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                        Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                        {
                            return this.GetType().Name + " | " +
                             "HandlePoll -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                            (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                        });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketMessageEventArgs> OnHandleNewMessage;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleNewMessageAsync;
        NextRequestedHandlerAction HandleOnNewMessage(object Message)
        {
            var handler = OnHandleNewMessage;
            var asyncHandler = OnHandleNewMessageAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleNewMessage is not handled, you need to assign the eventhandler");
            }

            if (_args_OnNewMessage == null)
            {
                _args_OnNewMessage = new HandleSocketMessageEventArgs(rxBuffer, txBuffer, Message);
            }
            else
            {
                _args_OnNewMessage.Message = Message;
                _args_OnNewMessage.RxBuffer = rxBuffer;
                _args_OnNewMessage.TxBuffer = txBuffer;
                _args_OnNewMessage.Count = null;
                _args_OnNewMessage.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {
                handler(this, _args_OnNewMessage);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                if (_args_OnNewMessage.NextAction != NextRequestedHandlerAction.Poll)

#endif
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleOnNewMessage -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_OnNewMessage.NextAction) +
                        (_args_OnNewMessage.Count.HasValue ? " (Count = " + _args_OnNewMessage.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_OnNewMessage);

                return _args_OnNewMessage.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_OnNewMessage);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_OnNewMessage);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                    if (_args_OnNewMessage.NextAction != NextRequestedHandlerAction.Poll)

#endif
                        Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                    {
                        return this.GetType().Name + " | " +
                         "HandleOnNewMessage -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_OnNewMessage.NextAction) +
                        (_args_OnNewMessage.Count.HasValue ? " (Count = " + _args_OnNewMessage.Count.Value.ToString() + ")" : "");
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_OnNewMessage);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_OnNewMessage.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandleConnect;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleConnectAsync;
        NextRequestedHandlerAction HandleConnect()
        {
            var handler = OnHandleConnect;
            var asyncHandler = OnHandleConnectAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleConnect is not handled, you need to assign the eventhandler");
            }

            if (_args_Uncaptured == null)
            {
                _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
            }
            else
            {
                _args_Uncaptured.RxBuffer = rxBuffer;
                _args_Uncaptured.TxBuffer = txBuffer;
                _args_Uncaptured.Count = null;
                _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {

                handler(this, _args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
            {
                return this.GetType().Name + " | " +
                    "HandleConnect -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                    (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
            });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleConnect -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                    (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketEventArgs> OnHandleDisconnect;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketEventArgs, Task> OnHandleDisconnectAsync;
        NextRequestedHandlerAction HandleDisconnect()
        {
            var handler = OnHandleDisconnect;
            var asyncHandler = OnHandleDisconnectAsync;

            if (handler == null && asyncHandler == null)
            {
                return NextRequestedHandlerAction.Dispose;
            }

            if (_args_HandleDisconnect == null)
            {
                _args_HandleDisconnect = new HandleSocketEventArgs();
            }
            else
            {
                _args_HandleDisconnect.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {
                handler(this, _args_HandleDisconnect);

                return _args_HandleDisconnect.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_HandleDisconnect);

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleDisconnect -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_HandleDisconnect.NextAction);
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleAsyncAction;
        async Task HandleAsyncAction()
        {
            var handler = OnHandleAsyncAction;

            if (handler != null)
            {
                if (_args_Uncaptured == null)
                {
                    _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
                }
                else
                {
                    _args_Uncaptured.RxBuffer = rxBuffer;
                    _args_Uncaptured.TxBuffer = txBuffer;
                    _args_Uncaptured.Count = null;
                    _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
                }

                bool sslStarted = this.SslStarted;

                await handler(this, _args_Uncaptured);

                if (SslStarted && (sslStarted != SslStarted))
                {
                    AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                }

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleAsyncAction -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                return;
            }

            throw new InvalidOperationException("HandleAsyncAction is not handled");
        }

        // todo cleanup
        /// <summary>
        /// allows descendent classes to override default nextaction after Transportsecurity is set by their descendents
        /// </summary>
        protected virtual void AfterHandleStartTransportSecurity(object sender, HandleSocketBEventArgs e)
        {
        }

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandleReceive;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleReceiveAsync;
        NextRequestedHandlerAction HandleReceive()
        {
            var handler = OnHandleReceive;
            var asyncHandler = OnHandleReceiveAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleReceive is not handled, you need to assign the eventhandler");
            }

            if (_args_Uncaptured == null)
            {
                _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
            }
            else
            {
                _args_Uncaptured.RxBuffer = rxBuffer;
                _args_Uncaptured.TxBuffer = txBuffer;
                _args_Uncaptured.Count = null;
                _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {
                handler(this, _args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleReceive -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleReceive -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                    (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandleSend;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleSendAsync;
        NextRequestedHandlerAction HandleSend()
        {
            var handler = OnHandleSend;
            var asyncHandler = OnHandleSendAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleSend is not handled, you need to assign the eventhandler");
            }

            if (handler != null)
            {
                if (_args_Uncaptured == null)
                {
                    _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
                }
                else
                {
                    _args_Uncaptured.RxBuffer = rxBuffer;
                    _args_Uncaptured.TxBuffer = txBuffer;
                    _args_Uncaptured.Count = null;
                    _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
                }
                handler(this, _args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleSend -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleSend -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                    (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleSocketBEventArgs> OnHandleStopTransportSecurity;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleSocketBEventArgs, Task> OnHandleStopTransportSecurityAsync;
        NextRequestedHandlerAction HandleStopTransportSecurity()
        {
            var handler = OnHandleStopTransportSecurity;
            var asyncHandler = OnHandleStopTransportSecurityAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleStopTransportSecurity is not handled, you need to assign the eventhandler");
            }

            StopSsl();

            if (_args_Uncaptured == null)
            {
                _args_Uncaptured = new HandleSocketBEventArgs(rxBuffer, txBuffer);
            }
            else
            {
                _args_Uncaptured.RxBuffer = rxBuffer;
                _args_Uncaptured.TxBuffer = txBuffer;
                _args_Uncaptured.Count = null;
                _args_Uncaptured.NextAction = NextRequestedHandlerAction.Dispose;
            }

            if (handler != null)
            {
                handler(this, _args_Uncaptured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleStopTransportSecurity -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                        (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Uncaptured);

                return _args_Uncaptured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Uncaptured);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_Uncaptured);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleStopTransportSecurity -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Uncaptured.NextAction) +
                    (_args_Uncaptured.Count.HasValue ? " (Count = " + _args_Uncaptured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Uncaptured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Uncaptured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<OnHandleExceptionEventArgs> OnHandleException;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]

        void OnException(Exception e)
        {
            try
            {
                var handler = OnHandleException;

                if (handler != null)
                {
                    if (_args_OnHandleException == null)
                    {
                        _args_OnHandleException = new OnHandleExceptionEventArgs(e);
                    }
                    else
                    {
                        _args_OnHandleException.Exception = e;
                    }

                    handler(this, _args_OnHandleException);
                }
            }
            catch { }
        }

        protected internal event EventHandler OnHandleReset;
        void HandleReset()
        {
            StopSsl();
            rxBuffer.Reset();
            txBuffer.Reset();
            SignalWaiteners.Clear();
            SignalSuppliers.Clear();
            var handler = OnHandleReset;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected internal event EventHandler<HandleCapturedSocketBEventArg> OnHandleCapturedSocketInitialize;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleCapturedSocketBEventArg, Task> OnHandleCapturedSocketInitializeAsync;
        NextRequestedCapturedHandlerAction HandleCapturedSocketInitialize(SocketAsyncEventArgs saeaCapture, DynamicBuffer rxBuffer, DynamicBuffer txBuffer)
        {
            SetActivityTimestamp();

            var handler = OnHandleCapturedSocketInitialize;
            var asyncHandler = OnHandleCapturedSocketInitializeAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleCapturedSocketInitialize is not handled, you need to assign the eventhandler");
            }

            if (_args_Captured == null)
            {
                _args_Captured = new HandleCapturedSocketBEventArg(saeaCapture, saeaCapture.Handler().ControllingHandler.SocketHasDataAvailable, rxBuffer, txBuffer);
            }
            else
            {
                _args_Captured.Count = null;
                _args_Captured.NextAction = NextRequestedCapturedHandlerAction.Release;
                _args_Captured.RxBuffer = rxBuffer;
                _args_Captured.TxBuffer = txBuffer;
                _args_Captured.saeaCapture = saeaCapture;
                _args_Captured.socketHasDataAvailable = saeaCapture.Handler().ControllingHandler.SocketHasDataAvailable;
            }

            if (handler != null)
            {
                handler(this, _args_Captured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleCapturedSocketInitialize -> " + Enum.GetName(typeof(NextRequestedCapturedHandlerAction), _args_Captured.NextAction) +
                        (_args_Captured.Count.HasValue ? " (Count = " + _args_Captured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Captured);

                return _args_Captured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Captured);


                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleCapturedSocketInitialize -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Captured.NextAction) +
                    (_args_Captured.Count.HasValue ? " (Count = " + _args_Captured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Captured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Captured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedCapturedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleCapturedSocketBEventArg> OnHandleCapturedSocketReceiveSuccess;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleCapturedSocketBEventArg, Task> OnHandleCapturedSocketReceiveSuccessAsync;
        NextRequestedCapturedHandlerAction HandleCapturedSocketReceiveSuccess(SocketAsyncEventArgs saeaCapture, DynamicBuffer rxBuffer, DynamicBuffer txBuffer)
        {
            var handler = OnHandleCapturedSocketReceiveSuccess;
            var asyncHandler = OnHandleCapturedSocketReceiveSuccessAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleCapturedSocketReceiveSuccess is not handled, you need to assign the eventhandler");
            }

            if (_args_Captured == null)
            {
                _args_Captured = new HandleCapturedSocketBEventArg(saeaCapture, saeaCapture.Handler().ControllingHandler.SocketHasDataAvailable, rxBuffer, txBuffer);
            }
            else
            {
                _args_Captured.Count = null;
                _args_Captured.NextAction = NextRequestedCapturedHandlerAction.Release;
                _args_Captured.RxBuffer = rxBuffer;
                _args_Captured.TxBuffer = txBuffer;
                _args_Captured.saeaCapture = saeaCapture;
                _args_Captured.socketHasDataAvailable = saeaCapture.Handler().ControllingHandler.SocketHasDataAvailable;
            }

            if (handler != null)
            {
                handler(this, _args_Captured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleCapturedSocketReceiveSuccess-> " + Enum.GetName(typeof(NextRequestedCapturedHandlerAction), _args_Captured.NextAction) +
                        (_args_Captured.Count.HasValue ? " (Count = " + _args_Captured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Captured);

                return _args_Captured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Captured);

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleCapturedSocketReceiveSuccess -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Captured.NextAction) +
                    (_args_Captured.Count.HasValue ? " (Count = " + _args_Captured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Captured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Captured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedCapturedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleCapturedSocketEventArgs> OnHandleCapturedSocketDisconnectSuccess;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleCapturedSocketEventArgs, Task> OnHandleCapturedSocketDisconnectSuccessAsync;
        NextRequestedCapturedHandlerAction HandleCapturedSocketDisconnectSuccess(SocketAsyncEventArgs saeaCapture)
        {
            var handler = OnHandleCapturedSocketDisconnectSuccess;
            var asyncHandler = OnHandleCapturedSocketDisconnectSuccessAsync;

            if (handler == null && asyncHandler == null)
            {
                return NextRequestedCapturedHandlerAction.DisposeCapturingHandler;
            }

            if (_args_CapturedDisconnect == null)
            {
                _args_CapturedDisconnect = new HandleCapturedSocketEventArgs(saeaCapture, false);
            }
            else
            {
                _args_CapturedDisconnect.NextAction = NextRequestedCapturedHandlerAction.Release;
                _args_CapturedDisconnect.saeaCapture = saeaCapture;
                _args_CapturedDisconnect.socketHasDataAvailable = false;
            }

            if (handler != null)
            {
                handler(this, _args_CapturedDisconnect);

                return _args_CapturedDisconnect.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_CapturedDisconnect);

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleCapturedSocketDisconnectSuccess";
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_CapturedDisconnect.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedCapturedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleCapturedSocketBEventArg> OnHandleCapturedSocketSendSuccess;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleCapturedSocketBEventArg, Task> OnHandleCapturedSocketSendSuccessAsync;
        NextRequestedCapturedHandlerAction HandleCapturedSocketSendSuccess(SocketAsyncEventArgs saeaCapture, DynamicBuffer rxBuffer, DynamicBuffer txBuffer)
        {
            var handler = OnHandleCapturedSocketSendSuccess;
            var asyncHandler = OnHandleCapturedSocketSendSuccessAsync;

            if (handler == null && asyncHandler == null)
            {
                throw new InvalidOperationException("OnHandleCapturedSocketSendSuccess is not handled, you need to assign the eventhandler");
            }

            if (_args_Captured == null)
            {
                _args_Captured = new HandleCapturedSocketBEventArg(saeaCapture, saeaCapture.Handler().ControllingHandler.SocketHasDataAvailable, rxBuffer, txBuffer);
            }
            else
            {
                _args_Captured.Count = null;
                _args_Captured.NextAction = NextRequestedCapturedHandlerAction.Release;
                _args_Captured.RxBuffer = rxBuffer;
                _args_Captured.TxBuffer = txBuffer;
                _args_Captured.saeaCapture = saeaCapture;
                _args_Captured.socketHasDataAvailable = saeaCapture.Handler().ControllingHandler.SocketHasDataAvailable;
            }

            if (handler != null)
            {
                handler(this, _args_Captured);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleCapturedSocketSendSuccess-> " + Enum.GetName(typeof(NextRequestedCapturedHandlerAction), _args_Captured.NextAction) +
                        (_args_Captured.Count.HasValue ? " (Count = " + _args_Captured.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                CheckArgumentsCountProperty(_args_Captured);

                return _args_Captured.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_Captured);

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                     "HandleCapturedSocketSendSuccess -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_Captured.NextAction) +
                    (_args_Captured.Count.HasValue ? " (Count = " + _args_Captured.Count.Value.ToString() + ")" : "");
                });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_Captured);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_Captured.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedCapturedHandlerAction.Handled;
        }

        protected internal event EventHandler<HandleCapturedSocketResetEventArgs> OnHandleCapturedSocketReset;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleCapturedSocketResetEventArgs, Task> OnHandleCapturedSocketResetAsync;
        internal NextRequestedHandlerAction HandleCapturedSocketReset(bool ResetWasRequested)
        {
            var handler = OnHandleCapturedSocketReset;
            var asyncHandler = OnHandleCapturedSocketResetAsync;

            if (handler == null && asyncHandler == null)
            {
                return NextRequestedHandlerAction.Dispose;
            }

            if (_args_HandleCapturedSocketReset == null)
            {
                _args_HandleCapturedSocketReset = new HandleCapturedSocketResetEventArgs(ResetWasRequested, rxBuffer, txBuffer);
            }
            else
            {
                _args_HandleCapturedSocketReset.Count = null;
                _args_HandleCapturedSocketReset.NextAction = NextRequestedHandlerAction.Dispose;
                _args_HandleCapturedSocketReset.RxBuffer = rxBuffer;
                _args_HandleCapturedSocketReset.TxBuffer = txBuffer;
                _args_HandleCapturedSocketReset.ResetWasRequested = ResetWasRequested;
            }

            if (handler != null)
            {
                handler(this, _args_HandleCapturedSocketReset);

                CheckArgumentsCountProperty(_args_HandleCapturedSocketReset);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                {
                    return this.GetType().Name + " | " +
                        "HandleCapturedSocketReset -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_HandleCapturedSocketReset.NextAction) +
                        (_args_HandleCapturedSocketReset.Count.HasValue ? " (Count = " + _args_HandleCapturedSocketReset.Count.Value.ToString() + ")" : "");
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                return _args_HandleCapturedSocketReset.NextAction;
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    if (Closing)
                    {
                        ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, NextRequestedHandlerAction.Dispose);
                        return;
                    }
                    await asyncHandler(this, _args_HandleCapturedSocketReset);

                    if (SslStarted && (sslStarted != SslStarted))
                    {
                        AfterHandleStartTransportSecurity(this, _args_HandleCapturedSocketReset);
                    }

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
#if (LogPolling)
#else
                    if (_args_Uncaptured.NextAction != NextRequestedHandlerAction.Poll)

#endif
                        Logger.LogSocketFlowMessage(ControllingHandler.saeaHandler, () =>
                        {
                            return this.GetType().Name + " | " +
                             "HandleCapturedSocketReset -> " + Enum.GetName(typeof(NextRequestedHandlerAction), _args_HandleCapturedSocketReset.NextAction) +
                            (_args_HandleCapturedSocketReset.Count.HasValue ? " (Count = " + _args_HandleCapturedSocketReset.Count.Value.ToString() + ")" : "");
                        });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    CheckArgumentsCountProperty(_args_HandleCapturedSocketReset);

                    ThreadPool.QueueUserWorkItem(PerformNextHandlerActionAsync, TranslateHandlerAction(_args_HandleCapturedSocketReset.NextAction));

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();

            return NextRequestedHandlerAction.Handled;
        }

        protected internal event EventHandler<MeasuredAmountEventArgs> OnMeasureBytesReceived;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, MeasuredAmountEventArgs, Task> OnMeasureBytesReceivedAsync;
        internal void TriggerOnMeasureBytesReceived(ulong amount)
        {
            var handler = OnMeasureBytesReceived;
            var asyncHandler = OnMeasureBytesReceivedAsync;

            if (handler == null && asyncHandler == null)
            {
                return;
            }

            if (_args_TriggerOnMeasureBytesReceived == null)
            {
                _args_TriggerOnMeasureBytesReceived = new MeasuredAmountEventArgs(amount);
            }
            else
            {
                _args_TriggerOnMeasureBytesReceived.Amount = amount;
            }

            if (handler != null)
            {
                handler(this, _args_TriggerOnMeasureBytesReceived);
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    await asyncHandler(this, _args_TriggerOnMeasureBytesReceived);
                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();
        }

        protected internal event EventHandler<MeasuredAmountEventArgs> OnMeasureBytesSend;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, MeasuredAmountEventArgs, Task> OnMeasureBytesSendAsync;
        internal void TriggerOnMeasureBytesSend(ulong amount)
        {
            var handler = OnMeasureBytesSend;
            var asyncHandler = OnMeasureBytesSendAsync;

            if (handler == null && asyncHandler == null)
            {
                return;
            }

            if (_args_TriggerOnMeasureBytesSend == null)
            {
                _args_TriggerOnMeasureBytesSend = new MeasuredAmountEventArgs(amount);
            }
            else
            {
                _args_TriggerOnMeasureBytesSend.Amount = amount;
            }

            if (handler != null)
            {
                handler(this, _args_TriggerOnMeasureBytesSend);
            }

            Task.Factory.StartNew(async delegate
            {
                try
                {
                    bool sslStarted = this.SslStarted;

                    await asyncHandler(this, _args_TriggerOnMeasureBytesSend);

                }
                catch (Exception e)
                {
                    _HandleException(e);

                    CloseAndReturnHandler();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();
        }

        protected internal event EventHandler<HandleTransportSecurityInitInfoEventArgs> OnTransportSecurityInitiatationCompleted;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleTransportSecurityInitInfoEventArgs, Task> OnTransportSecurityInitiatationCompletedAsync;
        internal void HandleTransportSecurityInitiatationCompleted()
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)

            Logger.LogHandlerFlowMessage(saeaHandler, () =>
            {

                var sb = new StringBuilder();

                sb.Append(SslConversionStream.IsAuthenticated ?
                    (SslConversionStream.IsServer ?
                      "Transportsecurity successfully established" + (SslConversionStream.IsEncrypted ? "" : " WITHOUT encryption") + ", as server, " + (SslConversionStream.IsMutuallyAuthenticated ? "both parties are" : "client is") + " authenticated using certificates. " :
                      "Transportsecurity successfully established" + (SslConversionStream.IsEncrypted ? "" : " WITHOUT encryption") + ", as client, " + (SslConversionStream.IsMutuallyAuthenticated ? "both parties are" : "server is") + " authenticated using certificates. "
                     ) :
                    (SslConversionStream.IsServer ?
                       "Transportsecurity successfully estblished" + (SslConversionStream.IsEncrypted ? "" : " WITHOUT encryption") + ", as server without certificate authentication." :
                       "Transportsecurity successfully estblished" + (SslConversionStream.IsEncrypted ? "" : " WITHOUT encryption") + ", as client without certificate authentication."
                     )
                );

                if (SslConversionStream.IsSigned)
                {
                    sb.Append("For tamper protection, data is being signed using a symmetric key algorithm. ");
                }

                sb.Append("A \"" +
                    Enum.GetName(typeof(System.Security.Authentication.SslProtocols), SslConversionStream.SslProtocol) +
                    "\" protocol is used using \"" +
                    Enum.GetName(typeof(CipherAlgorithmType), SslConversionStream.CipherAlgorithm) +
                    "\" cipher algorithm with a " + SslConversionStream.CipherStrength.ToString() + " bit cipher strength. "
                );

                var name = Enum.GetName(typeof(ExchangeAlgorithmType), SslConversionStream.KeyExchangeAlgorithm);
                if (!String.IsNullOrWhiteSpace(name))
                    sb.Append("Key and protocol detail exchange was done using a \"" + name +
                        "\" exchange algorithm, with a " + SslConversionStream.KeyExchangeStrength.ToString() + " bit key strength. "
                    );

                sb.Append("Using a \"" +
                    Enum.GetName(typeof(HashAlgorithmType), SslConversionStream.HashAlgorithm) + "\" " +
                    "hash algorithm, with a " + SslConversionStream.HashStrength.ToString() + " bit strength. "
                );

                if (SslConversionStream.CheckCertRevocationStatus)
                {
                    sb.Append("The remote certificate was checked for revocation, and was not revoked. ");
                }
                else
                {
                    sb.Append("The remote certificate was NOT checked for revocation. ");
                }

                if (SslConversionStream.RemoteCertificate != null)
                {
                    sb.Append("The remote \"" + SslConversionStream.RemoteCertificate.GetFormat() + "\" certificate was issued for " +

                        "\"" + SslConversionStream.RemoteCertificate.Subject + "\" by " +
                        "\"" + SslConversionStream.RemoteCertificate.Issuer + "\" " +
                        "on " + SslConversionStream.RemoteCertificate.GetEffectiveDateString().Trim() + " and " +
                        "will expire on " + SslConversionStream.RemoteCertificate.GetExpirationDateString() + " " +
                        " and uses a \"" + SslConversionStream.RemoteCertificate.GetKeyAlgorithm() + "\" key algorithm." +
                        " => " + SslConversionStream.RemoteCertificate.GetRawCertDataString()
                    );
                }
                return sb.ToString();
            });

#endif
            #endregion ------------------------------------------------------------------------------------------------

            var handler = OnTransportSecurityInitiatationCompleted;
            var asyncHandler = OnTransportSecurityInitiatationCompletedAsync;

            if (handler == null && asyncHandler == null)
            {
                return;
            }

            if (_args_HandleTransportSecurityInitiatationCompleted == null)
            {
                _args_HandleTransportSecurityInitiatationCompleted = new HandleTransportSecurityInitInfoEventArgs()
                {
                    CheckCertRevocationStatus = SslConversionStream.CheckCertRevocationStatus,
                    CipherAlgorithm = SslConversionStream.CipherAlgorithm,
                    CipherStrength = SslConversionStream.CipherStrength,
                    HashAlgorithm = SslConversionStream.HashAlgorithm,
                    HashStrength = SslConversionStream.HashStrength,
                    KeyExchangeAlgorithm = SslConversionStream.KeyExchangeAlgorithm,
                    KeyExchangeStrength = SslConversionStream.KeyExchangeStrength,
                    LocalCertificate = SslConversionStream.LocalCertificate,
                    RemoteCertificate = SslConversionStream.RemoteCertificate,
                    SslProtocol = SslConversionStream.SslProtocol,
                    IsAuthenticatedByCertificate = SslConversionStream.IsAuthenticated,
                    IsEncrypted = SslConversionStream.IsEncrypted,
                    IsMutuallyAuthenticated = SslConversionStream.IsMutuallyAuthenticated,
                    IsServer = SslConversionStream.IsServer,
                    IsSigned = SslConversionStream.IsSigned
                };
            }
            else
            {
                _args_HandleTransportSecurityInitiatationCompleted.CheckCertRevocationStatus = SslConversionStream.CheckCertRevocationStatus;
                _args_HandleTransportSecurityInitiatationCompleted.CipherAlgorithm = SslConversionStream.CipherAlgorithm;
                _args_HandleTransportSecurityInitiatationCompleted.CipherStrength = SslConversionStream.CipherStrength;
                _args_HandleTransportSecurityInitiatationCompleted.HashAlgorithm = SslConversionStream.HashAlgorithm;
                _args_HandleTransportSecurityInitiatationCompleted.HashStrength = SslConversionStream.HashStrength;
                _args_HandleTransportSecurityInitiatationCompleted.KeyExchangeAlgorithm = SslConversionStream.KeyExchangeAlgorithm;
                _args_HandleTransportSecurityInitiatationCompleted.KeyExchangeStrength = SslConversionStream.KeyExchangeStrength;
                _args_HandleTransportSecurityInitiatationCompleted.LocalCertificate = SslConversionStream.LocalCertificate;
                _args_HandleTransportSecurityInitiatationCompleted.RemoteCertificate = SslConversionStream.RemoteCertificate;
                _args_HandleTransportSecurityInitiatationCompleted.SslProtocol = SslConversionStream.SslProtocol;
                _args_HandleTransportSecurityInitiatationCompleted.IsAuthenticatedByCertificate = SslConversionStream.IsAuthenticated;
                _args_HandleTransportSecurityInitiatationCompleted.IsEncrypted = SslConversionStream.IsEncrypted;
                _args_HandleTransportSecurityInitiatationCompleted.IsMutuallyAuthenticated = SslConversionStream.IsMutuallyAuthenticated;
                _args_HandleTransportSecurityInitiatationCompleted.IsServer = SslConversionStream.IsServer;
                _args_HandleTransportSecurityInitiatationCompleted.IsSigned = SslConversionStream.IsSigned;
            }

            if (handler != null)
            {
                handler(this, _args_HandleTransportSecurityInitiatationCompleted);
            }
        }

        #endregion

        #region Misc
        void _HandleException(Exception e)
        {
            try
            {
                OnException(e);
            }
            catch { }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogException(e, this.GetType().Name + ": Exception occurred");
#endif
            #endregion ------------------------------------------------------------------------------------------------
        }

        private HandleSocketEventArgs _args_HandleDisconnect;
        private HandleSocketBEventArgs _args_Uncaptured;
        private HandleSocketMessageEventArgs _args_OnNewMessage;
        private OnHandleExceptionEventArgs _args_OnHandleException;
        private HandleCapturedSocketBEventArg _args_Captured;
        private HandleCapturedSocketResetEventArgs _args_HandleCapturedSocketReset;
        private MeasuredAmountEventArgs _args_TriggerOnMeasureBytesReceived;
        private MeasuredAmountEventArgs _args_TriggerOnMeasureBytesSend;
        private HandleTransportSecurityInitInfoEventArgs _args_HandleTransportSecurityInitiatationCompleted;
        private HandleCapturedSocketEventArgs _args_CapturedDisconnect;
        private long _UniqueSocketId;

        protected internal void SetReceiveBuffer(int? MaxLength = null)
        {
            var l = MaxLength.HasValue ? MaxLength.Value : MemoryConfiguration.ReceiveBufferSize;

            if ((saeaHandler.Buffer == null) || (saeaHandler.Buffer.Length < l))
            {
                saeaHandler.SetBuffer(new byte[l], 0, l);
            }
            else
            {
                saeaHandler.SetBuffer(0, l);
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () => { return "Setting receive size to " + saeaHandler.Count.ToString() + " bytes"; });
#endif
            #endregion ------------------------------------------------------------------------------------------------
        }

        public void SetActivityTimestamp(bool CheckAutoResetTimeoutsFlag = false)
        {
            if ((!CheckAutoResetTimeoutsFlag) || (AutoResetTimeouts))
            {
                Interlocked.Exchange(ref LastActivityTimeStamp, BitConverter.DoubleToInt64Bits(Timer.Duration));
            }
        }

        public virtual void SetCurrentStageTimeoutSeconds(double TimeoutSeconds, bool AutoResetTimeouts = true, double? BeginActivityTimestamp = null)
        {
            Interlocked.Exchange(ref CurrentStageTimeout, BitConverter.DoubleToInt64Bits(TimeoutSeconds));
            this.AutoResetTimeouts = AutoResetTimeouts;

            if (BeginActivityTimestamp.HasValue)
            {
                Interlocked.Exchange(ref LastActivityTimeStamp, BitConverter.DoubleToInt64Bits(BeginActivityTimestamp.Value));
            }
        }

        #endregion

        #region System.Net.Socket

        /// <param name="keepaliveTime">Specifies the timeout, in milliseconds, with no activity until the first keep-alive packet is sent.  The value must be greater than 0. If a value of less than or equal to zero is passed an ArgumentOutOfRangeException is thrown.</param>
        /// <param name="keepaliveInterval">Specifies the interval, in milliseconds, between when successive keep-alive packets are sent if no acknowledgement is received. The value must be greater than 0. If a value of less than or equal to zero is passed an ArgumentOutOfRangeException is thrown.</param>
        protected void SetTcpKeepAlive(uint keepaliveTime, uint keepaliveInterval)
        {
            SocketIO.SetTcpKeepAlive(saeaHandler, keepaliveTime, keepaliveInterval);
        }

        protected void SetReceiveBufferSize(int size)
        {
            SocketIO.SetReceiveBufferSize(saeaHandler, size);
        }

        protected void SetSendBufferSize(int size)
        {
            SocketIO.SetSendBufferSize(saeaHandler, size);
        }

        protected void SetBlocking(bool blocking)
        {
            SocketIO.SetBlocking(saeaHandler, blocking);
        }

        protected void SetNoDelay(bool nodelay)
        {
            SocketIO.SetNoDelay(saeaHandler, nodelay);
        }

        void CheckArgumentsCountProperty(HandleSocketBEventArgs eventArgs)
        {
            if (saeaHandler == null)
                return;

            switch (eventArgs.NextAction)
            {
                case NextRequestedHandlerAction.Receive:
                    if (eventArgs.Count.HasValue)
                        receiveCount = eventArgs.Count.Value;
                    else if (SslStarted)
                        receiveCount = 1;

                    if (!SslStarted)
                    {
                        SetReceiveBuffer();
                    }
                    break;
            }
        }

        void CheckArgumentsCountProperty(HandleCapturedSocketBEventArg eventArgs)
        {
            var handler = eventArgs.saeaCapture.Handler().ControllingHandler;
            var sslStarted = handler.SslStarted;

            switch (eventArgs.NextAction)
            {
                case NextRequestedCapturedHandlerAction.Receive:

                    if (eventArgs.Count.HasValue)
                        receiveCount = eventArgs.Count.Value;
                    else
                    {
                        if (sslStarted)
                        {
                            handler.receiveCount = 1;
                        }
                    }

                    if (!sslStarted)
                    {
                        handler.SetReceiveBuffer();
                    }

                    break;
            }
        }
        #endregion

        #region SocketHandlerAction enum

        /// <summary>
        /// Determines what next internal action is needed
        /// </summary>
        internal enum SocketHandlerActions
        {
            None,
            Initialize,
            Connect,
            AsyncAction,
            Receive,
            Send,
            StopTransportSecurity,
            Disconnect,
            Poll,
            WaitForSignal,
            WaitForMessage,
            ProcessReceivedMessage,
            Dispose,
            SetIdle,
            Handled,
            ReleaseControl,
            CaptureReleased,
            CaptureAccquired,
            CapturedReceive,
            CapturedSend,
            CapturedDisconnect,
            CaptureReleaseRequest,
            CaptureReleasedUnRequested,
            DisposeCapturingHandler
        };

        #endregion
    }

    #region Public Enums

    /// <summary>
    /// Determines the next asynchronous handler action that should be started
    /// </summary>
    public enum NextRequestedHandlerAction
    {
        Connect,
        AsyncAction,
        Receive,
        Send,
        StopTransportSecurity,
        Disconnect,
        WaitForSignal,
        Poll,
        WaitForMessage,
        Dispose,
        SetIdle,
        Handled,
        ReleaseControl
    };

    /// <summary>
    /// Determines the next asynchronous handler action that should be started
    /// </summary>
    public enum NextRequestedCapturedHandlerAction
    {
        Receive,
        Send,
        Disconnect,
        Release,
        DisposeCapturedHandler,
        DisposeCapturingHandler,
        Handled
    };

    #endregion
}
