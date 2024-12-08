namespace GenXdev.Buffers
{
    public class DynamicBufferAddedEventArgs : EventArgs
    {
        public DynamicBuffer Buffer { get; private set; }

        public int Count { get; private set; }
        public int TotalAvailable { get; private set; }

        public DynamicBufferAddedEventArgs(DynamicBuffer Buffer, int Count)
        {
            this.Buffer = Buffer;
            this.Count = Count;
            this.TotalAvailable = Buffer.Count;
        }
    }
}
