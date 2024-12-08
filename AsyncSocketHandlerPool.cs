/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.AsyncSockets.Handlers;
using Ninject;
using System.Collections.Concurrent;
using GenXdev.MemoryManagement;
using GenXdev.Configuration;
using GenXdev.AsyncSockets.Configuration;
using Ninject.Parameters;

namespace GenXdev.AsyncSockets.Containers
{
    public class AsyncSocketHandlerPool<HandlerType> : IAsyncSocketHandlerPool where HandlerType : SocketHandlerBase
    {
        #region Initialization

        IKernel Kernel;
        IMemoryManagerConfiguration MemoryConfiguration;
        IServiceMemoryManager MemoryManager;
        ISocketHandlerPoolConfiguration PoolConfiguration;
#if (Logging)
        IServiceLogger Logger;
#endif
        String ServiceName;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        [Inject]
        public AsyncSocketHandlerPool(
            IKernel Kernel,
            IMemoryManagerConfiguration MemoryConfiguration,
            IServiceMemoryManager MemoryManager,
#if (Logging)
 IServiceLogger Logger,
#endif
 ISocketHandlerPoolConfiguration PoolConfiguration,
            String ServiceName,
            bool DoInitialize = true
        )
        {
            this.Kernel = Kernel;
#if (Logging)
            this.Logger = Logger;
#endif
            this.MemoryConfiguration = MemoryConfiguration;
            this.MemoryManager = MemoryManager;
            this.PoolConfiguration = PoolConfiguration;
            this.ServiceName = ServiceName;

            if (DoInitialize)
            {
                Initialize();
            }
        }

        ~AsyncSocketHandlerPool()
        {
            Dispose(false);
        }

        protected bool started = false;

        public EventHandler<HandlerClosedEventArgs> RemoveProtocolHandlerCallback { get; set; }

        virtual protected void OnHandlerClosed(object sender, HandlerClosedEventArgs e)
        {
            SocketHandlerBase handler = e.handler;
            var callback = RemoveProtocolHandlerCallback;
            if (callback != null)
            {
                callback(this, new HandlerClosedEventArgs(handler));
            }
            else
            {
                Push(e.handler);
            }
        }

        protected void Initialize()
        {
            saeaBufferSize = Math.Max(MemoryConfiguration.ReceiveBufferSize, MemoryConfiguration.SendBufferSize);
            SocketQueue = new ConcurrentQueue<SocketHandlerBase>();

            Start();
        }

        HandlerType CreateNewHandler()
        {
            // create handler
            var handler = Kernel.Get<HandlerType>(
                new IParameter[2] {
                    new ConstructorArgument("ServiceName", ServiceName),
                    new ConstructorArgument("Pool", this)});

            handler.RegisterHandler(handler.saeaHandler, OnHandlerClosed);

            return handler;
        }

        protected virtual void PerformMaintenance()
        {
            SocketHandlerBase[] activeSockets;

            lock (ActiveSockets)
            {
                activeSockets = ActiveSockets.ToArray<SocketHandlerBase>();
            }
            foreach (var activeSocket in activeSockets)
            {
                if (IsExpired(activeSocket))
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogHandlerFlowMessage(activeSocket.ControllingHandler.saeaHandler, () => { return "Socket has timed out!"; });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    activeSocket.ControllingHandler.Close();
                }
            }
        }

        protected virtual bool IsExpired(SocketHandlerBase activeSocket)
        {
            return activeSocket.ControllingHandler.IsTimedOut;
        }

        protected bool Disposing;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            started = false;

            if (disposing)
            {

                Disposing = true;
                if (SocketQueue == null)
                    return;

                StopAllActiveConnections();


                // empty SocketQueue
                while (SocketQueue.TryDequeue(out SocketHandlerBase handler))
                {
                    handler.Dispose();
                }
            }
        }

        public virtual void Stop()
        {
            started = false;

            StopAllActiveConnections();
        }

        public virtual void Start()
        {
            if (!started)
            {
                started = true;

                Task.Factory.StartNew(async delegate
                    {
                        while (started)
                        {
                            try
                            {
                                PerformMaintenance();
                            }
                            catch
                            {
                                // ignore
                            }

                            await Task.Delay(1000);
                        }
                    },
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default
                ).Unwrap();
            }
        }

        public void StopAllActiveConnections()
        {
            if (ActiveSockets != null)
            {
                lock (ActiveSockets)
                {
                    var activeSockets = ActiveSockets.ToArray<SocketHandlerBase>();

                    foreach (var activeSocket in activeSockets)
                    {
                        activeSocket.ControllingHandler.Close();
                    }
                }
            }
        }

        #endregion

        #region Fields
        int saeaBufferSize;
        protected ConcurrentQueue<SocketHandlerBase> SocketQueue;
        protected HashSet<SocketHandlerBase> ActiveSockets = new HashSet<SocketHandlerBase>();
        //protected Timer Timer;
        #endregion

        #region Protected

        protected void FireHandleCapturedSocketReset(SocketHandlerBase socketHandler, bool resetWasRequested)
        {
            socketHandler.HandleCapturedSocketReset(resetWasRequested);
        }

        #endregion

        #region Public

        public bool Pop(out SocketHandlerBase handler)
        {
            lock (ActiveSockets)
            {
                var count = SocketQueue.Count;

                if (!SocketQueue.TryDequeue(out handler) && ((count < this.PoolConfiguration.MaxConnections) || (this.PoolConfiguration.MaxConnections == 0)))
                {
                    handler = CreateNewHandler();
                }

                if (handler != null)
                {
                    handler.OnDequeueHandler();
                    handler.RegisterHandler(handler.saeaHandler, OnHandlerClosed);
                    handler.SetCurrentStageTimeoutSeconds(10);
                    handler.SetActivityTimestamp();

                    ActiveSockets.Add(handler);

                    return true;
                }
            }
            return false;
        }

        public void Push(SocketHandlerBase handler)
        {
            lock (ActiveSockets)
            {
                ActiveSockets.Remove(handler);
                handler.OnEnqueueHandler();

                if (Disposing)
                {
                    handler.Dispose();
                }
                else
                {
                    SocketQueue.Enqueue(handler);
                }
            }
        }

        #endregion
    }
}
