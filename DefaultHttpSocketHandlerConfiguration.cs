/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.Configuration;
using Ionic.Zlib;

namespace GenXdev.AsyncSockets.Configuration
{
    public class DefaultHttpSocketHandlerConfiguration : ConfigurationBase, IHttpSocketHandlerConfiguration
    {
        private CompressionLevel gzipCompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

        /// <summary>
        /// http gzip compression support
        /// </summary>
        public bool EnableHttpGzipCompressionSupport { get; protected set; } = true;

        /// <summary>
        /// http deflate compression support 
        /// </summary>
        public bool EnableHttpDeflateCompressionSupport { get; protected set; } = true;

        /// <summary>
        /// http(s) proxy gzip compression level
        /// </summary>
        public Ionic.Zlib.CompressionLevel GzipCompressionLevel { get => gzipCompressionLevel; protected set => gzipCompressionLevel = value; }
        /// <summary>
        /// http(s) proxy deflate compression level
        /// </summary>
        public Ionic.Zlib.CompressionLevel DeflateCompressionLevel  { get; protected set; } = Ionic.Zlib.CompressionLevel.BestCompression;

        /// <summary>
        /// max request header size
        /// </summary>
        public int MaxIncomingRequestHeaderSize  { get; protected set; } = 4096;

        /// <summary>
        /// maximum time for for the request headers to be received
        /// </summary>
        public double HttpReadingHeadersTimeoutSeconds  { get; protected set; } = 20;

        /// <summary>
        /// how long a keep-alife http socket can be IDLE
        /// </summary>
        public double HttpKeepAliveTimeoutSeconds  { get; protected set; } = 60;

        /// <summary>
        /// TCP/IP Keepalive (if disabled, falls back to windows default = 2 hours)
        /// </summary>
        public bool EnableSocketKeepAlives  { get; protected set; } = false;

        /// <summary>
        /// TCP/IP Keepalive timeout in miliseconds
        /// </summary>
        public uint SocketKeepAliveTimeoutMiliSeconds  { get; protected set; } = 70000;

        /// <summary>
        /// TCP/IP Keepalive probe interval in miliseconds
        /// </summary>
        public uint SocketKeepAliveProbeIntervalMiliSeconds  { get; protected set; } = 60000;        
    }
}
