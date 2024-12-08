/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */

namespace GenXdev.AsyncSockets.Configuration
{
    public interface IServiceThreadConfiguration
    {
        // max threads
        int MinWorkerThreads { get; }
        int MaxWorkerThreads { get; }
        int MinIOCompletionPortThreads { get; }
        int MaxIOCompletionPortThreads { get; }
    }
}
