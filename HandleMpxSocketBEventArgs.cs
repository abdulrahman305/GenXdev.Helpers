using GenXdev.AsyncSockets.Containers;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleMpxSocketBEventArgs<ChannelDataType> : EventArgs where ChannelDataType : MPXChannelData
    {
        public ChannelDataType Data { get; internal set; }

        public string Channel { get; internal set; }

    }
}
