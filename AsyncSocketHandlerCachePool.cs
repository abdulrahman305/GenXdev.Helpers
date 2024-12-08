using GenXdev.AsyncSockets.Configuration;
using GenXdev.Configuration;
using GenXdev.MemoryManagement;
using Ninject;
using GenXdev.AsyncSockets.Handlers;

namespace GenXdev.AsyncSockets.Containers
{
    public class AsyncSocketHandlerCachePool<HandlerType> : AsyncSocketHandlerPool<HandlerType> where HandlerType : SocketHandlerBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AsyncSocketHandlerCachePool(
             IKernel Kernel
            , IMemoryManagerConfiguration MemoryConfiguration
            , IServiceMemoryManager MemoryManager
#if (Logging)
            , IServiceLogger Logger
#endif
            , string ServiceName
            , bool DoInitialize = true
        ) : base(
             Kernel
            , MemoryConfiguration
            , MemoryManager
#if (Logging)
            , Logger
#endif
            , new SocketHandlerPoolConfiguration(0)
            , ServiceName
            , DoInitialize
        )
        {
            Start();
        }

        public override void Start()
        {
            started = true;
        }
    }
}
