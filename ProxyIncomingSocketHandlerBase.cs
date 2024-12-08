using GenXdev.AsyncSockets.Handlers;
using GenXdev.Configuration;
using GenXdev.MemoryManagement;
using Ninject;

namespace GenXdev.AsyncSockets.Containers
{
    public abstract class ProxyIncomingSocketHandlerBase : SocketHandlerBase
    {
        [Inject]
        public ProxyIncomingSocketHandlerBase(
            IKernel Kernel
            , IMemoryManagerConfiguration MemoryConfiguration
            , IServiceMemoryManager MemoryManager
            , String ServiceName

#if (Logging)
, IServiceLogger Logger
#endif
, object Pool = null
)
            : base(
                Kernel
                , MemoryConfiguration
                , MemoryManager
                , ServiceName
#if (Logging)
, Logger
#endif
, Pool
        )
        {
            this.OnHandleReset += ProxyIncomingSocketHandlerBase_OnHandleReset;
        }

        void ProxyIncomingSocketHandlerBase_OnHandleReset(object sender, EventArgs e)
        {
            if (OutgoingHandler != null)
            {
                OutgoingHandler.Close();
                OutgoingHandler = null;
            }
        }

        public ProxyOutgoingSocketHandlerBase OutgoingHandler { get; set; }
    }
}
