using GenXdev.Buffers;

namespace GenXdev.AsyncSockets.Arguments
{
    public class HandleCapturedSocketResetEventArgs : HandleSocketBEventArgs
    {
        public bool ResetWasRequested { get; internal set; }

        public HandleCapturedSocketResetEventArgs(bool ResetWasRequested, DynamicBuffer RxBuffer, DynamicBuffer TxBuffer) :
            base(RxBuffer, TxBuffer)
        {
            this.ResetWasRequested = ResetWasRequested;
        }
    }
}
