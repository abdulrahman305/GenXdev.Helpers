namespace GenXdev.AsyncSockets.Configuration
{
    public class SocketHandlerPoolConfiguration : ISocketHandlerPoolConfiguration
    {
        public SocketHandlerPoolConfiguration(int maxConnections)
        {
            this.MaxConnections = maxConnections;
        }

        public int MaxConnections { get; set; }
    }
}
