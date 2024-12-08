using GenXdev.AsyncSockets.Handlers;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleSocketEventArgs : EventArgs
    {
        public NextRequestedHandlerAction NextAction { get; set; }

        public HandleSocketEventArgs()
        {
            NextAction = NextRequestedHandlerAction.Dispose;
        }
    }
}
