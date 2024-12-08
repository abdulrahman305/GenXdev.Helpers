using GenXdev.AsyncSockets.Configuration;
/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */

namespace GenXdev.AsyncSockets.Listener
{
    public interface ISocketHandlerListener : IDisposable
    {
        bool Started { get; }
        void Start();
        void Stop();

        void ActivateNewConfiguration(ISocketListenerConfiguration NewConfiguration);
    }
}
