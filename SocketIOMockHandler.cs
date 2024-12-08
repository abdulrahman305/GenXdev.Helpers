using GenXdev.AsyncSockets.Containers;
using GenXdev.AsyncSockets.Handlers;
using GenXdev.Buffers;
using System.Net.Sockets;
using Ninject;
using GenXdev.AsyncSockets.Logging;

namespace GenXdev.AsyncSockets.Mocking
{
    public class SocketIOMockHandler : ISocketIOHandler
    {
        #region Fields

        EventHandler OnStartHandlingServer;
        volatile SocketAsyncEventArgs saea;
        volatile int bytesTransferredTx;
        object stateLock = new object();
        internal long state = (int)SocketIOHandlerState.Idle;
        internal long oldState = (int)SocketIOHandlerState.Idle;

        internal SocketIOMockHandler Remote;
        DynamicBuffer RxBuffer;
        DynamicBuffer TxBuffer;
        bool IsServer;
        volatile int LocalPort = 1234;
        volatile bool Linger = false;
        IServiceLogger Logger;
        volatile int LingerTime;
        volatile int ReceiveTimeout;
        volatile int SendTimeout;
        volatile int ReceiveBufferSize;
        volatile int SendBufferSize;
        volatile bool Blocking;
        volatile bool NoDelay;

        #endregion

        #region Events

        public event EventHandler<SocketAsyncEventArgs> Completed;
        internal void TriggerCompleted(object sender, SocketAsyncEventArgs saea)
        {
            var args = this.saea;
            this.saea = null;

            if (args == null)
                throw new InvalidOperationException();

            var handler = Completed;

            if (handler != null)
            {
                handler(sender, args);
            }
        }
        #endregion

        #region Initialization

        public SocketIOMockHandler()
        {
        }

        public void Initialize(IKernel Kernel, SocketIOMockHandler Remote, int? localPort = null, bool IsServer = true, EventHandler OnStartHandlingServer = null)
        {
            this.Logger = Kernel.Get<IServiceLogger>();
            this.Remote = Remote;
            Remote.Remote = this;
            this.TxBuffer = Remote.RxBuffer;
            TxBuffer.OnRemoved += TxBuffer_OnRemoved;
            this.RxBuffer = Remote.TxBuffer;
            this.IsServer = IsServer;
            this.LocalPort = localPort.HasValue ? (localPort.Value) : IsServer ? (new Random()).Next(50000) + 2000 : 1234;
            this.OnStartHandlingServer = OnStartHandlingServer;
        }

        public void Initialize(IKernel Kernel, int? localPort = null, bool IsServer = true, EventHandler OnStartHandlingServer = null)
        {
            this.Logger = Kernel.Get<IServiceLogger>();
            this.TxBuffer = Kernel.Get<DynamicBuffer>();
            TxBuffer.OnRemoved += TxBuffer_OnRemoved;
            this.RxBuffer = Kernel.Get<DynamicBuffer>();
            this.IsServer = IsServer;
            this.LocalPort = localPort.HasValue ? (localPort.Value) : IsServer ? (new Random()).Next(50000) + 2000 : 1234;
            this.OnStartHandlingServer = OnStartHandlingServer;
        }

        #endregion

        #region ISocketIOHandler

