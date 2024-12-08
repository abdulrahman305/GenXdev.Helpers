/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Security.Authentication;
using GenXdev.Configuration;
using GenXdev.Helpers;
using System.Net;

namespace GenXdev.AsyncSockets.Configuration
{
    public class DefaultTLSConfiguration : ConfigurationBase, ISocketHandlerTLSConfiguration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DefaultTLSConfiguration(string filePath = null)
        {
            if (String.IsNullOrWhiteSpace(_TLS_Default_CACertificate_AuthorityName))
                _TLS_Default_CACertificate_AuthorityName =
                    GenXdev.Helpers.Environment.GetAssemblyCompanyName(this.GetType().Assembly);

            filePath = !String.IsNullOrWhiteSpace(filePath) && File.Exists(filePath) ? filePath :
                Path.Combine(
                    GenXdev.Helpers.Environment.GetApplicationRootDirectory(),
                    this.GetType().Name + ".cfg"
                );

            LoadConfigurationFromDisk(filePath);
        }

        object _SyncRoot = new object();
        public object SyncRoot { get { return _SyncRoot; } }

        /// <summary>
        /// Provides a uniquename by which certificates using this configuration will be cached
        /// </summary>
        public string UniqueConfigurationName
        {
            get
            {
                return GetUniqueConfigurationName();
            }

            set
            {
                SetUniqueConfigurationName(value);
            }
        }

        protected virtual void SetUniqueConfigurationName(string value)
        {
            lock (_SyncRoot)
                _UniqueConfigurationName = value;
        }

        protected virtual string GetUniqueConfigurationName()
        {
            lock (_SyncRoot)
                return _UniqueConfigurationName;
        }

        string _UniqueConfigurationName = String.Empty;

        /// <summary>
        /// Determines when to start TLS
        /// </summary>
        public SocketHandlerTLSActivationOptions TLS_ActivationOptions
        {
            get
            {
                return GetTLS_ActivationOptions();
            }

            set
            {
                SetTLS_ActivationOptons(value);
            }
        }

        protected virtual void SetTLS_ActivationOptons(SocketHandlerTLSActivationOptions value)
        {
            lock (_SyncRoot)
                _TLS_ActivationOptions = value;
        }

        protected virtual SocketHandlerTLSActivationOptions GetTLS_ActivationOptions()
        {
            lock (_SyncRoot)
                return _TLS_ActivationOptions;
        }

        SocketHandlerTLSActivationOptions _TLS_ActivationOptions = SocketHandlerTLSActivationOptions.TLSAutoDetect;

        /// <summary>
        /// Determines how to obtain the server certificate for TLS usage
        /// </summary>
        public SocketHandlerTLSCertificateUsage TLS_CertificateUsage
        {
            get
            {
                return GetTLS_CertificateUsage();
            }

            set
            {
                SetTLS_CertificateUsage(value);
            }
        }

        protected virtual void SetTLS_CertificateUsage(SocketHandlerTLSCertificateUsage value)
        {
            lock (_SyncRoot)
                _TLS_CertificateUsage = value;
        }

        protected virtual SocketHandlerTLSCertificateUsage GetTLS_CertificateUsage()
        {
            lock (_SyncRoot)
                return _TLS_CertificateUsage;
        }

        SocketHandlerTLSCertificateUsage _TLS_CertificateUsage = SocketHandlerTLSCertificateUsage.AutoGenerate;

        /// <summary>
        /// Which SSL/TLS protocols to support
        /// </summary>
        public SslProtocols TLS_EnabledProtocols
        {
            get
            {
                return GetTLS_EnabledProtocols();
            }

            set
            {
                SetTLS_EnabledProtocols(value);
            }
        }

        protected virtual void SetTLS_EnabledProtocols(SslProtocols value)
        {
            lock (_SyncRoot)
                _TLS_EnabledProtocols = value;
        }

        protected virtual SslProtocols GetTLS_EnabledProtocols()
        {
            lock (_SyncRoot)
                return _TLS_EnabledProtocols;
        }

        SslProtocols _TLS_EnabledProtocols = SslProtocols.Tls12;

        /// <summary>
        /// The list of hostnames for auto generated (if enabled) server certificate
        /// </summary>
        public string[] TLS_AdditionalHostNames
        {
            get
            {
                return GetTLS_AdditionalHostNames();
            }

            set
            {
                SetTLS_AdditionalHostNames(value);
            }
        }

