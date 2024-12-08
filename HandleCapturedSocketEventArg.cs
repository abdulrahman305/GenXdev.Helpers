using GenXdev.AsyncSockets.Handlers;
using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleCapturedSocketEventArgs : EventArgs
    {
        public SocketAsyncEventArgs saeaCapture { get; internal set; }
        public bool socketHasDataAvailable { get; internal set; }

        public NextRequestedCapturedHandlerAction NextAction { get; set; }

        public HandleCapturedSocketEventArgs(SocketAsyncEventArgs saeaHandler, bool socketHasDataAvailable)
        {
            this.saeaCapture = saeaHandler;
            this.socketHasDataAvailable = socketHasDataAvailable;
            NextAction = NextRequestedCapturedHandlerAction.DisposeCapturingHandler;
        }
    }
}
