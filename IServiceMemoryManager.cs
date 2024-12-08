namespace GenXdev.MemoryManagement
{
    public interface IServiceMemoryManager : IDisposable
    {
        // Summary:
        //     Releases the buffers currently cached in the manager.
        void Clear();

        //
        // Summary:
        //     Returns a buffer to the pool.
        //
        // Parameters:
        //   buffer:
        //     A reference to the buffer being returned.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer reference cannot be null.
        //
        //   System.ArgumentException:
        //     Length of buffer does not match the pool's buffer length property.
        void ReturnBuffer(byte[] buffer);
        //
        // Summary:
        //     Gets a buffer of at least the specified size from the pool.
        //
        // Parameters:
        //   bufferSize:
        //     The size, in bytes, of the requested buffer.
        //
        // Returns:
        //     A byte array that is the requested size of the buffer.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     bufferSize cannot be less than zero.
        byte[] TakeBuffer(int bufferSize);
    }
}
