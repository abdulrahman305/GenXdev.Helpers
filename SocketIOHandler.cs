using System.Net.Sockets;

namespace GenXdev.AsyncSockets.Handlers
{
    public class SocketIOHandler : ISocketIOHandler
    {
        public event EventHandler<SocketAsyncEventArgs> Completed;

        void OnCompleted(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnCompleted;

            var handler = Completed;

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public int ReceiveBlocking(SocketAsyncEventArgs saea, byte[] buffer, int offset, int size)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            saea.Completed -= OnCompleted;
            saea.Completed += OnCompleted;

            return saea.AcceptSocket.Receive(buffer, offset, size, SocketFlags.None);
        }

        public int SendBlocking(SocketAsyncEventArgs saea, byte[] buffer)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            saea.Completed -= OnCompleted;
            saea.Completed += OnCompleted;

            return saea.AcceptSocket.Send(buffer);
        }

        public void SetReceiveTimeout(SocketAsyncEventArgs saea, int timeout)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.ReceiveTimeout = timeout;
        }

        public void SetSendTimeout(SocketAsyncEventArgs saea, int timeout)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.SendTimeout = timeout;
        }

        public bool GetNoDelay(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            return saea.AcceptSocket.NoDelay;
        }

        public int GetReceiveTimeout(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            return saea.AcceptSocket.ReceiveTimeout;
        }

        public bool SendAsync(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            saea.Completed -= OnCompleted;
            saea.Completed += OnCompleted;

            if (saea.AcceptSocket.SendAsync(saea))
            {
                return true;
            }

            saea.Completed -= OnCompleted;
            return false;
        }

        public bool ReceiveAsync(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            saea.Completed -= OnCompleted;
            saea.Completed += OnCompleted;

            if (saea.AcceptSocket.ReceiveAsync(saea))
            {
                return true;
            }

            saea.Completed -= OnCompleted;
            return false;
        }

        public bool ConnectAsync(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            saea.Completed -= OnCompleted;
            saea.Completed += OnCompleted;

            if (saea.AcceptSocket.ConnectAsync(saea))
            {
                return true;
            }

            saea.Completed -= OnCompleted;
            return false;
        }

        public bool DisconnectAsync(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            saea.Completed -= OnCompleted;
            saea.Completed += OnCompleted;

            if (saea.AcceptSocket.DisconnectAsync(saea))
            {
                return true;
            }

            saea.Completed -= OnCompleted;
            return false;
        }

        public void Close(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.Close(0);
        }

        public void SetTcpKeepAlive(SocketAsyncEventArgs saea, uint keepaliveTime, uint keepaliveInterval)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.SetTcpKeepAlive(keepaliveTime, keepaliveInterval);
        }

        public void SetReceiveBufferSize(SocketAsyncEventArgs saea, int size)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            if (saea.AcceptSocket.ReceiveBufferSize != size)
            {
                saea.AcceptSocket.ReceiveBufferSize = size;
            }
        }

        public void SetSendBufferSize(SocketAsyncEventArgs saea, int size)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            if (saea.AcceptSocket.SendBufferSize != size)
            {
                saea.AcceptSocket.SendBufferSize = size;
            }
        }

        public void SetBlocking(SocketAsyncEventArgs saea, bool blocking)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.Blocking = blocking;
        }

        public void SetNoDelay(SocketAsyncEventArgs saea, bool nodelay)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.NoDelay = nodelay;
        }

        public bool SocketHasDataAvailable(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            return (saea.AcceptSocket.Available > 0)
                // todo double check
                ;//|| saea.AcceptSocket.Poll(1, SelectMode.SelectRead);
        }

        public int NrOfBytesSocketHasAvailable(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            return saea.AcceptSocket.Available;
        }

        public int GetLocalPort(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            System.Net.IPEndPoint endpoint = saea.AcceptSocket.LocalEndPoint as System.Net.IPEndPoint;

            return endpoint != null ? endpoint.Port : 0;
        }

        public int GetRemotePort(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            System.Net.IPEndPoint endpoint = saea.AcceptSocket.RemoteEndPoint as System.Net.IPEndPoint;

            return endpoint != null ? endpoint.Port : 0;
        }

        public void SetLingerState(SocketAsyncEventArgs saea, bool linger)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;
            try
            {
                saea.AcceptSocket.LingerState.Enabled = false;
            }
            catch
            {
                // ignore
            }
        }

        public void Shutdown(SocketAsyncEventArgs saea, SocketShutdown how)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.Shutdown(how);
        }

        public void SetLingerTime(SocketAsyncEventArgs saea, int lingerTime)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return;

            saea.AcceptSocket.LingerState.LingerTime = lingerTime;
        }

        public bool IsConnectionlessSocket(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            return (saea.AcceptSocket.SocketType == SocketType.Dgram);
        }

        public bool IsConnected(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return false;

            return saea.AcceptSocket.Connected;
        }

        public SocketAsyncOperation GetLastOperation(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return SocketAsyncOperation.None;

            return saea.LastOperation;
        }

        public int GetBytesTransfered(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return 0;

            return saea.BytesTransferred;
        }

        public SocketError GetSocketError(SocketAsyncEventArgs saea)
        {
            if ((saea == null) || (saea.AcceptSocket == null))
                return SocketError.OperationNotSupported;

            return saea.SocketError;
        }

        public int GetReceivedData(SocketAsyncEventArgs saea, GenXdev.Buffers.DynamicBuffer rxBuffer)
        {
            return rxBuffer.Add(saea.Buffer, saea.Offset, saea.BytesTransferred);
        }

        public void PrepareSendBuffer(SocketAsyncEventArgs saea, GenXdev.Buffers.DynamicBuffer buffer, int count)
        {
            buffer.FillAndSetSendBuffer(saea, count, false);
        }

        public void Unregister(SocketAsyncEventArgs saea)
        {
            if (saea == null) return;
            saea.Completed -= OnCompleted;
        }
    }
}