        public int ReceiveBlocking(System.Net.Sockets.SocketAsyncEventArgs saea, byte[] buffer, int offset, int count)
        {
            this.saea = saea;

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> ReceiveBlocking start, " +
                    "receiving " + count.ToString() + " bytes into buffer @ offset " + offset.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetNewState(SocketIOHandlerState.Connected, SocketIOHandlerState.ReceivingBlocking);

            int read = RxBuffer.Remove(buffer, offset, count);
            while ((read < count) && IsOperational() && Remote.IsOperational())
            {
                System.Threading.Thread.Sleep(10);

                read += RxBuffer.Remove(buffer, offset + read, count - read);
            }

            if (!(IsOperational() && Remote.IsOperational()))
            {
                throw new ObjectDisposedException("Socket is closed");
            }

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            var b = buffer;
            if (offset > 0)
            {
                b = new byte[count];
                Buffer.BlockCopy(buffer, offset, b, 0, count);
            }

            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> ReceiveBlocking finished, " +

                    (IsServer ? "Server: " : "Client: ") +
                    "received " + count.ToString() + " bytes into buffer @ offset " + offset.ToString() + " => " +
                    GenXdev.Helpers.Hash.FormatBytesAsHexString(b);
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetNewState(SocketIOHandlerState.ReceivingBlocking, SocketIOHandlerState.Connected);

            return read;
        }

        public int SendBlocking(System.Net.Sockets.SocketAsyncEventArgs saea, byte[] buffer)
        {
            long currentState = Interlocked.Read(ref state);
            if (currentState != (int)SocketIOHandlerState.Connected)
            {
                throw new InvalidOperationException("SendBlocking called while in state: " +
                    Enum.GetName(typeof(SocketIOHandlerState), currentState));
            }

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> SendBlocking, " +

                    (IsServer ? "Server: " : "Client: ") +
                    "sending blocking: " + buffer.Length.ToString() + " bytes => " +
                    GenXdev.Helpers.Hash.FormatBytesAsHexString(buffer);
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            return TxBuffer.Add(buffer);
        }

        public bool SendAsync(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            this.saea = saea;

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> SendAsync";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetNewState(SocketIOHandlerState.Connected, SocketIOHandlerState.Sending);

            ThreadPool.QueueUserWorkItem((obj) =>
            {

                lock (TxBuffer.SyncRoot)
                {
                    if (saea.Count == 0)
                        throw new InvalidOperationException();

                    bytesTransferredTx = TxBuffer.Add(saea.Buffer, saea.Offset, saea.Count);

                    if (Remote.TrySetUnIdle())
                    {
                        Monitor.Exit(TxBuffer.SyncRoot);
                        try
                        {
                            Remote.TriggerCompleted(Remote, Remote.saea);
                        }
                        finally
                        {
                            Monitor.Enter(TxBuffer.SyncRoot);
                        }
                    }

                    if ((TxBuffer.Count < SendBufferSize * 2) || !TrySetIdle(SocketIOHandlerState.Sending))
                    {
                        SetNewState(SocketIOHandlerState.Sending, SocketIOHandlerState.Connected);

                        ThreadPool.QueueUserWorkItem((obj2) =>
                        {
                            TriggerCompleted(this, this.saea);
                        });
                    }
                }
            });

            return true;
        }

        public bool ReceiveAsync(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            this.saea = saea;

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> ReceiveAsync";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetNewState(SocketIOHandlerState.Connected, SocketIOHandlerState.Receiving);

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                // no race conditions
                lock (RxBuffer.SyncRoot)
                {
                    if (RxBuffer.Count > 0)
                    {
                        SetNewState(SocketIOHandlerState.Receiving, SocketIOHandlerState.Connected);

                        ThreadPool.QueueUserWorkItem((obj2) =>
                        {
                            TriggerCompleted(this, this.saea);
                        });
                    }
                    else
                    {
                        SetNewState(SocketIOHandlerState.Receiving, SocketIOHandlerState.ReceivingIdle);
                    }
                }
            });

