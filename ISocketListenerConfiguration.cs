/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.Configuration;

namespace GenXdev.AsyncSockets.Configuration
{
    public enum SocketBindType { LocalOnly, ExternalOnly, Both };
    public interface ISocketListenerConfiguration : ISocketHandlerPoolConfiguration
    {
        event EventHandler<OnPropertyChangedEventArgs> OnPropertyChanged;

        // max connection backlog
        int MaxBackLog { get; }

        // incoming port(s)
        int[] ListeningPorts { get; }
    }
}
