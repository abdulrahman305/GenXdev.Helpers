/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */

namespace GenXdev.AsyncSockets.Configuration
{
    public interface IHttpSocketHandlerConfiguration : ISocketHandlerConfiguration
    {
        /// <summary>
        /// http gzip compression support
        /// </summary>
        bool EnableHttpGzipCompressionSupport { get; }

        /// <summary>
        /// http deflate compression support 
        /// </summary>
        bool EnableHttpDeflateCompressionSupport { get; }

        /// <summary>
        /// http(s) proxy gzip compression level
        /// </summary>
        Ionic.Zlib.CompressionLevel GzipCompressionLevel { get; }

        /// <summary>
        /// http(s) proxy deflate compression level
        /// </summary>
        Ionic.Zlib.CompressionLevel DeflateCompressionLevel { get; }

        /// <summary>
        /// max request header size
        /// </summary>
        int MaxIncomingRequestHeaderSize { get; }

        /// <summary>
        /// the maximum time for for the request headers to be received
        /// </summary>
        double HttpReadingHeadersTimeoutSeconds { get; }

        /// <summary>
        /// how long a keep-alife http socket can be IDLE
        /// </summary>
        double HttpKeepAliveTimeoutSeconds { get; }        
    }
}
