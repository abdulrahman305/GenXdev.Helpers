using GenXdev.AsyncSockets.Containers;
using GenXdev.Buffers;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleSocketBEventArgs : HandleSocketEventArgs, ISocketEventArgsB
    {
        public DynamicBuffer RxBuffer { get; internal set; }

        public DynamicBuffer TxBuffer { get; internal set; }

        public int? Count;

        public HandleSocketBEventArgs(DynamicBuffer RxBuffer, DynamicBuffer TxBuffer)
        {
            this.RxBuffer = RxBuffer;
            this.TxBuffer = TxBuffer;
        }
    }
}
