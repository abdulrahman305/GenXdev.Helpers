/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.AsyncSockets.Arguments;
using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Handlers
{
    public interface IWebResourceAssignmentHandler
    {
        void AssignWebResourceHandler(HttpRequestSocketHandler HttpHandler, HandleSocketBEventArgs e, SocketAsyncEventArgs saea);
    }
}
