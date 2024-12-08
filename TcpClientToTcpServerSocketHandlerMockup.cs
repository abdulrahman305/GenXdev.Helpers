using GenXdev.AsyncSockets.Arguments;
using GenXdev.AsyncSockets.Handlers;
using GenXdev.AsyncSockets.Logging;
using Ninject;
using System.Collections.Concurrent;

namespace GenXdev.AsyncSockets.Mocking
{

    public delegate void OnInitializeCallback<ServerHandlerType, ClientHandlerType>(ServerHandlerType ServerHandler, ClientHandlerType ClientHandler);

    public class TcpClientToTcpServerSocketHandlerMockup<ServerHandlerType, ClientHandlerType> : IDisposable
        where ServerHandlerType : SocketHandlerBase
        where ClientHandlerType : SocketHandlerBase
    {
        IKernel Kernel;
        IServiceLogger Logger;
        volatile bool ClientHandlerIsTimedOut;
        volatile bool ServerHandlerIsTimedOut;


        public ServerHandlerType ServerHandler { get; private set; }
        public ClientHandlerType ClientHandler { get; private set; }

        [Inject]
        public TcpClientToTcpServerSocketHandlerMockup(IKernel Kernel)
        {
            this.Kernel = Kernel;
            this.Logger = Kernel.Get<IServiceLogger>();
        }

        void ClientHandler_OnHandleException(object sender, OnHandleExceptionEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            if ((ClientHandlerIsTimedOut || ServerHandlerIsTimedOut) && (
                e.Exception.Message.ToLowerInvariant().Contains("dispose")))
                return;

            if ((ClientHandlerIsTimedOut || ServerHandlerIsTimedOut) && (
                e.Exception.Message.ToLowerInvariant().Contains("handler wants to switch state")))
                return;

            Exceptions.Enqueue(e.Exception);
        }

        void ServerHandler_OnHandleException(object sender, OnHandleExceptionEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            if ((ClientHandlerIsTimedOut || ServerHandlerIsTimedOut) && (
                e.Exception.Message.ToLowerInvariant().Contains("dispose")))
                return;

            if ((ClientHandlerIsTimedOut || ServerHandlerIsTimedOut) && (
                e.Exception.Message.ToLowerInvariant().Contains("handler wants to switch state")))
                return;

            Logger.LogException(e.Exception, "Exception: " + e.Exception.Message);
            Exceptions.Enqueue(e.Exception);
        }

        object closeLock = new object();
        void ClientHandler_OnHandlerClosed(object sender, Containers.HandlerClosedEventArgs e)
        {
            lock (closeLock)
            {
                Logger.LogProgramFlowInfoMessage(() =>
                {
                    return "ClientHandler closed";
                });
                ClientHandlerClosed = true;
                CheckForContinue();
            }
        }

        void ServerHandler_OnHandlerClosed(object sender, Containers.HandlerClosedEventArgs e)
        {
            lock (closeLock)
            {
                Logger.LogProgramFlowInfoMessage(() =>
                {
                    return "ServerHandler closed";
                });
                ServerHandlerClosed = true;
                CheckForContinue();
            }
        }

        void CheckForContinue()
        {
            if (ServerHandlerClosed && ClientHandlerClosed)
            {
                ServerHandlerClosed = false;
                ClientHandlerClosed = false;
                ConnectionsCompleted++;

                if (ConnectionsCompleted < ConnectionsToComplete)
                {
                    Logger.LogProgramFlowInfoMessage(() =>
                    {
                        return "CheckForContinue: ConnectionsCompleted < ConnectionsToComplete, " +
                            ConnectionsCompleted.ToString() + " < " + ConnectionsToComplete.ToString();
                    });

                    ThreadPool.QueueUserWorkItem(ServerHandler.StartHandlingSocket);
                }
            }
        }

        #region Fields

        volatile bool ClientHandlerClosed;
        volatile bool ServerHandlerClosed;
        volatile int ConnectionsCompleted;
        volatile int ConnectionsToComplete;
        ConcurrentQueue<Exception> Exceptions = new ConcurrentQueue<Exception>();

        #endregion

        public TcpClientToTcpServerSocketHandlerMockup<ServerHandlerType, ClientHandlerType> Execute(int ConnectionsToComplete = 1, OnInitializeCallback<ServerHandlerType, ClientHandlerType> InitializeCallback = null)
        {
            ClientHandlerIsTimedOut = false;
            ServerHandlerIsTimedOut = false;

            lock (this)
            {
                if (ConnectionsToComplete < 1)
                    return this;

                this.ConnectionsToComplete = ConnectionsToComplete;
                try
                {
                    // create socket IO handlers
                    var ServerSocketIOHandler = new SocketIOMockHandler();
                    var ClientSocketIOHandler = new SocketIOMockHandler() { Remote = ServerSocketIOHandler };

                    // create handlers
                    this.ServerHandler = Kernel.Get<ServerHandlerType>();
                    this.ClientHandler = Kernel.Get<ClientHandlerType>();

                    //  initialize io handlers
                    ServerSocketIOHandler.Initialize(Kernel);
                    ClientSocketIOHandler.Initialize(Kernel, ServerSocketIOHandler,
                        null,
                        false,
                        (object sender, EventArgs e) =>
                        {
                            ServerHandler.StartHandlingSocket(null);
                        }
                    );

                    // assign our own handlers
                    this.ServerHandler.OnHandlerClosed += ServerHandler_OnHandlerClosed;
                    this.ServerHandler.OnHandleException += ServerHandler_OnHandleException;
                    this.ClientHandler.OnHandlerClosed += ClientHandler_OnHandlerClosed;
                    this.ClientHandler.OnHandleException += ClientHandler_OnHandleException;

                    // initialize handlers
                    this.ServerHandler.CreateSaea();
                    ServerSocketIOHandler.SetSaea(ServerHandler.saeaHandler);
                    this.ClientHandler.CreateSaea();
                    ClientSocketIOHandler.SetSaea(ClientHandler.saeaHandler);

                    this.ServerHandler.OnDequeueHandler();
                    this.ClientHandler.OnDequeueHandler();

                    // assign new mock io handlers
                    this.ServerHandler.SocketIO = ServerSocketIOHandler;
                    this.ServerHandler.RegisterHandler(this.ServerHandler.saeaHandler);
                    this.ClientHandler.SocketIO = ClientSocketIOHandler;
                    this.ClientHandler.RegisterHandler(this.ClientHandler.saeaHandler);

                    // allow unit-test to do it's initialization
                    if (InitializeCallback != null)
                    {
                        InitializeCallback(this.ServerHandler, this.ClientHandler);
                    }

                    // reset
                    ConnectionsCompleted = 0;
                    ServerHandlerClosed = false;
                    ClientHandlerClosed = false;

                    // start 
                    System.Threading.ThreadPool.QueueUserWorkItem((state) =>
                    {
                        ClientHandler.StartHandlingSocket(null);
                    });

                    // 
                    while (ConnectionsCompleted < this.ConnectionsToComplete)
                    {
                        if ((ServerHandler.IsTimedOut) || (ClientHandler.IsTimedOut))
                        {
                            Kernel.Get<IServiceLogger>().Stop();
                            Kernel.Get<IServiceLogger>().Start();
                            Logger.LogProgramFlowInfoMessage(() =>
                            {
                                return ClientHandler.GetType().Name + ", timeout expired!";
                            });

                            ClientHandlerIsTimedOut = true;
                            ClientHandler.Close();
                            ServerHandlerIsTimedOut = true;
                            ServerHandler.Close();
                        }

                        System.Threading.Thread.Sleep(100);
                    }

                    if (ConnectionsCompleted > this.ConnectionsToComplete)
                    {
                        throw new InvalidOperationException("ConnectionsCompleted > ConnectionsToComplete");
                    }

                    if (Exceptions.TryDequeue(out Exception exc))
                        throw exc;
                }
                finally
                {
                    Logger.LogProgramFlowInfoMessage(() =>
                    {
                        return "stopping!";
                    });
                    Kernel.Get<IServiceLogger>().Stop();

                    DisposeHandlers();
                }

                return this;
            }
        }

        #region Disposing

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeHandlers();
            }
        }

        private void DisposeHandlers()
        {
            while (Exceptions.TryDequeue(out Exception e)) { };

            if (ServerHandler != null)
            {
                ServerHandler.OnHandlerClosed -= ServerHandler_OnHandlerClosed;
                ServerHandler.OnHandleException -= ServerHandler_OnHandleException;

                ServerHandler.Dispose();
                ServerHandler = null;
            }

            if (ClientHandler != null)
            {
                ClientHandler.OnHandlerClosed -= ClientHandler_OnHandlerClosed;
                ClientHandler.OnHandleException -= ClientHandler_OnHandleException;

                ClientHandler.Dispose();
                ClientHandler = null;
            }
        }

        ~TcpClientToTcpServerSocketHandlerMockup()
        {
            Dispose(false);
        }

        #endregion
    }
}
