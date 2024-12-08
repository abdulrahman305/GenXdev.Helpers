using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace GenXdev.AsyncSockets.Arguments
{
    /// <summary>
    /// Contains the transportsecurity information
    /// 
    /// Eventhandler should throw exception if not valid
    /// </summary>
    public class HandleTransportSecurityInitInfoEventArgs : EventArgs
    {
        //     true if the certificate revocation list is checked; otherwise, false.
        public bool CheckCertRevocationStatus { get; internal set; }
        //
        // Summary:
        //     Gets a value that identifies the bulk encryption algorithm used by this System.Net.Security.SslStream.
        //
        // Returns:
        //     A System.Security.Authentication.CipherAlgorithmType value.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Net.Security.SslStream.CipherAlgorithm property was accessed before
        //     the completion of the authentication process or the authentication process
        //     failed.
        public CipherAlgorithmType CipherAlgorithm { get; internal set; }
        //
        // Summary:
        //     Gets a value that identifies the strength of the cipher algorithm used by
        //     this System.Net.Security.SslStream.
        //
        // Returns:
        //     An System.Int32 value that specifies the strength of the algorithm, in bits.
        public int CipherStrength { get; internal set; }
        //
        // Summary:
        //     Gets the algorithm used for generating message authentication codes (MACs).
        //
        // Returns:
        //     A System.Security.Authentication.HashAlgorithmType value.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Net.Security.SslStream.HashAlgorithm property was accessed before
        //     the completion of the authentication process or the authentication process
        //     failed.
        public HashAlgorithmType HashAlgorithm { get; internal set; }
        //
        // Summary:
        //     Gets a value that identifies the strength of the hash algorithm used by this
        //     instance.
        //
        // Returns:
        //     An System.Int32 value that specifies the strength of the System.Security.Authentication.HashAlgorithmType
        //     algorithm, in bits. Valid values are 128 or 160.
        public int HashStrength { get; internal set; }
        //
        // Summary:
        //     Gets a System.Boolean value that indicates whether authentication was successful.
        //
        // Returns:
        //     true if successful authentication occurred; otherwise, false.
        public ExchangeAlgorithmType KeyExchangeAlgorithm { get; internal set; }
        //
        // Summary:
        //     Gets a value that identifies the strength of the key exchange algorithm used
        //     by this instance.
        //
        // Returns:
        //     An System.Int32 value that specifies the strength of the System.Security.Authentication.ExchangeAlgorithmType
        //     algorithm, in bits.
        public int KeyExchangeStrength { get; internal set; }
        //
        // Summary:
        //     Gets the certificate used to authenticate the local endpoint.
        //
        // Returns:
        //     An X509Certificate object that represents the certificate supplied for authentication
        //     or null if no certificate was supplied.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     Authentication failed or has not occurred.
        public X509Certificate LocalCertificate { get; internal set; }

        //
        // Summary:
        //     Gets the certificate used to authenticate the remote endpoint.
        //
        // Returns:
        //     An X509Certificate object that represents the certificate supplied for authentication
        //     or null if no certificate was supplied.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     Authentication failed or has not occurred.
        public X509Certificate RemoteCertificate { get; internal set; }
        //
        // Summary:
        //     Gets a value that indicates the security protocol used to authenticate this
        //     connection.
        //
        // Returns:
        //     The System.Security.Authentication.SslProtocols value that represents the
        //     protocol used for authentication.
        public SslProtocols SslProtocol { get; internal set; }
        //
        // Summary:
        //     Gets a System.Boolean value that indicates whether data sent using this System.Net.Security.AuthenticatedStream
        //     is encrypted.
        //
        // Returns:
        //     true if data is encrypted before being transmitted over the network and decrypted
        //     when it reaches the remote endpoint; otherwise, false.
        public bool IsEncrypted { get; internal set; }

        //
        // Summary:
        //     Gets a System.Boolean value that indicates whether authentication by certificate was successful.
        //
        // Returns:
        //     true if successful authentication occurred; otherwise, false.
        public bool IsAuthenticatedByCertificate { get; internal set; }
        //
        // Summary:
        //     Gets a System.Boolean value that indicates whether both server and client
        //     have been authenticated.
        //
        // Returns:
        //     true if the client and server have been authenticated; otherwise, false.
        public bool IsMutuallyAuthenticated { get; internal set; }
        //
        // Summary:
        //     Gets a System.Boolean value that indicates whether the local side of the
        //     connection was authenticated as the server.
        //
        // Returns:
        //     true if the local endpoint was authenticated as the server side of a client-server
        //     authenticated connection; false if the local endpoint was authenticated as
        //     the client.
        public bool IsServer { get; internal set; }
        //
        // Summary:
        //     Gets a System.Boolean value that indicates whether the data sent using this
        //     stream is signed.
        //
        // Returns:
        //     true if the data is signed before being transmitted; otherwise, false.
        public bool IsSigned { get; internal set; }
    }
}
