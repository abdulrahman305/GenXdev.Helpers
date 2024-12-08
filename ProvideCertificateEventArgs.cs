using System.Security.Cryptography.X509Certificates;

namespace GenXdev.AsyncSockets.Handlers
{
    public class ProvideCertificateEventArgs : EventArgs
    {
        public X509Certificate2 Result { get; set; }
    }
}