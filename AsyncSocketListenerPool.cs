/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Net.Sockets;
using Ninject;
using System.Collections.Concurrent;
using GenXdev.AsyncSockets.Configuration;

namespace GenXdev.AsyncSockets.Containers
{
    public class AsyncSocketListenerPool : IAsyncSocketListenerPool
    {
        #region Initialization

        IKernel Kernel;
        ISocketListenerConfiguration ListenerConfiguration;

        [Inject]
        public AsyncSocketListenerPool(
            IKernel Kernel,
            ISocketListenerConfiguration ListenerConfiguration
        )
        {
            this.Kernel = Kernel;
            this.ListenerConfiguration = ListenerConfiguration;

            Initialize();
        }

        public EventHandler<SocketAsyncEventArgs> OnListenerSocketIOCompleted { get; set; }

        void OnHandlerIOCompleted(object sender, SocketAsyncEventArgs saea)
        {
            if (OnListenerSocketIOCompleted != null) OnListenerSocketIOCompleted(sender, saea);
        }

        void Initialize()
        {
            SocketQueue = new ConcurrentQueue<SocketAsyncEventArgs>();

            // fill to the max
            for (var i = 0; i < ListenerConfiguration.MaxConnections; i++)
            {
                // create new saea
                var saea = new SocketAsyncEventArgs();

                // enSocketQueue
                SocketQueue.Enqueue(saea);

                // assign event handler
                saea.Completed += new EventHandler<SocketAsyncEventArgs>(OnHandlerIOCompleted);
            }
        }

        protected bool Disposing;
        protected virtual void Dispose(bool disposing)
        {
            Disposing = true;

            if (disposing)
            {
                if (SocketQueue == null) return;

                StopAllActiveConnections();


                // empty SocketQueue
                while (SocketQueue.TryDequeue(out SocketAsyncEventArgs saea))
                {
                    DisposeHandler(saea);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~AsyncSocketListenerPool()
        {
            Dispose(false);
        }

        private void DisposeHandler(SocketAsyncEventArgs saea)
        {
            // dispose object
            saea.Dispose();
        }

        protected void StopAllActiveConnections()
        {
            if (SocketQueue != null)
            {
                lock (ActiveSockets)
                {
                    foreach (var activeSocket in ActiveSockets)
                    {
                        try
                        {
                            if ((activeSocket != null) && (activeSocket.Handler() != null))
                            {
                                activeSocket.Handler().Close();
                            }
                        }
                        catch { }
                    }
                }
            }
            Thread.Sleep(2);
        }

        #endregion

        #region Fields
        protected ConcurrentQueue<SocketAsyncEventArgs> SocketQueue;
        protected HashSet<SocketAsyncEventArgs> ActiveSockets = new HashSet<SocketAsyncEventArgs>();
        #endregion

        #region Public

        public bool Pop(out SocketAsyncEventArgs saea)
        {
            saea = null;
            if (Disposing) return false;
            if (SocketQueue.TryDequeue(out saea))
            {
                if (saea.Handler() != null)
                {
                    try
                    {
                        saea.Handler().OnDequeueHandler();
                    }
                    catch { }
                }
                lock (ActiveSockets)
                {
                    ActiveSockets.Add(saea);
                }

                return true;
            }
            return false;
        }

        public void Push(SocketAsyncEventArgs saea)
        {
            lock (ActiveSockets)
            {
                ActiveSockets.Remove(saea);
            }

            if (Disposing)
            {
                DisposeHandler(saea);
            }
            else
            {
                SocketQueue.Enqueue(saea);
            }
        }

        #endregion
    }
}
