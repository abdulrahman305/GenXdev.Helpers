using GenXdev.AsyncSockets.Handlers;
using GenXdev.Configuration;
using GenXdev.MemoryManagement;
using Ninject;

namespace GenXdev.AsyncSockets.Containers
{
    public abstract class ProxyOutgoingSocketHandlerBase : SocketHandlerBase
    {
        [Inject]
        public ProxyOutgoingSocketHandlerBase(
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
            this.OnHandleReset += ProxyOutgoingSocketHandlerBase_OnHandleReset;
        }
        public

        void ProxyOutgoingSocketHandlerBase_OnHandleReset(object sender, EventArgs e)
        {
            if (IncomingHandler != null)
            {
                IncomingHandler.Close();
                IncomingHandler = null;
            }
        }

        public ProxyIncomingSocketHandlerBase IncomingHandler { get; set; }
    }
}
