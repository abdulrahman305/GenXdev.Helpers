using GenXdev.AsyncSockets.Enums;

namespace GenXdev.AsyncSockets.Handlers
{
    public class MpxServerSocketNewStateEventArgs : EventArgs
    {
        public MpxServerSocketHandlerState NewState { get; internal set; }
    }
}
