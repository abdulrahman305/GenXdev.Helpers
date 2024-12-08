namespace GenXdev.AsyncSockets.Enums
{
    public enum MpxSocketHandlerChannelState
    {
        Idle = 0,
        InitializingIncomingTransfer = 1,
        InitializingOutgoingTransfer = 2,
        Resume = 3,
        Start = 4,
        Done = 5,
        Skip = 6,
        ReceivingTransfer = 7,
        SendingCompressedTransfer = 8,
        SendingUnCompressedTransfer = 9,
        ConfirmingIncomingTransfer = 10,
        ConfirmingOutgoingTransfer = 11
    };
}
