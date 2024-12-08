namespace GenXdev.AsyncSockets.Enums
{
    public enum MpxServerSocketHandlerState
    {
        IdentifyingClientType = 0,

        ReceivingClientProtocolVersion = 1,
        ReceivingClientCheckResult = 2,
        ReceivingClientMinimumVersion = 3,

        IdentifyingClient = 4,

        Multiplexed = 6,

        RetreivingChannelRequested_MultiPlexed = 7,
        RetreivingChannelAdded_MultiPlexed = 9,
        RetreivingChannelRemoved_MultiPlexed = 10,
        RetreivingChannelYield_MultiPlexed = 11,

        Disconnecting = 8
    };
}
