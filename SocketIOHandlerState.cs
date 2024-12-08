namespace GenXdev.AsyncSockets.Containers
{
    public enum SocketIOHandlerState
    {
        Idle = 0,
        Connecting = 1,
        Connected = 2,
        Sending = 3,
        SendingIdle = 4,
        Receiving = 5,
        ReceivingIdle = 6,
        ReceivingBlocking = 7,
        Disconnecting = 8,
        Disposed = 9
    }
}
