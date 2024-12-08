using Ninject;
using GenXdev.Configuration;
using System.ServiceModel.Channels;

namespace GenXdev.MemoryManagement
{
    public class ServiceMemoryManager : IServiceMemoryManager
    {
        [Inject]
        public ServiceMemoryManager(IMemoryManagerConfiguration Configuration)
        {
            this.Configuration = Configuration;

            // Reserve large chunk of memory and reuse this without reallocations during runtime
            BufferManager = BufferManager.CreateBufferManager(Configuration.ReservedServerMemory, 1024 * 1024 * 10);
        }

        IMemoryManagerConfiguration Configuration { get; set; }

        BufferManager BufferManager;

        public void Clear()
        {
            BufferManager.Clear();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
        }

        ~ServiceMemoryManager()
        {
            Dispose(false);
        }

        public long mem;

        public void ReturnBuffer(byte[] buffer)
        {
            Interlocked.Add(ref mem, buffer.Length * -1);
            BufferManager.ReturnBuffer(buffer);
        }

        public byte[] TakeBuffer(int bufferSize)
        {
            var buf = BufferManager.TakeBuffer(bufferSize);
            Interlocked.Add(ref mem, buf.Length);
            return buf;
        }
    }
}