            return true;
        }

        public bool ConnectAsync(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            if (IsServer)
            {
                throw new InvalidOperationException("IsServer is true, but ConnectAsync is called");
            }

            this.saea = saea;

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> ConnectAsync";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetNewState(SocketIOHandlerState.Idle, SocketIOHandlerState.Connecting);

            ThreadPool.QueueUserWorkItem((obj) =>
            {

                SetNewState(SocketIOHandlerState.Connecting, SocketIOHandlerState.Connected);
                Remote.SetNewState(SocketIOHandlerState.Idle, SocketIOHandlerState.Connected);

                if (OnStartHandlingServer == null)
                    throw new InvalidOperationException("Must assign OnStartHandlingServer for server iohandlers");

                ThreadPool.QueueUserWorkItem((obj2) =>
                {
                    TriggerCompleted(this, saea);
                });

                OnStartHandlingServer(this, EventArgs.Empty);
            });

            return true;
        }

        public bool DisconnectAsync(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            this.saea = saea;

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> DisconnectAsync";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            SetNewState(SocketIOHandlerState.Connected, SocketIOHandlerState.Disconnecting);

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                SetNewState(SocketIOHandlerState.Disconnecting, SocketIOHandlerState.Idle);

                TriggerCompleted(this, saea);
            });

            return true;
        }

        public void Close(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            Interlocked.Exchange(ref state, (int)SocketIOHandlerState.Disposed);

            if (Remote.TrySetNewState(SocketIOHandlerState.ReceivingIdle, SocketIOHandlerState.Disposed) ||
                Remote.TrySetNewState(SocketIOHandlerState.Connecting, SocketIOHandlerState.Disposed) ||
                Remote.TrySetNewState(SocketIOHandlerState.Disconnecting, SocketIOHandlerState.Disposed) ||
                Remote.TrySetNewState(SocketIOHandlerState.Sending, SocketIOHandlerState.Disposed) ||
                Remote.TrySetNewState(SocketIOHandlerState.SendingIdle, SocketIOHandlerState.Disposed)
            )
            {
                Remote.TriggerCompleted(Remote, Remote.saea);
            }
        }

        public void SetTcpKeepAlive(System.Net.Sockets.SocketAsyncEventArgs saea, uint keepaliveTime, uint keepaliveInterval)
        {

        }

        public void SetReceiveTimeout(System.Net.Sockets.SocketAsyncEventArgs saea, int timeout)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> Setting new receive timeout to " + timeout.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.ReceiveTimeout = timeout;
        }

        public void SetSendTimeout(System.Net.Sockets.SocketAsyncEventArgs saea, int timeout)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> Setting new send timeout to " + timeout.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.SendTimeout = timeout;
        }

        public void SetReceiveBufferSize(System.Net.Sockets.SocketAsyncEventArgs saea, int size)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> Setting new receivebuffer size to " + size.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.ReceiveBufferSize = size;
        }

        public void SetSendBufferSize(System.Net.Sockets.SocketAsyncEventArgs saea, int size)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> Setting new sendbuffer size to " + size.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.SendBufferSize = size;
        }

        public void SetBlocking(System.Net.Sockets.SocketAsyncEventArgs saea, bool blocking)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> set blocking: " + blocking.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.Blocking = blocking;
        }

        public void SetNoDelay(System.Net.Sockets.SocketAsyncEventArgs saea, bool nodelay)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> set nodelay: " + nodelay.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.NoDelay = nodelay;
        }

        public int GetReceiveTimeout(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            return ReceiveTimeout;
        }

        public bool GetNoDelay(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            return NoDelay;
        }

        public bool SocketHasDataAvailable(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            if (!IsOperational() || (!Remote.IsOperational()))
            {
                throw new ObjectDisposedException("Socket is closed");
            }

            return RxBuffer.Count > 0;
        }

        public int NrOfBytesSocketHasAvailable(SocketAsyncEventArgs saea)
        {
            return RxBuffer.Count;
        }

        public int GetLocalPort(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            return LocalPort;
        }

        public int GetRemotePort(System.Net.Sockets.SocketAsyncEventArgs saea)
        {
            return Remote.GetLocalPort(null);
        }

        public void SetLingerState(System.Net.Sockets.SocketAsyncEventArgs saea, bool linger)
        {
            this.Linger = linger;
        }

        public void Shutdown(System.Net.Sockets.SocketAsyncEventArgs saea, System.Net.Sockets.SocketShutdown how)
        {
            Close(saea);
        }

        public void SetLingerTime(System.Net.Sockets.SocketAsyncEventArgs saea, int lingerTime)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> Setting new lingertime to " + lingerTime.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            this.LingerTime = lingerTime;
        }

        public bool IsConnectionlessSocket(System.Net.Sockets.SocketAsyncEventArgs saeaHandler)
        {
            return false;
        }

        public bool IsConnected(System.Net.Sockets.SocketAsyncEventArgs saeaHandler)
        {
            long currentState = Interlocked.Read(ref state);

            return (currentState != (int)SocketIOHandlerState.Idle) && (currentState != (int)SocketIOHandlerState.Disposed) && (currentState != (int)SocketIOHandlerState.Connecting);
        }

        public bool IsOperational()
        {
            lock (stateLock)
            {
                long currentState = Interlocked.Read(ref state);
                long oldState = Interlocked.Read(ref this.oldState);

                return ((state != (int)SocketIOHandlerState.Idle) || (oldState == (int)SocketIOHandlerState.Disconnecting)) &&
                        (state != (int)SocketIOHandlerState.Disposed);
            }
        }

        public System.Net.Sockets.SocketAsyncOperation GetLastOperation(System.Net.Sockets.SocketAsyncEventArgs saeaHandler)
        {
            switch ((SocketIOHandlerState)Interlocked.Read(ref oldState))
            {
                case SocketIOHandlerState.Connecting:
                    return SocketAsyncOperation.Connect;

                case SocketIOHandlerState.Disconnecting:
                    return SocketAsyncOperation.Disconnect;

                case SocketIOHandlerState.Receiving:
                case SocketIOHandlerState.ReceivingIdle:
                    return SocketAsyncOperation.Receive;

                case SocketIOHandlerState.Sending:
                case SocketIOHandlerState.SendingIdle:
                    return SocketAsyncOperation.Send;

                default:
                    return SocketAsyncOperation.None;
            }
        }

        public int GetBytesTransfered(System.Net.Sockets.SocketAsyncEventArgs saeaHandler)
        {
            switch (GetLastOperation(saeaHandler))
            {
                case SocketAsyncOperation.Receive:
                    return Math.Min(RxBuffer.Count, ReceiveBufferSize);
                case SocketAsyncOperation.Send:
                    return bytesTransferredTx;
            }
            return 0;
        }

        public System.Net.Sockets.SocketError GetSocketError(System.Net.Sockets.SocketAsyncEventArgs saeaHandler)
        {
            if (!(Remote.IsOperational() && IsOperational()))
                return SocketError.Shutdown;

            return SocketError.Success;
        }

        public int GetReceivedData(SocketAsyncEventArgs saea, DynamicBuffer rxBuffer)
        {
            var bytes = this.RxBuffer.RemoveBytes(GetBytesTransfered(saea));

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> " +
                    "Read " + bytes.Length.ToString() + " bytes from receivebuffer => " + GenXdev.Helpers.Hash.FormatBytesAsHexString(bytes);
            });

