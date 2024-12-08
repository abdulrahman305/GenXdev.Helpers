namespace GenXdev.AsyncSockets.Enums
{
    public enum MpxClientSocketHandlerState
    {
        IdentifyingServerType = 0,

        ReceivingServerProtocolVersion = 1,
        ReceivingMinimumRequiredServerVersion = 2,

        ReceivingServerAuthenticationResult = 3,

        Multiplexed = 5,

        RetreivingChannelRequested_MultiPlexed = 6,
        RetreivingChannelAdded_MultiPlexed = 8,
        RetreivingChannelRemoved_MultiPlexed = 9,
        RetreivingChannelYield_MultiPlexed = 10,

        Disconnecting = 7,
    };
}
