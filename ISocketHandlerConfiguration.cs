/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */

namespace GenXdev.AsyncSockets.Configuration
{
    public interface ISocketHandlerConfiguration
    {
        // TCP/IP Keepalive (if disabled, falls back to windows default = 2 hours)
        bool EnableSocketKeepAlives { get; }

        // TCP/IP Keepalive timeout in miliseconds
        uint SocketKeepAliveTimeoutMiliSeconds { get; }

        // TCP/IP Keepalive probe interval in miliseconds
        uint SocketKeepAliveProbeIntervalMiliSeconds { get; }
    }
}
