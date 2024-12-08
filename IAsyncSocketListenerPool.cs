/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Containers
{
    public interface IAsyncSocketListenerPool : IDisposable
    {
        EventHandler<SocketAsyncEventArgs> OnListenerSocketIOCompleted { get; set; }

        bool Pop(out SocketAsyncEventArgs saea);

        void Push(SocketAsyncEventArgs saea);
    }
}
