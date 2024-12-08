using GenXdev.AsyncSockets.Enums;

namespace GenXdev.AsyncSockets.Handlers
{
    public class MpxClientSocketNewStateEventArgs : EventArgs
    {
        public MpxClientSocketHandlerState NewState
        {
            get;
            internal set;
        }
    }
}
