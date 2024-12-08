namespace GenXdev.AsyncSockets.Arguments
{
    public class OnHandleExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; internal set; }

        public OnHandleExceptionEventArgs(Exception e)
        {
            this.Exception = e;
        }
    }
}
