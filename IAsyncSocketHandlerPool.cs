/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.AsyncSockets.Handlers;

namespace GenXdev.AsyncSockets.Containers
{
    public class HandlerClosedEventArgs : EventArgs
    {
        public SocketHandlerBase handler { get; private set; }

        public HandlerClosedEventArgs(SocketHandlerBase handler)
        {
            this.handler = handler;
        }
    }
    public interface IAsyncSocketHandlerPool : IDisposable
    {
        bool Pop(out SocketHandlerBase handler);
        void Push(SocketHandlerBase handler);
        void StopAllActiveConnections();
        void Stop();
        void Start();

        EventHandler<HandlerClosedEventArgs> RemoveProtocolHandlerCallback { get; set; }
    }
}
