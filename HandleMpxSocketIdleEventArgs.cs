using GenXdev.AsyncSockets.Containers;

namespace GenXdev.AsyncSockets.Arguments
{
    public enum HandleMpxSocketIdleNextAction { Continue, AllowCapture };

    public class HandleMpxSocketIdleEventArgs<ChannelDataType> : EventArgs where ChannelDataType : MPXChannelData
    {
        public MPXChannelDataDictionary<ChannelDataType> Channels { get; internal set; }
        public HandleMpxSocketIdleNextAction NextAction { get; set; } = HandleMpxSocketIdleNextAction.Continue;
    }
}
