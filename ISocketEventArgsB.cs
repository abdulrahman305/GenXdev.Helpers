using GenXdev.Buffers;

namespace GenXdev.AsyncSockets.Containers
{
    public interface ISocketEventArgsB
    {
        DynamicBuffer RxBuffer { get; }

        DynamicBuffer TxBuffer { get; }

    }
}
