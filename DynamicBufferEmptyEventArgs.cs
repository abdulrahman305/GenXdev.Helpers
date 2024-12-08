namespace GenXdev.Buffers
{
    public class DynamicBufferEmptyEventArgs : EventArgs
    {
        public DynamicBuffer Buffer { get; private set; }

        public DynamicBufferEmptyEventArgs(DynamicBuffer Buffer)
        {
            this.Buffer = Buffer;
        }
    }
}
