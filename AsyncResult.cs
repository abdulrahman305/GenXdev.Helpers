namespace GenXdev.AsyncSockets.Additional
{
    public class AsyncResult : IAsyncResult, IDisposable
    {
        public object AsyncState { get; set; }

        public System.Threading.WaitHandle AsyncWaitHandle { get; set; }
        public bool CompletedSynchronously { get; set; }
        public bool IsCompleted { get; set; }

        public int Done { get; set; }
        public int Needed { get; set; }
        internal AsyncCallback callback;
        public byte[] Buffer;
        public int Offset;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (AsyncWaitHandle != null)
                {
                    var mre = AsyncWaitHandle as ManualResetEvent;
                    if (mre != null)
                    {
                        mre.Set();
                    }

                    AsyncWaitHandle.Dispose();
                    AsyncWaitHandle = null;
                }
            }
        }

        ~AsyncResult()
        {
            Dispose(false);
        }
    }
}
