using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace GenXdev.Additional
{

    // Summary:
    //     Provides asynchronous wrappers for .NET Framework operations.
    public static class AsyncExtensions
    {
        public static Task AuthenticateAsServerAsync(this SslStream sslstream, X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
        {
            return Task.Factory.FromAsync(
                sslstream.BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation, asyncCallback, asyncState),
                sslstream.EndAuthenticateAsServer
            );
        }

        public static Task AuthenticateAsClientAsync(this SslStream sslstream, string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
        {
            return Task.Factory.FromAsync(
                sslstream.BeginAuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, asyncCallback, asyncState),
                sslstream.EndAuthenticateAsClient
            );
        }

        public static Task<IPHostEntry> GetHostEntryAsync(string hostName, AsyncCallback asyncCallback = null, object asyncState = null)
        {
            return Task.Factory.FromAsync<IPHostEntry>(
                Dns.BeginGetHostEntry(hostName, asyncCallback, asyncState),
                Dns.EndGetHostEntry
            );
        }
    }
}
