namespace GenXdev.AsyncSockets.Arguments
{
    public class OnClientSetChannelInfoEventArgs : EventArgs
    {
        public string[] ChannelSubscriptions { get; internal set; }
    }
}
