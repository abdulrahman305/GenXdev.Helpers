using GenXdev.AsyncSockets.Handlers;
using Ninject;
using GenXdev.Buffers;

namespace GenXdev.AsyncSockets.Additional
{
    public class AsyncSocketHandlerTlsStreamAdapter : Stream
    {
        SocketHandlerBase SocketHandler;
        DynamicBuffer rxBuffer;
        IKernel Kernel;

        public AsyncSocketHandlerTlsStreamAdapter(
              IKernel Kernel
            , SocketHandlerBase SocketHandler
)
        {
            this.Kernel = Kernel;
            this.rxBuffer = Kernel.Get<DynamicBuffer>();
            SetHandler(SocketHandler);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.SocketHandler = null;

                    if (this.rxBuffer != null)
                    {
                        this.rxBuffer.Dispose();
                        this.rxBuffer = null;
                    }

                    if (asyncAction != null)
                    {
                        asyncAction.AsyncWaitHandle.Close();
                        asyncAction = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        Stack<AsyncResult> receiveStack = new Stack<AsyncResult>();
        Stack<AsyncResult> sendStack = new Stack<AsyncResult>();
        AsyncResult asyncAction = new AsyncResult()
        {
            AsyncWaitHandle = new ManualResetEvent(false)
        };


        internal bool HandleReceive()
        {
            if (Interlocked.Read(ref state) != 2)
                throw new InvalidOperationException();
            do
            {
                var received = SocketHandler.SocketIO.GetReceivedData(SocketHandler.saeaHandler, rxBuffer);
                if (received == 0)
                    throw new Exception("Connection reset by peer");
                asyncAction.Done -= received;
                asyncAction.Done += received;
                var isCompleted = rxBuffer.Count >= asyncAction.Needed;
                asyncAction.IsCompleted = isCompleted || (!SocketHandler.Active);

                if (isCompleted)
                {
                    var removed = rxBuffer.Remove(asyncAction.Buffer, asyncAction.Offset, asyncAction.Needed);

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
                    {
                        return "Received " +
                            received.ToString() +
                            " encrypted bytes into decryption buffer, we now have enough, needed " +
                            asyncAction.Needed + " bytes, starting decryption";
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    var callback = asyncAction.callback;

                    ((ManualResetEvent)asyncAction.AsyncWaitHandle).Set();

                    if (callback != null)
                    {
                        if (Interlocked.CompareExchange(ref state, 0, 2) == 2)
                        {
                            callback(asyncAction);
                        }
                    }

                    return true;
                }

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
                {
                    return "Received " +
                        received.ToString() +
                        " encrypted bytes into decryption buffer, still need " + (asyncAction.Needed - asyncAction.Done).ToString() +
                        " more, starting receive";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                SocketHandler.SetReceiveBuffer(SocketHandler.MemoryConfiguration.ReceiveBufferSize);

            } while (!SocketHandler.SocketIO.ReceiveAsync(SocketHandler.saeaHandler));

            return false;
        }

        internal bool HandleSend()
        {

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
            {
                return "Sent " +
                    SocketHandler.SocketIO.GetBytesTransfered(SocketHandler.saeaHandler).ToString() +
                    " encrypted bytes";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            if (Interlocked.Read(ref state) != 1)
                throw new InvalidOperationException();

            asyncAction.IsCompleted = true;

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
            {
                return "Sending of " +
                    asyncAction.Needed.ToString() +
                    " encrypted bytes completed successfully";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            var callback = asyncAction.callback;

            ((ManualResetEvent)asyncAction.AsyncWaitHandle).Set();

            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem((state2) =>
                {
                    if (Interlocked.CompareExchange(ref state, 0, 1) == 1)
                    {
                        callback(asyncAction);
                    }
                });
            }

            return true;
        }

        IAsyncResult GetEmptyAsyncResult(int count, AsyncCallback callback, object asyncState)
        {
            AsyncResult asyncAction = new AsyncResult()
            {
                AsyncWaitHandle = new ManualResetEvent(true),
                AsyncState = asyncState,
                CompletedSynchronously = true,
                Needed = count,
                IsCompleted = true,
            };

            return asyncAction;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (Interlocked.CompareExchange(ref this.state, 2, 0) != 0)
                throw new InvalidOperationException();

            if (rxBuffer.Count >= count)
            {
                rxBuffer.Remove(buffer, offset, count);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
                {
                    return "Ssl requires " +
                        count.ToString() +
                        " encrypted bytes to decode, already available in receivebuffer, starting decryption";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                return GetEmptyAsyncResult(count, callback, state);
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
            {
                return "Ssl requires " +
                    count.ToString() +
                    " encrypted bytes to decode, starting receive";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SocketHandler.SetReceiveBuffer(SocketHandler.MemoryConfiguration.ReceiveBufferSize);

            ((ManualResetEvent)asyncAction.AsyncWaitHandle).Reset();
            asyncAction.AsyncState = state;
            asyncAction.CompletedSynchronously = false;
            asyncAction.Done = 0;
            asyncAction.Needed = count;
            asyncAction.Buffer = buffer;
            asyncAction.Offset = offset;
            asyncAction.IsCompleted = false;
            asyncAction.callback = callback;

            var completedSynchronously = !SocketHandler.SocketIO.ReceiveAsync(SocketHandler.saeaHandler);

            if (completedSynchronously)
            {
                asyncAction.CompletedSynchronously = HandleReceive();
            }

            return asyncAction;
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            var needed = ((AsyncResult)asyncResult).Needed;

            if (Interlocked.CompareExchange(ref state, 0, 2) == 2)
            {
                asyncResult.AsyncWaitHandle.WaitOne(Timeout.Infinite, true);
            }

            return needed;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (Interlocked.CompareExchange(ref this.state, 1, 0) != 0)
                throw new InvalidOperationException();

            // nothing to do?
            if (count <= 0)
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () => { return "Zero bytes encrypted"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                return GetEmptyAsyncResult(count, callback, state);
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            SocketHandler.Logger.LogSocketFlowMessage(SocketHandler.saeaHandler, () =>
            {
                return "Starting " +
                    "to send " + count.ToString() + " encrypted bytes ";
            }
            );
#endif
            #endregion ------------------------------------------------------------------------------------------------

            ((ManualResetEvent)asyncAction.AsyncWaitHandle).Reset();
            asyncAction.AsyncState = state;
            asyncAction.CompletedSynchronously = false;
            asyncAction.Done = 0;
            asyncAction.Needed = count;
            asyncAction.Buffer = buffer;
            asyncAction.Offset = offset;
            asyncAction.IsCompleted = false;
            asyncAction.callback = callback;

            SocketHandler.saeaHandler.SetBuffer(buffer, offset, count);

            SocketHandler.SocketIO.SetReceiveBufferSize(SocketHandler.saeaHandler, SocketHandler.MemoryConfiguration.ReceiveBufferSize);

            var completedSynchronously = !SocketHandler.SocketIO.SendAsync(SocketHandler.saeaHandler);

            if (completedSynchronously)
            {
                asyncAction.CompletedSynchronously = HandleSend();
            }

            return asyncAction;
        }

        long state = 0;

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (Interlocked.CompareExchange(ref state, 0, 1) != 0)
                return;

            asyncResult.AsyncWaitHandle.WaitOne(Timeout.Infinite, true);
        }

        #region Stream

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get
            {
                return rxBuffer.Count;
            }
        }

        public override long Position
        {
            get
            {
                return 0;
            }
            set
            {
                throw new InvalidOperationException("Stream does not support seeking");
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Stream does not support seeking");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Stream does not support setting the size property");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("Use async methods");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("Use async methods");
        }

        #endregion

        #region Helpers

        internal void Finalize(DynamicBuffer RxBuffer)
        {
            Interlocked.Exchange(ref this.state, 0);
            this.rxBuffer.MoveTo(RxBuffer);
        }

        internal void SetHandler(SocketHandlerBase NewHandler)
        {
            this.SocketHandler = NewHandler;
        }

        internal void Initialize()
        {
            SocketHandler.rxBuffer.MoveTo(this.rxBuffer);
            Interlocked.Exchange(ref this.state, 0);
        }

        #endregion

        public int NrOfBytesSocketHasAvailable
        {
            get
            {
                // the rx buffer is still encrypted, 
                // and there is no way of knowing 
                // how many exact decoded bytes are in there,
                // so 0 or 1
                return Math.Min(1, this.rxBuffer.Count);
            }
        }

        public bool DataAvailable
        {
            get
            {
                return this.rxBuffer.Count > 0;
            }
        }
    }
}