#endif
            #endregion ------------------------------------------------------------------------------------------------

            return rxBuffer.Add(bytes);

            //return this.RxBuffer.MoveTo(rxBuffer, bytesTransferred);
        }

        public void PrepareSendBuffer(SocketAsyncEventArgs saea, DynamicBuffer buffer, int count)
        {
            this.saea = saea;
            var bytes = buffer.RemoveBytes(count);

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saea, () =>
            {
                return "=> PrepareSendBuffer, " +

                    (IsServer ? "Server: " : "Client: ") +
                    "filling send buffer: " + bytes.Length.ToString() + " bytes => " +
                    GenXdev.Helpers.Hash.FormatBytesAsHexString(bytes);
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            bytesTransferredTx = bytes.Length;
            saea.SetBuffer(bytes, 0, count);
        }


        public void Unregister(SocketAsyncEventArgs saea)
        {
            this.saea = saea == this.saea ? null : this.saea;
        }

        #endregion

        #region Private

        void TxBuffer_OnRemoved(object sender, DynamicBufferEmptyEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((obj2) =>
            {
                bool trigger = false;

                lock (TxBuffer.SyncRoot)
                {
                    if (TxBuffer.Count <= SendBufferSize * 2)
                    {
                        trigger = TrySetNewState(SocketIOHandlerState.SendingIdle, SocketIOHandlerState.Connected);
                    }
                }

                if (trigger)
                {
                    TriggerCompleted(this, this.saea);
                }
            });

        }

        internal void SetNewState(SocketIOHandlerState requiredCurrentState, SocketIOHandlerState newState)
        {
            lock (stateLock)
            {
                var result = Interlocked.CompareExchange(
                    ref this.state,
                    (int)newState,
                    (int)requiredCurrentState
                    );

                Interlocked.Exchange(ref oldState, result);

                if (result != (int)requiredCurrentState)
                {
                    throw new InvalidOperationException(String.Format("SetNewState, " +
                        (IsServer ? "Server" : "Client") + " handler wants to switch state from \"{0}\" to \"{1}\", but IOHandler is in state \"{2}\"",
                        Enum.GetName(typeof(SocketIOHandlerState), requiredCurrentState),
                        Enum.GetName(typeof(SocketIOHandlerState), newState),
                        Enum.GetName(typeof(SocketIOHandlerState), result)
                    ));
                }

                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saea, () =>
                {
                    return String.Format("=> " +
                            "Statechange from \"{0}\"  to \"{1}\"",
                            Enum.GetName(typeof(SocketIOHandlerState), requiredCurrentState),
                            Enum.GetName(typeof(SocketIOHandlerState), newState)
                    );
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------
            }
        }

        bool TrySetIdle(SocketIOHandlerState requiredCurrentState)
        {
            lock (stateLock)
            {
                if (requiredCurrentState == SocketIOHandlerState.Sending)
                {
                    return
                         (Interlocked.Read(ref Remote.state) != (int)SocketIOHandlerState.SendingIdle) &&
                        TrySetNewState(requiredCurrentState, SocketIOHandlerState.SendingIdle);
                }
                if (requiredCurrentState == SocketIOHandlerState.Receiving)
                {
                    return
                        (Interlocked.Read(ref Remote.state) != (int)SocketIOHandlerState.ReceivingIdle) &&
                        TrySetNewState(requiredCurrentState, SocketIOHandlerState.ReceivingIdle);
                }

                throw new InvalidOperationException();
            }
        }

        bool TrySetUnIdle()
        {
            lock (stateLock)
            {
                return
                    TrySetNewState(SocketIOHandlerState.ReceivingIdle, SocketIOHandlerState.Connected) ||
                    TrySetNewState(SocketIOHandlerState.SendingIdle, SocketIOHandlerState.Connected);
            }
        }

        bool TrySetNewState(SocketIOHandlerState requiredCurrentState, SocketIOHandlerState newState)
        {
            lock (stateLock)
            {
                var oldState = Interlocked.CompareExchange(
                    ref this.state,
                    (int)newState,
                    (int)requiredCurrentState
                );

                if (oldState != (int)requiredCurrentState)
                {
                    return false;
                }

                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saea, () =>
                {
                    return String.Format("=> " +
                            "Successfull statechange from \"{0}\"  to \"{1}\"",
                            Enum.GetName(typeof(SocketIOHandlerState), requiredCurrentState),
                            Enum.GetName(typeof(SocketIOHandlerState), newState)
                    );
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                Interlocked.Exchange(ref this.oldState, oldState);

                return true;
            }
        }

        internal void SetSaea(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            saea = socketAsyncEventArgs;
        }


        #endregion
    }
}
