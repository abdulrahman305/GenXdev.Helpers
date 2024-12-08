using GenXdev.AsyncSockets.Containers;
using GenXdev.Buffers;
using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleCapturedSocketBEventArg : HandleCapturedSocketEventArgs, ISocketEventArgsB
    {
        public DynamicBuffer RxBuffer { get; internal set; }

        public DynamicBuffer TxBuffer { get; internal set; }

        public int? Count;

        public HandleCapturedSocketBEventArg(SocketAsyncEventArgs saeaHandler, bool socketHasDataAvailable, DynamicBuffer RxBuffer, DynamicBuffer TxBuffer)
            : base(saeaHandler, socketHasDataAvailable)
        {

            this.RxBuffer = RxBuffer;
            this.TxBuffer = TxBuffer;
        }
    }
}
