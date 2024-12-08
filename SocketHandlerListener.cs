/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using Ninject;
using System.Net.Sockets;
using GenXdev.AsyncSockets.Handlers;
using System.Net;
using GenXdev.MemoryManagement;
using GenXdev.AsyncSockets.Containers;
using GenXdev.Configuration;
using GenXdev.AsyncSockets.Configuration;

namespace GenXdev.AsyncSockets.Listener
{
    public class SocketHandlerListener<HandlerType> : ISocketHandlerListener where HandlerType : SocketHandlerBase
    {
        #region Initialization

        IKernel Kernel;
        IServiceMemoryManager MemoryManager;
        ISocketListenerConfiguration ListenerConfiguration;
        IAsyncSocketListenerPool ListenerPool;

#if (Logging)
        IServiceLogger Logger;
#endif


        [Inject]
        public SocketHandlerListener(
            IKernel Kernel,
            IServiceMemoryManager MemoryManager,
            ISocketListenerConfiguration ListenerConfiguration,
            IAsyncSocketListenerPool ListenerPool,
            IAsyncSocketHandlerPool HandlerPool
#if (Logging)
, IServiceLogger Logger
#endif

    )
        {

            this.Kernel = Kernel;
            this.MemoryManager = MemoryManager;
            this.ListenerConfiguration = ListenerConfiguration;
            this.ListenerPool = ListenerPool;
            this.HandlerPool = HandlerPool;
#if (Logging)
            this.Logger = Logger;
#endif
            this.ListenerTimer = new System.Threading.Timer(new TimerCallback(ListenerTimerCallback), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            this.ListenerConfiguration.OnPropertyChanged += ListenerConfiguration_OnPropertyChanged;

            Initialize();
        }

        void ListenerConfiguration_OnPropertyChanged(object sender, OnPropertyChangedEventArgs e)
        {
            this.ActivateNewConfiguration(this.ListenerConfiguration);
        }

        void Initialize()
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogProgramFlowInfoMessage(() =>
            {
                return "Initializing socket listener for ports: " + String.Join(", ", (from q in ListenerConfiguration.ListeningPorts orderby q ascending select q.ToString()).ToList<string>());
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            #region Sockets

            // Pool of reusable SocketAsyncEventArgs objects for accept operations
            ListenerPool.OnListenerSocketIOCompleted = OnListenerSocketIOCompleted;

            // Pool of reusable SocketAsyncEventArgs objects for socket operations
            HandlerPool.RemoveProtocolHandlerCallback = OnRemoveSocket;

            #endregion

            #region Locks

            // Total nr of incoming connections enforcer
            MaxIncomingSocketsEnforcer = new Semaphore(ListenerConfiguration.MaxConnections, ListenerConfiguration.MaxConnections);

            #endregion
        }

        protected virtual IPEndPoint CreateEndPoint(int Port)
        {
            try
            {
                return new IPEndPoint(IPAddress.Any, Port);
            }
            catch (InvalidOperationException)
            {
                return new IPEndPoint(IPAddress.Any, Port);
            }
        }

        #endregion

        #region Fields

        #region Statistics

        #region Connections count

        // Total number of incoming connections
        long TotalNrOfIncomingSockets;


        #endregion

        #region Performance Logging

#if (Logging)
        // Last performance measurement time stamp
        double LastPerformanceMeasurementTimestamp;

        // Total number of incoming connections accepted since last measurement
        long TotalNrOfIncomingSocketsAccepted;

        // Total number of incoming connections closed since last measurement
        long TotalNrOfIncomingSocketsClosed;

#endif

        #endregion

        #endregion

        #region Misc

        // Timer for high precision time measurement
        GenXdev.Additional.HiPerfTimer.NativeMethods Timer = new GenXdev.Additional.HiPerfTimer.NativeMethods(true);

        #endregion

        #region Locks

        // Total nr of incoming connections enforcer
        Semaphore MaxIncomingSocketsEnforcer;

        #endregion

        #region Sockets

        // listener socket
        List<Socket> SocketListeners = new List<Socket>();
        private System.Threading.Timer ListenerTimer;

        #endregion

        #endregion

        #region Public
        public bool Started { get; private set; }
        public bool ListenerStarted { get; private set; }
        public IAsyncSocketHandlerPool HandlerPool { get; private set; }

        // starts the proxy server
        public void Start()
        {
            if (ListenerStarted)
                return;

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogProgramFlowInfoMessage(() =>
            {
                return "Starting socket listener for ports: " + String.Join(", ", (from q in ListenerConfiguration.ListeningPorts orderby q ascending select q.ToString()).ToList<string>());
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            Started = true;

            // Create listener
            SocketListeners = new List<Socket>();

            ActivateNewConfiguration(this.ListenerConfiguration);

            ListenerTimer.Change(1000, 1000);
        }

        // stops the proxy server
        public void Stop()
        {
            if (!Started)
                return;

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogProgramFlowInfoMessage(() =>
            {
                return "Stopping socket listener for ports: " + String.Join(", ", (from q in ListenerConfiguration.ListeningPorts orderby q ascending select q.ToString()).ToList<string>());
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            Started = false;
            ListenerTimer.Change(Timeout.Infinite, Timeout.Infinite);

            // Stop listener
            foreach (var socketListener in SocketListeners)
            {
                try
                {
                    socketListener.Close();
                }
                catch { }
            }
            SocketListeners.Clear();

            HandlerPool.Stop();
            HandlerPool.StopAllActiveConnections();

            ListenerStarted = false;
            DateTime start = DateTime.UtcNow;

            // wait for tasks to complete
            while ((Interlocked.Read(ref TotalNrOfIncomingSockets) > 0) && (DateTime.UtcNow - start < TimeSpan.FromSeconds(2)))
            {
                // yield
                Thread.Sleep(10);
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogProgramFlowInfoMessage(() =>
            {
                return "Listener stopped";
            });
#endif
            #endregion ----------------------
        }
        public void ActivateNewConfiguration(ISocketListenerConfiguration NewConfiguration)
        {
            lock (SocketListeners)
            {
                var newPorts = NewConfiguration.ListeningPorts.ToList<int>();

                var currentPorts = (
                    from q in SocketListeners
                    select ((IPEndPoint)q.LocalEndPoint).Port
                ).ToList<int>();

                var addedPorts = (
                    from q in newPorts
                    where currentPorts.IndexOf(q) < 0
                    select q
                ).ToList<int>();

                var removedPorts = (
                    from q in currentPorts
                    where newPorts.IndexOf(q) < 0
                    select q
                ).ToList<int>();

                var removedSockets = (
                    from q in SocketListeners
                    where removedPorts.IndexOf(((IPEndPoint)q.LocalEndPoint).Port) >= 0
                    select q
                ).ToArray<Socket>();

                foreach (var removedSocket in removedSockets)
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogProgramFlowInfoMessage(() =>
                    {
                        return String.Format("New listening configuration removed listening port {0}, listener stopped",
                            ((IPEndPoint)removedSocket.LocalEndPoint).Port);
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    try
                    {
                        removedSocket.Dispose();
                    }
                    catch { }

                    SocketListeners.Remove(removedSocket);
                }

                foreach (var newPort in addedPorts)
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogProgramFlowInfoMessage(() =>
                    {
                        return String.Format("New listening configuration added listening port {0}, listener starting..",
                            newPort);
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    try
                    {
                        var newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        newSocket.ExclusiveAddressUse = false;
                        newSocket.Bind(CreateEndPoint(newPort));

                        // Start listening
                        newSocket.Listen(newPort);

                        SocketListeners.Add(newSocket);

                        StartAcceptSockets(newSocket);

                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogProgramFlowInfoMessage(() =>
                        {
                            return "..listener started successfully";
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------
                    }
#if (Logging)
                    catch (Exception e)
#else
                    catch
#endif
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogException(e, String.Format("..could not bind to port {0}", newPort));
#endif
                        #endregion ------------------------------------------------------------------------------------------------
                    }
                }

                ListenerStarted = true;
                Started = true;
            }
        }

        // disposes the proxy server
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Started)
                    Stop();

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogProgramFlowInfoMessage(() =>
                {
                    return "Starting server disposal";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                #region Sockets

                if (SocketListeners != null)
                {
                    // listener
                    foreach (var socket in SocketListeners)
                    {
                        socket.Dispose();
                    }

                    SocketListeners.Clear();
                    SocketListeners = null;
                }

                if (ListenerPool != null)
                {
                    // Pool of reusable SocketAsyncEventArgs objects for accept operations
                    ListenerPool.Dispose();
                    ListenerPool = null;
                }

                if (HandlerPool != null)
                {
                    // Pool of reusable SocketAsyncEventArgs objects for socket operations
                    HandlerPool.Dispose();
                    HandlerPool = null;
                }

                #endregion

                #region Memory

                if (MemoryManager != null)
                {
                    // Reserve large chunk of memory and reuse this without reallocations during runtime
                    MemoryManager.Dispose();
                    MemoryManager = null;
                }
                #endregion
            }
        }

        #endregion

        #region Listening

        void StartAcceptSockets(Socket socket)
        {
            lock (SocketListeners)
            {
                if (!Started || (SocketListeners.IndexOf(socket) < 0))
                    return;
            }

            // protect against to many connections
            MaxIncomingSocketsEnforcer.WaitOne();

            if (!Started)
            {
                MaxIncomingSocketsEnforcer.Release();
                return;
            }

            // get saea

            if (ListenerPool.Pop(out SocketAsyncEventArgs acceptEventArg))
            {
                // start accept
                if (!socket.AcceptAsync(acceptEventArg))
                {
                    HandleAccept(socket, acceptEventArg);
                }
            }
            else
            {
                // should stop the appdomain
                throw new Exception("Enforcer failure");
            }
        }


        void HandleBadAccept(SocketAsyncEventArgs AcceptEventArgs)
        {
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(AcceptEventArgs, () =>
            {
                return "Handling bad accept operation";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            //This method closes the socket and releases all resources, both
            //managed and unmanaged. It internally calls Dispose
            try
            {
                if (AcceptEventArgs.AcceptSocket != null)
                {
                    AcceptEventArgs.AcceptSocket.LingerState.Enabled = false;
                    AcceptEventArgs.AcceptSocket.Close();
                }
            }
            catch { }

            // dereference
            AcceptEventArgs.AcceptSocket = null;

            //Put the saea back in the pool
            ListenerPool.Push(AcceptEventArgs);

            //Release Semaphore so that its connection counter will be decremented.
            //This must be done AFTER putting the SocketAsyncEventArg back into the pool,
            //or you can run into problems.
            MaxIncomingSocketsEnforcer.Release();
        }

        #endregion

        #region Handling

        void OnListenerSocketIOCompleted(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Accept:

                    HandleAccept((Socket)sender, saea);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        void HandleAccept(Socket socket, SocketAsyncEventArgs AcceptEventArgs)
        {
            if (!Started) return;

            lock (SocketListeners)
            {
                if (SocketListeners.IndexOf(socket) < 0)
                    return;
            }

            // Not successfull?
            if (AcceptEventArgs.SocketError != SocketError.Success)
            {
                //Let's destroy this socket, since it could be bad.
                HandleBadAccept(AcceptEventArgs);

                // Start another accept operation
                StartAcceptSockets(socket);
                return;
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogProgramFlowInfoMessage(() =>
            {
                return String.Format(
                    "New connection on listing port {0} from remote address {1}:{2}",
                    ((IPEndPoint)socket.LocalEndPoint).Port,
                    ((IPEndPoint)AcceptEventArgs.AcceptSocket.RemoteEndPoint).Address.ToString(),
                    ((IPEndPoint)AcceptEventArgs.AcceptSocket.RemoteEndPoint).Port
                    );
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------


            // Get an operation saea

            if (HandlerPool.Pop(out SocketHandlerBase operationHandler))
            {
                // Update statistics
                Interlocked.Increment(ref TotalNrOfIncomingSockets);
#if (Logging)
                Interlocked.Increment(ref TotalNrOfIncomingSocketsAccepted);
#endif

                // Assign socket to it
                operationHandler.CreateSaea();
                operationHandler.saeaHandler.AcceptSocket = AcceptEventArgs.AcceptSocket;

                // Reset accept socket
                AcceptEventArgs.AcceptSocket = null;

                // Put the accept socket back in the pool
                ListenerPool.Push(AcceptEventArgs);

                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                try
                {
                    Logger.LogSocketFlowMessage(AcceptEventArgs, () =>
                    {
                        return "New connection on port " +
                            ((IPEndPoint)operationHandler.saeaHandler.AcceptSocket.LocalEndPoint).Port.ToString() + " from " +
                            ((IPEndPoint)operationHandler.saeaHandler.AcceptSocket.RemoteEndPoint).Address.ToString() + ":" +
                            ((IPEndPoint)operationHandler.saeaHandler.AcceptSocket.RemoteEndPoint).Port.ToString();
                    });
                }
                catch { }
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // Start handling this socket
                ThreadPool.QueueUserWorkItem(operationHandler.StartHandlingSocket);

                // Start another accept operation
                StartAcceptSockets(socket);
            }
            else
            {
                // should stop the appdomain
                throw new Exception("Enforcer failure");
            }
        }

        void OnRemoveSocket(object sender, HandlerClosedEventArgs e)
        {
            SocketHandlerBase handler = e.handler;

            // Update statistics
            Interlocked.Decrement(ref TotalNrOfIncomingSockets);
#if (Logging)
            Interlocked.Increment(ref TotalNrOfIncomingSocketsClosed);
#endif

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(handler.saeaHandler, () =>
            {
                return "Socket closed";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            // Put the SocketAsyncEventArg back into the pool,
            // to be used by another client. 
            HandlerPool.Push(handler);

            //Release Semaphore so that its connection counter will be decremented.
            //This must be done AFTER putting the SocketAsyncEventArg back into the pool,
            //or you can run into problems.
            MaxIncomingSocketsEnforcer.Release();
        }

        #endregion

        #region Performance logging

        void ListenerTimerCallback(object state)
        {
            this.ActivateNewConfiguration(this.ListenerConfiguration);

#if (Logging)
            Logger.LogPerformanceStatusMessage(() =>
            {
                return String.Format("Pool of {0} In {1} | ps InA {2} | InC {3} | M {4}",

                    typeof(HandlerType).Name.Replace("SocketHandler", "").Replace("ServerHandler", "").PadRight(20, ' '),
                    Interlocked.Read(ref TotalNrOfIncomingSockets).ToString().PadLeft(4, ' '),
                    (Convert.ToDouble(Interlocked.Read(ref TotalNrOfIncomingSocketsAccepted)) / (Timer.Duration - LastPerformanceMeasurementTimestamp)).ToString("0").PadLeft(4, ' '),
                    (Convert.ToDouble(Interlocked.Read(ref TotalNrOfIncomingSocketsClosed)) / (Timer.Duration - LastPerformanceMeasurementTimestamp)).ToString("0").PadLeft(4, ' '),
                    Interlocked.Read(ref ((ServiceMemoryManager)MemoryManager).mem).ToString()
                    );
            });

            this.LastPerformanceMeasurementTimestamp = Timer.Duration;
            Interlocked.Exchange(ref this.TotalNrOfIncomingSocketsAccepted, 0);
            Interlocked.Exchange(ref this.TotalNrOfIncomingSocketsClosed, 0);
#endif
        }

        #endregion
    }
}
