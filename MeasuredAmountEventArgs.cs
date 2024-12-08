namespace GenXdev.AsyncSockets.Arguments
{
    public class MeasuredAmountEventArgs : EventArgs
    {
        public ulong Amount { get; internal set; }

        public MeasuredAmountEventArgs(ulong amount)
        {
            this.Amount = amount;
        }
    }
}
