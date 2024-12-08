using GenXdev.Buffers;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleSocketMessageEventArgs : HandleSocketBEventArgs
    {
        public object Message { get; internal set; }
        public HandleSocketMessageEventArgs(DynamicBuffer RxBuffer, DynamicBuffer TxBuffer, object Message)
            : base(RxBuffer, TxBuffer)
        {
            this.Message = Message;
        }
    }

}
