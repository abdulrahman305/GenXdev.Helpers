using Ninject;
using System.Net.Sockets;
using GenXdev.AsyncSockets.Handlers;
using GenXdev.AsyncSockets.Configuration;
using GenXdev.Configuration;
using GenXdev.MemoryManagement;

namespace GenXdev.AsyncSockets.Containers
{
    public delegate void OutgoingSocketRemovedNotificationDelegate(SocketAsyncEventArgs saea);

    public class ProxyOutgoingSocketHandlerPool<T> : AsyncSocketHandlerPool<T> where T : ProxyOutgoingSocketHandlerBase
    {
        #region Initialization / finalization

        public ProxyOutgoingSocketHandlerPool(
            IKernel Kernel,
            IMemoryManagerConfiguration MemoryConfiguration,
            IServiceMemoryManager MemoryManager,
#if (Logging)
 IServiceLogger Logger,
#endif
 ISocketListenerConfiguration ListenerConfiguration,
            String ServiceName
            )

            : base(
            Kernel,
            MemoryConfiguration,
            MemoryManager,
#if (Logging)
            Logger,
#endif

 ListenerConfiguration,
            ServiceName,
            false
            )
        {
            base.RemoveProtocolHandlerCallback = OnRemoveOutgoingSocket;
#if (Logging)
            this.Logger = Kernel.Get<IServiceLogger>();
#endif
            Initialize();
        }

        #endregion

        #region Fields

        #region Service

#if (Logging)
        IServiceLogger Logger;
#endif

        #endregion

        #region Statistics

        // Total number of outgoing Outgoing connections
        public long TotalNrOfOutgoingOutgoingSockets;

#if (Logging)
        // Total number of outgoing connections made since last measurement
        public long TotalNrOfOutgoingSocketsConnected;

        // Total number of outgoing connections closed since last measurement
        public long TotalNrOfOutgoingSocketsClosed;
#endif

        #endregion

        #endregion

        #region Public

        public NextRequestedHandlerAction CaptureOutgoingSocket(ProxyIncomingSocketHandlerBase Registree)
        {
            if (Disposing)
                return NextRequestedHandlerAction.Dispose;

            // find it


            // not found?
            if (Pop(out SocketHandlerBase OutgoingHandler))
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(Registree.saeaHandler, () => { return "Added new Outgoing connection"; });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                Interlocked.Increment(ref TotalNrOfOutgoingOutgoingSockets);
#if (Logging)
                Interlocked.Increment(ref TotalNrOfOutgoingSocketsConnected);
#endif

                // start it
                Registree.OutgoingHandler = (ProxyOutgoingSocketHandlerBase)OutgoingHandler;
                ((ProxyOutgoingSocketHandlerBase)OutgoingHandler).IncomingHandler = Registree;
                OutgoingHandler.StartHandlingSocket(null);

                // schedule capture
                return OutgoingHandler.EnqueueCapture(Registree, 1);
            }
            else
            {
                return Registree.HandleCapturedSocketReset(false);
            }
        }

        #endregion

        #region Private

        void OnRemoveOutgoingSocket(object sender, HandlerClosedEventArgs e)
        {
            // Update statistics
            Interlocked.Decrement(ref TotalNrOfOutgoingOutgoingSockets);
#if (Logging)
            Interlocked.Increment(ref TotalNrOfOutgoingSocketsClosed);
#endif

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(e.handler.saeaHandler, () => { return "Outgoing socket closed"; });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            // Put the SocketAsyncEventArg back into the pool,
            // to be used again
            Push(e.handler);
        }

        #endregion
    }
}
