/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Runtime.InteropServices;
using GenXdev.AsyncSockets.Handlers;

namespace System.Net.Sockets
{
    static class SocketExtensions
    {
        public static void SetTcpKeepAlive(this Socket socket, uint keepaliveTime, uint keepaliveInterval)
        {
            /* the native structure
            struct tcp_keepalive {
            ULONG onoff;
            ULONG keepalivetime;
            ULONG keepaliveinterval;
            };
            */

            // marshal the equivalent of the native structure into a byte array
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(keepaliveTime)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)keepaliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)keepaliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            // write SIO_VALS to Socket IOControl
            socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }
    }

    static class SocketAsyncEventArgsExtensions
    {
        public static SocketHandlerBase Handler(this SocketAsyncEventArgs saea)
        {
            if (saea == null)
                return null;
            lock (saea)
            {
                return (SocketHandlerBase)saea.UserToken;
            }
        }
        public static void SetHandler(this SocketAsyncEventArgs saea, SocketHandlerBase Handler)
        {
            if (saea == null)
                return;
            lock (saea)
            {
                saea.UserToken = Handler;
            }
        }
    }
}