        protected virtual void SetTLS_AdditionalHostNames(string[] value)
        {
            lock (_SyncRoot)
            {
                if (value != null)
                {
                    value = (from q in value where !String.IsNullOrWhiteSpace(q) select q.Trim().ToLower()).Distinct<string>().ToArray<String>();
                }

                _TLS_AdditionalHostNames = value;
            }
        }

        protected virtual string[] GetTLS_AdditionalHostNames()
        {
            lock (_SyncRoot)
            {
                if (_TLS_AdditionalHostNames == null)
                {
                    return new string[1] { GenXdev.Helpers.Network.GetPublicExternalHostname(null) };
                }

                return _TLS_AdditionalHostNames;
            }
        }

        string[] _TLS_AdditionalHostNames = null;

        /// <summary>
        /// The full PFX filepath to the load/store the certificate-authority certificate from
        /// This file must contain both public and private keys
        /// </summary>
        public string TLS_CACertificate_PfxFilename
        {
            get
            {
                return GetTLS_CACertificate_PfxFilename();
            }

            set
            {
                SetTLS_CACertificate_PfxFilename(value);
            }
        }

        protected virtual void SetTLS_CACertificate_PfxFilename(string value)
        {
            lock (_SyncRoot)
                _TLS_CACertificate_PfxFilename = value;
        }

        protected virtual string GetTLS_CACertificate_PfxFilename()
        {
            lock (_SyncRoot)
                return _TLS_CACertificate_PfxFilename;
        }

        string _TLS_CACertificate_PfxFilename = null;

        /// <summary>
        /// The password for the PFX file, or null if none
        /// </summary>
        public string TLS_CACertificate_Password
        {
            get
            {
                return GetTLS_CACertificate_Password();
            }

            set
            {
                SetTLS_CACertificate_Password(value);
            }
        }

        protected virtual void SetTLS_CACertificate_Password(string value)
        {
            lock (_SyncRoot)
                _TLS_CACertificate_Password = value;
        }

        protected virtual string GetTLS_CACertificate_Password()
        {
            lock (_SyncRoot)
                return _TLS_CACertificate_Password;
        }

        string _TLS_CACertificate_Password = null;

        /// <summary>
        /// The organisation name for the autogenerated (if enabled) CA Certificate
        /// </summary>
        public string TLS_Default_CACertificate_AuthorityName
        {
            get
            {
                return GetTLS_Default_CACertificate_AuthorityName();
            }

            set
            {
                SetTLS_Default_CACertificate_AuthorityName(value);
            }
        }

        protected virtual void SetTLS_Default_CACertificate_AuthorityName(string value)
        {
            lock (_SyncRoot)
                _TLS_Default_CACertificate_AuthorityName = value;
        }

        protected virtual string GetTLS_Default_CACertificate_AuthorityName()
        {
            lock (_SyncRoot)
                return _TLS_Default_CACertificate_AuthorityName;
        }

        string _TLS_Default_CACertificate_AuthorityName;

        /// <summary>
        /// The full PFX filepath to the load/store the server certificate from
        /// This file must contain both public and private keys
        /// </summary>
        public string TLS_ServerCertificate_PfxFilename
        {
            get
            {
                return GetTLS_ServerCertificate_PfxFilename();
            }

            set
            {
                SetTLS_ServerCertificate_PfxFilename(value);
            }
        }

        protected virtual void SetTLS_ServerCertificate_PfxFilename(string value)
        {
            lock (_SyncRoot)
                _TLS_ServerCertificate_PfxFilename = value;
        }

        protected virtual string GetTLS_ServerCertificate_PfxFilename()
        {
            lock (_SyncRoot)
                return _TLS_ServerCertificate_PfxFilename;
        }

        string _TLS_ServerCertificate_PfxFilename = null;

        /// <summary>
        /// The password for the PFX file, or null if none
        /// </summary>
        public string TLS_ServerCertificate_Password
        {
            get
            {
                return GetTLS_ServerCertificate_Password();
            }

            set
            {
                SetTLS_ServerCertificate_Password(value);
            }
        }

        protected virtual void SetTLS_ServerCertificate_Password(string value)
        {
            lock (_SyncRoot)
                _TLS_ServerCertificate_Password = value;
        }

        protected virtual string GetTLS_ServerCertificate_Password()
        {
            lock (_SyncRoot)
                return _TLS_ServerCertificate_Password;
        }

        string _TLS_ServerCertificate_Password = null;
    }
}
