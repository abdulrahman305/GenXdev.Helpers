using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Handlers
{
    public interface ISocketIOHandler
    {
        event EventHandler<SocketAsyncEventArgs> Completed;

        int ReceiveBlocking(SocketAsyncEventArgs saea, byte[] buffer, int offset, int count);
        int SendBlocking(SocketAsyncEventArgs saea, byte[] buffer);
        bool SendAsync(SocketAsyncEventArgs saea);
        bool ReceiveAsync(SocketAsyncEventArgs saea);
        bool ConnectAsync(SocketAsyncEventArgs saea);
        bool DisconnectAsync(SocketAsyncEventArgs saea);
        void Close(SocketAsyncEventArgs saea);

        void SetTcpKeepAlive(SocketAsyncEventArgs saea, uint keepaliveTime, uint keepaliveInterval);

        void SetReceiveTimeout(SocketAsyncEventArgs saea, int timeout);
        void SetSendTimeout(SocketAsyncEventArgs saea, int timeout);
        void SetReceiveBufferSize(SocketAsyncEventArgs saea, int size);
        void SetSendBufferSize(SocketAsyncEventArgs saea, int size);
        void SetBlocking(SocketAsyncEventArgs saea, bool blocking);
        void SetNoDelay(SocketAsyncEventArgs saea, bool nodelay);

        int GetReceiveTimeout(SocketAsyncEventArgs saea);

        bool GetNoDelay(SocketAsyncEventArgs saea);

        bool SocketHasDataAvailable(SocketAsyncEventArgs saea);
        int NrOfBytesSocketHasAvailable(SocketAsyncEventArgs saea);

        int GetLocalPort(SocketAsyncEventArgs saea);
        int GetRemotePort(SocketAsyncEventArgs saea);
        void SetLingerState(SocketAsyncEventArgs saea, bool linger);

        void Shutdown(SocketAsyncEventArgs saea, SocketShutdown how);

        void SetLingerTime(SocketAsyncEventArgs saea, int lingerTime);

        bool IsConnectionlessSocket(SocketAsyncEventArgs saea);

        bool IsConnected(SocketAsyncEventArgs saea);

        SocketAsyncOperation GetLastOperation(SocketAsyncEventArgs saea);

        int GetBytesTransfered(SocketAsyncEventArgs saea);

        SocketError GetSocketError(SocketAsyncEventArgs saea);

        int GetReceivedData(SocketAsyncEventArgs saea, GenXdev.Buffers.DynamicBuffer rxBuffer);

        void PrepareSendBuffer(SocketAsyncEventArgs saea, GenXdev.Buffers.DynamicBuffer buffer, int count);
        void Unregister(SocketAsyncEventArgs saea);
    }
}