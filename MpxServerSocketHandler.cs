using Ninject;
using GenXdev.MemoryManagement;
using GenXdev.Configuration;
using GenXdev.Buffers;
using GenXdev.AsyncSockets.Containers;
using GenXdev.Helpers;
using GenXdev.AsyncSockets.Enums;
using GenXdev.AsyncSockets.Arguments;

namespace GenXdev.AsyncSockets.Handlers
{
    public class MpxServerSocketHandler<ChannelDataType> : SocketHandlerBase where ChannelDataType : MPXChannelData
    {
        public enum MagicLengthPack
        {
            ChannelRequest = -6,
            ChannelAdded = -4,
            ChannelRemoved = -5,
            ChannelYield = -7,
            Ping = -2,
            Pong = -3
        };

        #region Initialization

        [Inject]
        public MpxServerSocketHandler(
            IKernel Kernel
            , IMemoryManagerConfiguration MemoryConfiguration
            , IServiceMemoryManager MemoryManager
            , String ServiceName

#if (Logging)
, IServiceLogger Logger
#endif
, object Pool = null
            , byte[] ProtocolId = null
            , Version MinimumRequiredClientVersion = null
            , Version ImplementedServerVersion = null
)
            : base(
                  Kernel
                , MemoryConfiguration
                , MemoryManager
                , ServiceName
#if (Logging)
, Logger
#endif
, Pool
            )
        {
            this.MinimumRequiredProtocolVersion =
                MinimumRequiredClientVersion != null ? MinimumRequiredClientVersion : Version.Parse("1.0.0.0");

            this.ImplementedProtocolVersion =
                ImplementedServerVersion != null ? ImplementedServerVersion : Version.Parse("1.0.0.0");

            this.ProtocolId = (ProtocolId != null) ? ProtocolId : new byte[0];
            this.Channels = new MPXChannelDataDictionary<ChannelDataType>(Kernel);
            this.ControlPacketQueue = Kernel.Get<DynamicBuffer>();
        }


        protected override void SetupHandlers()
        {
            this.OnHandleInitializeAsync += MpxServerSocketHandler_OnHandleInitialize;
            this.OnHandleReceiveAsync += MpxServerSocketHandler_OnHandleReceive;
            this.OnHandleSendAsync += MpxServerSocketHandler_OnHandleSend;
            this.OnHandlePollAsync += MpxServerSocketHandler_OnHandlePoll;
            this.OnHandleReset += MpxServerSocketHandler_OnHandleReset;
        }

        #endregion

        #region Properties

        public bool keepAlive { get; set; }

        protected bool allowAnonymousChannels { get; set; } = true;

        public MPXChannelDataDictionary<ChannelDataType> Channels
        {
            get;
            private set;
        }

        #endregion

        #region Fields

        static byte[] SofwareId = new byte[] { 6, 2, 20, 19 };
        int ReceivedBytesSinceLastSent;
        byte[] ProtocolId;

        Version MinimumRequiredProtocolVersion;
        Version ImplementedProtocolVersion;
        Version ClientVersion;
        byte[] ClientTypeAuthChallenge;
        MpxServerSocketHandlerState State;
        DynamicBuffer ControlPacketQueue;
        private ClientTypeInfoEventArgs _ClientTypeInfoEventArgs;
        private OnMultiplexedChannelEventArgs _OnMultiplexedChannelEventArgs;
        private VersionMismatchEventArgs _VersionMismatchEventArgs;
        private AuthenticateClientEventArgs _AuthenticateClientEventArgs;
        private HandleMpxSocketIdleEventArgs<ChannelDataType> _HandleMpxSocketIdleEventArgs;
        private MpxServerSocketNewStateEventArgs _MpxServerSocketNewStateEventArgs;


        #endregion

        #region Events

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, EventArgs, Task> OnMultiplexedOutgoingPing;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, EventArgs, Task> OnMultiplexedIncomingPing;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, EventArgs, Task> OnMultiplexedOutgoingPong;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, EventArgs, Task> OnMultiplexedIncomingPong;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, ClientTypeInfoEventArgs, Task> OnMultiplexedInfoProtocolHandshakeResult;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, AuthenticateClientEventArgs, Task> OnMultiplexedHandleAuthenticateClient;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, VersionMismatchEventArgs, Task> OnMultiplexedInfoProtocolVersions;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, MpxServerSocketNewStateEventArgs, Task> OnMultiplexedInfoNewState;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, OnMultiplexedChannelEventArgs, Task> OnMultiplexedInfoChannelRemoved;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, ChannelDataType, Task> OnMultiplexedInfoChannelAdded;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, ChannelDataType, Task> OnMultiplexedInfoChannelYieldSet;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, ChannelDataType, Task> OnMultiplexedHandleReceive;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, ChannelDataType, Task> OnMultiplexedHandleSend;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, HandleMpxSocketIdleEventArgs<ChannelDataType>, Task> OnMultiplexedHandleIdle;

        #endregion

        #region Protocol

        #region Authentication

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_IdentifyingClientType(object sender, HandleSocketBEventArgs e)
        {
            // not enough received yet?
            if (e.RxBuffer.Count < 32)
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString() + " bytes, need 32";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                e.NextAction = NextRequestedHandlerAction.Receive;
                e.Count = Math.Max(Math.Max(1, NrOfBytesSocketHasAvailable), 32 - e.RxBuffer.Count);
                return true;
            }

            // what is expected?
            var expected = GenXdev.Helpers.Hash.GetSha256Bytes(SofwareId, ProtocolId, SslLocalCertificateHash, SslRemoteCertificateHash);

            e.RxBuffer.Position = 0;
            // everything ok?
            if (e.RxBuffer.IndexOf(expected) == 0)
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(saeaHandler, () =>
                {
                    return "Client has successfully responsed with the right protocol type challenge response => " +
                            GenXdev.Helpers.Hash.FormatBytesAsHexString(expected);
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // prevent timeout
                SetActivityTimestamp();

                // set timeout
                SetCurrentStageTimeoutSeconds(15, false);

                // fire event
                var successHandler = OnMultiplexedInfoProtocolHandshakeResult;

                if (successHandler != null)
                {
                    if (this._ClientTypeInfoEventArgs == null)
                    {
                        this._ClientTypeInfoEventArgs = new ClientTypeInfoEventArgs();
                    }

                    this._ClientTypeInfoEventArgs.Successfull = true;

                    await successHandler(this, this._ClientTypeInfoEventArgs);
                }

                // clean-up
                e.RxBuffer.Remove(expected.Length);

                // set new state and log
                await SetState(MpxServerSocketHandlerState.ReceivingClientProtocolVersion);

                // process new state, there's probaply more in the inputbuffer
                return await MpxServerSocketHandler_OnHandleReceive_ReceivingClientProtocolVersion(sender, e);
            }

            // fire event
            var failureHandler = OnMultiplexedInfoProtocolHandshakeResult;

            if (failureHandler != null)
            {
                if (this._ClientTypeInfoEventArgs == null)
                {
                    this._ClientTypeInfoEventArgs = new ClientTypeInfoEventArgs();
                }

                this._ClientTypeInfoEventArgs.Successfull = false;

                await failureHandler(this, this._ClientTypeInfoEventArgs);
            }
            else
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(saeaHandler, () =>
                {
                    return "Handshake partial unsuccessfull, could be a man-in-the-middle attack, disposing socket";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------
            }

            // get rid of this socket
            e.NextAction = NextRequestedHandlerAction.Dispose;
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_ReceivingClientProtocolVersion(object sender, HandleSocketBEventArgs e)
        {
            // init

            // received our version string yet?
            if (BufferHelpers.TryGetStringFromBuffer(ref e.Count, e.RxBuffer, out string protocolVersion, 15))
            {
                // parse it
                ClientVersion = Version.Parse(protocolVersion);

                // is it ok?
                if (ClientVersion >= MinimumRequiredProtocolVersion)
                {
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogHandlerFlowMessage(saeaHandler, () =>
                        {
                            return "Received client version, so far ok, sending our version and success result";
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------
                    }

                    // set new state and log
                    await SetState(MpxServerSocketHandlerState.ReceivingClientCheckResult);

                    // send success result
                    e.TxBuffer.Add(1);
                }
                else
                {
                    // fire event
                    var failureHandler = OnMultiplexedInfoProtocolVersions;

                    if (failureHandler != null)
                    {
                        if (this._VersionMismatchEventArgs == null)
                        {
                            this._VersionMismatchEventArgs = new VersionMismatchEventArgs();
                        }

                        this._VersionMismatchEventArgs.LocalReportsError = true;
                        this._VersionMismatchEventArgs.RemoteReportedError = false;
                        this._VersionMismatchEventArgs.LocalVersion = ImplementedProtocolVersion;
                        this._VersionMismatchEventArgs.RemoteVersion = ClientVersion;
                        this._VersionMismatchEventArgs.MinimumVersionExpected = MinimumRequiredProtocolVersion;
                        this._VersionMismatchEventArgs.MinimumVersionRemoteExpects = null;

                        await failureHandler(this, this._VersionMismatchEventArgs);
                    }
                    else
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogHandlerFlowMessage(saeaHandler, () =>
                        {
                            return
                                String.Format(
                                    "Protocol version check FAILED, remote client has version {0}, this server supports version v{1} upto and included version {2}" +
                                    ", disconnecting after sending failure result",
                                    ClientVersion,
                                    MinimumRequiredProtocolVersion,
                                    ImplementedProtocolVersion
                                    );
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------
                    }

                    // set new state and log
                    await SetState(MpxServerSocketHandlerState.Disconnecting);

                    // send failure result
                    e.TxBuffer.Add(0);
                }

                // send our version
                BufferHelpers.WriteStringToBuffer(e.TxBuffer, ImplementedProtocolVersion.ToString());

                // there is probaply more in the inputbuffer..
                e.NextAction = NextRequestedHandlerAction.Send;
                return true;
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString() + " bytes";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            e.NextAction = NextRequestedHandlerAction.Receive;
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_ReceivingClientCheckResult(object sender, HandleSocketBEventArgs e)
        {
            // have enough?
            if (e.RxBuffer.Length > 0)
            {
                // get result
                bool result = e.RxBuffer[0] == 1;

                // remove result
                e.RxBuffer.Remove(1);

                if (result)
                {
                    // set new state and log
                    await SetState(MpxServerSocketHandlerState.IdentifyingClient);

                    // fire event
                    var successHandler = OnMultiplexedInfoProtocolVersions;

                    if (successHandler != null)
                    {
                        if (this._VersionMismatchEventArgs == null)
                        {
                            this._VersionMismatchEventArgs = new VersionMismatchEventArgs();
                        }

                        this._VersionMismatchEventArgs.LocalReportsError = false;
                        this._VersionMismatchEventArgs.RemoteReportedError = false;
                        this._VersionMismatchEventArgs.LocalVersion = ImplementedProtocolVersion;
                        this._VersionMismatchEventArgs.RemoteVersion = ClientVersion;
                        this._VersionMismatchEventArgs.MinimumVersionExpected = MinimumRequiredProtocolVersion;
                        this._VersionMismatchEventArgs.MinimumVersionRemoteExpects = null;

                        await successHandler(this, this._VersionMismatchEventArgs);
                    }
                    else
                    {
                        #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                        Logger.LogHandlerFlowMessage(saeaHandler, () =>
                        {
                            return "Handshake successfull, multiplexed protocol acknowledged" +
                                (this.SslStarted ? ", and both client and server certificate hashes compared successfully" : "") +
                                ", now sending success result";
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------
                    }
                }
                else
                {
                    // set new state and log
                    await SetState(MpxServerSocketHandlerState.ReceivingClientMinimumVersion);
                }

                // there is probaply more in the inputbuffer..
                await MpxServerSocketHandler_OnHandleReceive_AllStates(sender, e);
                return true;
            }

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return
                    "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            e.NextAction = NextRequestedHandlerAction.Receive;
            e.Count = Math.Max(1, NrOfBytesSocketHasAvailable);
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_ReceivingClientMinimumVersion(object sender, HandleSocketBEventArgs e)
        {
            // init

            // have enough?
            if (BufferHelpers.TryGetStringFromBuffer(ref e.Count, e.RxBuffer, out string clientVersionString, 15))
            {
                // parse it
                var clientMinimumVersion = Version.Parse(clientVersionString);

                // set new state and log
                await SetState(MpxServerSocketHandlerState.Disconnecting);

                // fire event
                var handler = OnMultiplexedInfoProtocolVersions;

                if (handler != null)
                {
                    if (this._VersionMismatchEventArgs == null)
                    {
                        this._VersionMismatchEventArgs = new VersionMismatchEventArgs();
                    }

                    this._VersionMismatchEventArgs.LocalReportsError = false;
                    this._VersionMismatchEventArgs.RemoteReportedError = true;
                    this._VersionMismatchEventArgs.LocalVersion = ImplementedProtocolVersion;
                    this._VersionMismatchEventArgs.RemoteVersion = ClientVersion;
                    this._VersionMismatchEventArgs.MinimumVersionExpected = MinimumRequiredProtocolVersion;
                    this._VersionMismatchEventArgs.MinimumVersionRemoteExpects = clientMinimumVersion;

                    await handler(this, this._VersionMismatchEventArgs);
                }
                else
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogHandlerFlowMessage(saeaHandler, () =>
                    {
                        return String.Format(
                                "Protocol version check FAILED, remote client supports version v{0} upto and included version {1}, this server supports version v{2} upto and included version {3}",
                                clientMinimumVersion,
                                ClientVersion,
                                MinimumRequiredProtocolVersion,
                                ImplementedProtocolVersion
                                );
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------
                }

                e.NextAction = NextRequestedHandlerAction.Disconnect;
                return true;
            }


            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            e.NextAction = NextRequestedHandlerAction.Receive;
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_IdentifyingClient(object sender, HandleSocketBEventArgs e)
        {
            // continue receiving if not enough received yet
            if (!BufferHelpers.TryGetBytesFromBuffer(ref e.Count, e.RxBuffer, out byte[] clientAuthentication, 1024 * 1024))
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Not enough data in receivebuffer, receiving more, need 1, have: " + e.RxBuffer.Count.ToString();
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // init
                e.NextAction = NextRequestedHandlerAction.Receive;
                return true;
            }

            if (await HandleAuthenticateClient(clientAuthentication))
            {
                // set new state and log
                await SetState(MpxServerSocketHandlerState.Multiplexed);

                // send success result
                e.TxBuffer.Add(1);
                e.NextAction = NextRequestedHandlerAction.Send;

                return true;
            }

            // set new state and log
            await SetState(MpxServerSocketHandlerState.Disconnecting);

            // send failure result
            e.TxBuffer.Add(0);
            e.NextAction = NextRequestedHandlerAction.Send;
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_RetreivingChannelAdded(object sender, HandleSocketBEventArgs e, bool IsRequest)
        {
            // receive channel name
            if (BufferHelpers.TryGetStringFromBuffer(ref e.Count, e.RxBuffer, out string channelName, 100000))
            {
                // set new state
                await SetState(MpxServerSocketHandlerState.Multiplexed);

                // check
                if (!ChannelIsActive(channelName))
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogHandlerFlowMessage(saeaHandler, () =>
                    {
                        return "Channel added by peer: " + channelName;
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------            

                    // update config
                    var channelData = Channels[channelName];

                    if (IsRequest)
                    {
                        SendChannelAdded(channelName);
                    }

                    // fire event
                    var handler2 = OnMultiplexedInfoChannelAdded;

                    if (handler2 != null)
                    {
                        await handler2(this, channelData);
                    }
                }

                return false;
            }

            // receive more if necessary
            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------            

            if (IsRequest)
            {
                await SetState(MpxServerSocketHandlerState.RetreivingChannelRequested_MultiPlexed);
            }
            else
            {
                await SetState(MpxServerSocketHandlerState.RetreivingChannelAdded_MultiPlexed);
            }
            e.NextAction = NextRequestedHandlerAction.Receive;
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_RetreivingChannelRemoved(object sender, HandleSocketBEventArgs e)
        {
            // receive channel name
            if (BufferHelpers.TryGetStringFromBuffer(ref e.Count, e.RxBuffer, out string channelName, 100000))
            {
                // set new state
                await SetState(MpxServerSocketHandlerState.Multiplexed);

                // check
                if (ChannelIsActive(channelName))
                {

                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogHandlerFlowMessage(saeaHandler, () =>
                    {
                        return "Channel removed by peer: " + channelName;
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------            

                    // update config
                    Channels[channelName] = null;

                    // fire event
                    var handler2 = OnMultiplexedInfoChannelRemoved;

                    if (handler2 != null)
                    {
                        if (this._OnMultiplexedChannelEventArgs == null)
                        {
                            this._OnMultiplexedChannelEventArgs = new OnMultiplexedChannelEventArgs();
                        }

                        this._OnMultiplexedChannelEventArgs.Channel = channelName;

                        await handler2(this, this._OnMultiplexedChannelEventArgs);
                    }
                }

                return false;
            }

            // receive more if necessary

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString();
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------            

            await SetState(MpxServerSocketHandlerState.RetreivingChannelRemoved_MultiPlexed);
            e.NextAction = NextRequestedHandlerAction.Receive;
            return true;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_RetreivingChannelYield(object sender, HandleSocketBEventArgs e)
        {
            // receive channel name
            if (BufferHelpers.TryGetStringAndInt32FromBuffer(ref e.Count, e.RxBuffer, out string channelName, out int value, 100000))
            {
                // set new state
                await SetState(MpxServerSocketHandlerState.Multiplexed);

                // check
                if (ChannelIsActive(channelName))
                {
                    #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                    Logger.LogHandlerFlowMessage(saeaHandler, () =>
                    {
                        return "Channel yield set by peer, new value: " + value.ToString() + " on channel: " + channelName;
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------            

                    // reference
                    var channelData = Channels[channelName];

                    // update config
                    if (value < 0)
                    {
                        // todo temp
                        // Console.WriteLine("Channel continued: " + channelData.ChannelName);

                        channelData.ChannelYieldSetByPeer = false;
                    }
                    else
                    {
                        // todo temp
                        // Console.WriteLine("Channel yielded: " + channelData.ChannelName);

                        channelData.ChannelYieldSetByPeer = true;
                    }

                    // fire event
                    var handler2 = OnMultiplexedInfoChannelYieldSet;

                    if (handler2 != null)
                    {
                        await handler2(this, channelData);
                    }
                }

                return false;
            }

            // receive more if necessary

            #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Not enough data in receivebuffer, receiving more, have: " + e.RxBuffer.Count.ToString();
                });
#endif
            #endregion ------------------------------------------------------------------------------------------------            

            await SetState(MpxServerSocketHandlerState.RetreivingChannelYield_MultiPlexed);
            e.NextAction = NextRequestedHandlerAction.Receive;
            return true;
        }

        async Task<bool> HandleAuthenticateClient(byte[] clientAuthentication)
        {
            var handler = OnMultiplexedHandleAuthenticateClient;

            if (handler != null)
            {
                if (this._AuthenticateClientEventArgs == null)
                {
                    this._AuthenticateClientEventArgs = new AuthenticateClientEventArgs();
                }

                this._AuthenticateClientEventArgs.ClientAuthentication = clientAuthentication;
                this._AuthenticateClientEventArgs.Handled = false;
                this._AuthenticateClientEventArgs.Result = false;

                await handler(this, this._AuthenticateClientEventArgs);

                if (this._AuthenticateClientEventArgs.Handled)
                {
                    return this._AuthenticateClientEventArgs.Result;
                }
            }

            return true;
        }

        #endregion

        #region Multiplexed

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_Multiplexed(object sender, HandleSocketBEventArgs e)
        {
            // init

            // check for magic packets
            if (e.RxBuffer.Length >= sizeof(int))
            {
                // receive length integer of next packet
                e.RxBuffer.Position = 0;
                var packetLength = e.RxBuffer.Reader.ReadInt32();

                // is it magic?
                if (packetLength < 0)
                {
                    // remove it
                    e.RxBuffer.Remove(sizeof(int));

                    // delegate handling
                    if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed_MagicPacket(sender, e, packetLength))
                        return true;
                }
            }
            else if (e.RxBuffer.Count > 0)
            {
                e.Count = Math.Max(Math.Max(1, NrOfBytesSocketHasAvailable), sizeof(int) - e.RxBuffer.Count);
                e.NextAction = NextRequestedHandlerAction.Receive;
                return true;
            }

            // receive packets
            // todo use dynamic buffer for these large chunks also in serverhandlers
            while (BufferHelpers.TryGetBytesFromBuffer(ref e.Count, e.RxBuffer, out byte[] packet, out string channelName, MemoryConfiguration.DynamicBufferFragmentSize - 25, 100))
            {
                // init
                ChannelDataType channelData;

                // reference eventhandler
                var handler = OnMultiplexedHandleReceive;

                // are we interested in this packet?
                if (handler != null && (allowAnonymousChannels || ChannelIsActive(channelName)))
                {
                    // get channeldata
                    channelData = Channels[channelName];

                    // add packet to receivebuffer
                    channelData.RxBuffer.Add(packet);

                    await handler(this, channelData);

                    // buffers overflow? (client did not listen to yield message)
                    if (channelData.RxBuffer.Length > MemoryConfiguration.SendBufferSize * MPXChannelData.MinimumFactorBeforeDisconnecting)
                    {
                        // disconnect client
                        throw new InvalidOperationException(
                            "Buffers overflow for channel \"" +
                            channelData.ChannelName +
                            "\" Client did not listen to yield message"
                        );
                    }

                    // buffers running full?
                    if (channelData.RxBuffer.Length > MemoryConfiguration.SendBufferSize * MPXChannelData.MinimumFactorBeforeSendingChannelYield &&
                        !channelData.ChannelYieldSend)
                    {
                        // remember
                        channelData.ChannelYieldSend = true;

                        // send request
                        SendChannelYield(channelData.ChannelName, true);
                    }
                }
                else
                {
                    #region ------------------------------------------------------------------------------------------------
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () =>
                    {
                        return "Server has not subscribed to channel " + channelName.ToString();
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------
                }


                // check for magic packets
                if (e.RxBuffer.Length >= sizeof(int))
                {
                    // receive length integer of next packet
                    e.RxBuffer.Position = 0;
                    var packetLength = e.RxBuffer.Reader.ReadInt32();

                    // is it magic?
                    if (packetLength < 0)
                    {
                        // remove it
                        e.RxBuffer.Remove(sizeof(int));

                        // delegate handling
                        if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed_MagicPacket(sender, e, packetLength))
                            return true;
                    }
                }
            }

            if (SendControlPackets(sender, e))
            {
                return true;
            }

            if (e.RxBuffer.Count > 0 && e.Count.HasValue)
            {
                e.NextAction = NextRequestedHandlerAction.Receive;
                return true;
            }

#if (UnitTests)
            // if there is something to send, do it now
            if (SendMpxBuffers(sender, e))
            {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () => {
                return  "Channel handlers have produced output, starting sending, txBuffer size: " + e.TxBuffer.Count.ToString();});
#endif
            #endregion ------------------------------------------------------------------------------------------------
                return;
            }
#endif


            // first perform basic pending tasks
            if (await MpxServerSocketHandler_OnHandlePoll_DefaultActions(sender, e))
                return true;


            return false;
        }

        async Task<bool> MpxServerSocketHandler_OnHandleReceive_Multiplexed_MagicPacket(object sender, HandleSocketBEventArgs e, int controlPacketId)
        {
#if (Logging)
            string name = "";
            try
            {
                name = Enum.GetName(typeof(MagicLengthPack), controlPacketId);
            }
            catch
            {
                name = "unknown";
            }

            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Magic packet received of type \"" + controlPacketId.ToString() + " = " + name + "\"";
            });
#endif
            // let's see which one
            switch (controlPacketId)
            {
                // client wants to add channel?
                case (int)MagicLengthPack.ChannelAdded:

                    // start receiving it                    
                    return await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelAdded(sender, e, false);

                // client wants to add channel?
                case (int)MagicLengthPack.ChannelRequest:

                    // start receiving it                    
                    return await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelAdded(sender, e, true);

                // client wants to remove a channel?
                case (int)MagicLengthPack.ChannelRemoved:

                    // start receiving it                    
                    return await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelRemoved(sender, e);

                // client wants to set upload yield for a channel?
                case (int)MagicLengthPack.ChannelYield:

                    // start receiving it                    
                    return await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelYield(sender, e);

                case (int)MagicLengthPack.Ping:

                    var handler = OnMultiplexedIncomingPing;

                    if (handler != null)
                    {
                        await handler(this, EventArgs.Empty);
                    }

                    var handler2 = OnMultiplexedOutgoingPong;

                    if (handler2 != null)
                    {
                        await handler2(this, EventArgs.Empty);
                    }

                    // write magic packet length
                    e.TxBuffer.Writer.Write((int)MagicLengthPack.Pong);
                    e.TxBuffer.Writer.Flush();
                    e.NextAction = NextRequestedHandlerAction.Send;
                    return true;

                case (int)MagicLengthPack.Pong:

                    var handler3 = OnMultiplexedIncomingPong;

                    if (handler3 != null)
                    {
                        await handler3(this, EventArgs.Empty);
                    }

                    SetActivityTimestamp(false);
                    SetCurrentStageTimeoutSeconds(15, false);
                    return false;
            }

            throw new InvalidOperationException("Unknown control packet Id");
        }

        async Task MpxServerSocketHandler_OnHandleSend_Multiplexed(object sender, HandleSocketBEventArgs e)
        {
            await MpxServerSocketHandler_OnHandlePoll(sender, e);
        }

        private async Task MpxServerSocketHandler_OnFillTxBuffers(HandleSocketBEventArgs e)
        {
            // reference eventhandler
            var handler = OnMultiplexedHandleSend;

            // handler assigned?
            if (handler != null)
            {
                // get all active channels that have almost empty buffers
                var channels = (

                        from q in Channels.All

                        where
                            (q.TxBuffer.Count < MemoryConfiguration.DynamicBufferFragmentSize - 25)

                        select q
                    ).ToList();

                foreach (var channelData in channels)
                {
                    #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                    Logger.LogSocketFlowMessage(saeaHandler, () =>
                    {
                        return "=> Sendbuffer of channel " + channelData.ChannelName.ToString() +
                            " running empty, now contains " + channelData.TxBuffer.Count.ToString() + " bytes";
                    });
#endif
                    #endregion ------------------------------------------------------------------------------------------------

                    await handler(this, channelData);
                }
            }
        }

        async Task<bool> MpxServerSocketHandler_OnHandlePoll_DefaultActions(object sender, HandleSocketBEventArgs e)
        {
            // is there data expected to be received?
            if (SocketHasDataAvailable)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return
                        "Socket has more data available, starting receive..";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                if (!e.Count.HasValue)
                {
                    e.Count = Math.Max(1, NrOfBytesSocketHasAvailable);
                }
                e.NextAction = NextRequestedHandlerAction.Receive;
                return true;
            }


            if (this.keepAlive && IdleSeconds > 10)
            {
                SetActivityTimestamp();
                SetCurrentStageTimeoutSeconds(15, false);

                var handler = OnMultiplexedOutgoingPing;

                if (handler != null)
                {
                    await handler(this, EventArgs.Empty);
                }

                // write magic packet length
                e.TxBuffer.Writer.Write((int)MagicLengthPack.Ping);
                e.TxBuffer.Writer.Flush();
                e.NextAction = NextRequestedHandlerAction.Send;
                return true;
            }

            // something important to send right away?
            if (SendControlPackets(sender, e))
            {
                return true;
            }

            // is the send buffer becoming empty?
            await MpxServerSocketHandler_OnFillTxBuffers(e);
            if (SendMpxBuffers(sender, e))
                return true;

            // is there something to send?
            if (e.TxBuffer.Count > 0)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Sendbuffer not empty yet, starting sending..";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------
                e.NextAction = NextRequestedHandlerAction.Send;
                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region Handling

        async Task MpxServerSocketHandler_OnHandleInitialize(object sender, HandleSocketBEventArgs e)
        {
            // set poll interval
            SetPollIntervalMs(5);

            // how long to allow ssl to be estblished?
            SetActivityTimestamp();
            SetCurrentStageTimeoutSeconds(10, false, Timer.Duration);

            // init
            await SetState(MpxServerSocketHandlerState.IdentifyingClientType);

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogHandlerFlowMessage(saeaHandler, () =>
            {
                return "Sending 256 bit authentication challenge";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            // secure transport
            e.NextAction = NextRequestedHandlerAction.AsyncAction;
        }

        protected override void AfterHandleStartTransportSecurity(object sender, HandleSocketBEventArgs e)
        {
            // setup identification challenge
            ClientTypeAuthChallenge = GenXdev.Helpers.Hash.GetSha256Bytes(SofwareId, ProtocolId, SslLocalCertificateHash);

            // send clienttype identification challenge
            e.TxBuffer.Add(ClientTypeAuthChallenge);

            // set timeouts
            SetActivityTimestamp();
            SetCurrentStageTimeoutSeconds(5, false);

            // send it
            e.NextAction = NextRequestedHandlerAction.Send;
        }

        async Task MpxServerSocketHandler_OnHandleReceive(object sender, HandleSocketBEventArgs e)
        {
            // prevent timeout
            SetActivityTimestamp();

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Receivebuffer now contains " + e.RxBuffer.Count.ToString() + " bytes";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            await MpxServerSocketHandler_OnHandleReceive_AllStates(sender, e);
        }

        async Task MpxServerSocketHandler_OnHandleReceive_AllStates(object sender, HandleSocketBEventArgs e)
        {
            var count = e.RxBuffer.Count;
            try
            {
                switch (State)
                {
                    case MpxServerSocketHandlerState.IdentifyingClientType:
                        if (await MpxServerSocketHandler_OnHandleReceive_IdentifyingClientType(sender, e))
                        {
                            return;
                        }
                        break;

                    case MpxServerSocketHandlerState.ReceivingClientProtocolVersion:
                        if (await MpxServerSocketHandler_OnHandleReceive_ReceivingClientProtocolVersion(sender, e))
                        {
                            return;
                        }
                        break;

                    case MpxServerSocketHandlerState.ReceivingClientCheckResult:
                        if (await MpxServerSocketHandler_OnHandleReceive_ReceivingClientCheckResult(sender, e))
                        {
                            return;
                        }
                        break;

                    case MpxServerSocketHandlerState.ReceivingClientMinimumVersion:
                        if (await MpxServerSocketHandler_OnHandleReceive_ReceivingClientMinimumVersion(sender, e))
                        {
                            return;
                        }
                        break;

                    case MpxServerSocketHandlerState.IdentifyingClient:
                        if (await MpxServerSocketHandler_OnHandleReceive_IdentifyingClient(sender, e))
                        {
                            return;
                        }
                        break;

                    case MpxServerSocketHandlerState.RetreivingChannelAdded_MultiPlexed:
                        if (await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelAdded(sender, e, false))
                        {
                            return;
                        }

                        if (e.RxBuffer.Count > 0 && State == MpxServerSocketHandlerState.Multiplexed)
                        {
                            if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed(sender, e))
                            {
                                return;
                            }
                        }

                        break;

                    case MpxServerSocketHandlerState.RetreivingChannelRequested_MultiPlexed:
                        if (await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelAdded(sender, e, true))
                        {
                            return;
                        }

                        if (e.RxBuffer.Count > 0 && State == MpxServerSocketHandlerState.Multiplexed)
                        {
                            if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed(sender, e))
                            {
                                return;
                            }
                        }

                        break;

                    case MpxServerSocketHandlerState.RetreivingChannelRemoved_MultiPlexed:
                        if (await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelRemoved(sender, e))
                        {
                            return;
                        }

                        if (e.RxBuffer.Count > 0 && State == MpxServerSocketHandlerState.Multiplexed)
                        {
                            if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed(sender, e))
                            {
                                return;
                            }
                        }
                        break;

                    case MpxServerSocketHandlerState.RetreivingChannelYield_MultiPlexed:
                        if (await MpxServerSocketHandler_OnHandleReceive_RetreivingChannelYield(sender, e))
                        {
                            return;
                        }

                        if (e.RxBuffer.Count > 0 && State == MpxServerSocketHandlerState.Multiplexed)
                        {
                            if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed(sender, e))
                            {
                                return;
                            }
                        }
                        break;

                    case MpxServerSocketHandlerState.Multiplexed:
                        if (await MpxServerSocketHandler_OnHandleReceive_Multiplexed(sender, e))
                        {
                            return;
                        }
                        break;
                }

                if (SocketHasDataAvailable)
                {
                    e.NextAction = NextRequestedHandlerAction.Receive;
                    e.Count = Math.Max(1, NrOfBytesSocketHasAvailable);
                    return;
                }

                WakeupOnSignalFrom(this, "WakeupSignal", e, true);
            }
            finally
            {
                ReceivedBytesSinceLastSent += count - e.RxBuffer.Count;
            }
        }

        async Task MpxServerSocketHandler_OnHandleSend(object sender, HandleSocketBEventArgs e)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Sendbuffer now contains " + e.TxBuffer.Count.ToString() + " bytes";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            ReceivedBytesSinceLastSent = 0;

            switch (State)
            {
                case MpxServerSocketHandlerState.Disconnecting:

                    // still more to sent?
                    if (e.TxBuffer.Count > 0)
                    {
                        // continue
                        e.NextAction = NextRequestedHandlerAction.Send;
                        return;
                    }

                    e.NextAction = NextRequestedHandlerAction.Disconnect;
                    return;

                case MpxServerSocketHandlerState.Multiplexed:

                    await MpxServerSocketHandler_OnHandleSend_Multiplexed(sender, e);
                    return;

                default:

                    // still more to sent?
                    if (e.TxBuffer.Count > 0)
                    {
                        // continue
                        e.NextAction = NextRequestedHandlerAction.Send;
                        return;
                    }

                    if (!e.Count.HasValue)
                    {
                        e.Count = Math.Max(1, NrOfBytesSocketHasAvailable);
                    }

                    e.NextAction = NextRequestedHandlerAction.Receive;
                    return;
            }
        }

        async Task MpxServerSocketHandler_OnHandlePoll(object sender, HandleSocketBEventArgs e)
        {
            // todo temp
            if (State < MpxServerSocketHandlerState.Multiplexed)
            {
                throw new InvalidOperationException();
            }

            // first perform basic pending tasks
            if (await MpxServerSocketHandler_OnHandlePoll_DefaultActions(sender, e))
                return;

            // alright, let's trigger the idle event
            var handler = OnMultiplexedHandleIdle;

            // assigned?
            if (handler != null)
            {
                if (this._HandleMpxSocketIdleEventArgs == null)
                {
                    this._HandleMpxSocketIdleEventArgs = new HandleMpxSocketIdleEventArgs<ChannelDataType>();
                }

                this._HandleMpxSocketIdleEventArgs.NextAction = HandleMpxSocketIdleNextAction.Continue;
                this._HandleMpxSocketIdleEventArgs.Channels = Channels;

                await handler(this, this._HandleMpxSocketIdleEventArgs);

                // continue?                
                if (State == MpxServerSocketHandlerState.Multiplexed && this._HandleMpxSocketIdleEventArgs.NextAction == HandleMpxSocketIdleNextAction.AllowCapture && this.CaptureWaiting())
                {
                    // allow capture
                    e.NextAction = NextRequestedHandlerAction.SetIdle;
                    return;
                }

                // check for pending tasks again
                if (await MpxServerSocketHandler_OnHandlePoll_DefaultActions(sender, e))
                    return;
            }

            // keep polling
            WakeupOnSignalFrom(this, "WakeupSignal", e, true);
        }

        void MpxServerSocketHandler_OnHandleReset(object sender, EventArgs e)
        {
            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Cleaning up socket handler";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------
            State = MpxServerSocketHandlerState.IdentifyingClientType;

            this.keepAlive = false;
            ClientTypeAuthChallenge = new byte[0];
            Channels.Clear();
            ClientVersion = null;
            ReceivedBytesSinceLastSent = 0;
            ControlPacketQueue.Reset();
        }

        #endregion

        #region Misc

        async Task SetState(MpxServerSocketHandlerState State)
        {
            if (this.State == State) return;

            this.State = State;

            // fire event
            var handler = OnMultiplexedInfoNewState;

            if (handler != null)
            {
                if (this._MpxServerSocketNewStateEventArgs == null)
                {
                    this._MpxServerSocketNewStateEventArgs = new MpxServerSocketNewStateEventArgs();
                }

                this._MpxServerSocketNewStateEventArgs.NewState = State;

                await handler(this, this._MpxServerSocketNewStateEventArgs);
            }
            else
            {
                #region ---------------------------------------------------------------------------------------------[LOG]-
#if (Logging)
                Logger.LogHandlerFlowMessage(saeaHandler, () =>
                {
                    return "New multiplexed protocol state: " +
                        Enum.GetName(typeof(MpxServerSocketHandlerState), State);
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------
            }
        }

        bool SendMpxBuffers(object sender, HandleSocketBEventArgs e)
        {
            lock (Channels)
            {

                var mutedChannels =
                    (
                        from q in Channels.All

                            // non-empty transmitbuffers
                        where q.ChannelYieldSend
                        select q
                    ).ToArray();

                foreach (var mutedChannel in mutedChannels)
                {
                    // buffers running empty?
                    if (mutedChannel.RxBuffer.Length < MemoryConfiguration.SendBufferSize * MPXChannelData.MinimumFactorBeforeSendingChannelContinue)
                    {
                        // remember
                        mutedChannel.ChannelYieldSend = false;

                        // send request
                        SendChannelYield(mutedChannel.ChannelName, false);
                    }
                }

                var result = false;
                var payloads =
                    (
                        from q in Channels.All

                            // non-empty transmitbuffers
                        where q.TxBuffer.Count > 0 && q.PrepareBeforeTransmit()
                        select new
                        {
                            data = q,
                            weight = q.WeightFactor.HasValue ? Math.Min(1d, Math.Max(0d, q.WeightFactor.Value)) : 1d
                        }
                    ).ToArray();

                var sum = (from q2 in payloads select q2.weight).Sum();
                var weights = payloads.Length > 0 ? (sum <= 0 ? 1 : payloads.Length / sum) : 1;

                foreach (var channel in payloads)
                {
                    var maxBytes = Math.Max(1, Math.Min(MemoryConfiguration.DynamicBufferFragmentSize - 25,
                        Convert.ToInt32(Math.Floor(

                            ((MemoryConfiguration.DynamicBufferFragmentSize - 25) / weights) *

                            channel.weight
                        ))
                    ));
                    maxBytes = Math.Min(maxBytes, channel.data.TxBuffer.Count);

                    // something to send?
                    if (maxBytes > 0)
                    {
                        // send it
                        // todo these large chunks with dynamic buffers also in clienthandler
                        var bytes = channel.data.TxBuffer.RemoveBytes(maxBytes);

                        BufferHelpers.WriteBytesToBuffer(e.TxBuffer, bytes, channel.data.ChannelName, 0, null);

                        #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                        Logger.LogSocketFlowMessage(saeaHandler, () =>
                        {
                            return "Sending " + bytes.Length.ToString() + " bytes from channel " + channel.data.ChannelName.ToString() + "'s receivebuffer, " +
                                "there are now " + channel.data.TxBuffer.Count.ToString() + " bytes left in channel sendbuffer";
                        });
#endif
                        #endregion ------------------------------------------------------------------------------------------------

                        result = true;
                    }
                }

                if (result)
                {
                    e.NextAction = NextRequestedHandlerAction.Send;
                }

                return result;
            }
        }

        bool SendControlPackets(object sender, HandleSocketBEventArgs e)
        {
            // try to send 
            if (ControlPacketQueue.MoveTo(e.TxBuffer) == 0)
            {

                return false;
            }

            #region ---------------------------------------------------------------------------------------------------
#if (Logging)
            Logger.LogSocketFlowMessage(saeaHandler, () =>
            {
                return "Sending control packets";
            });
#endif
            #endregion ------------------------------------------------------------------------------------------------

            // send
            e.NextAction = NextRequestedHandlerAction.Send;
            return true;
        }

        #endregion

        #region Session

        void SendChannelAdded(string channelName)
        {
            // send update using a dynamicbuffer for composing
            lock (ControlPacketQueue.SyncRoot)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Adding new channel \"" + channelName + "\", control packet added to queue";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // write magic packet length
                ControlPacketQueue.Writer.Write((int)MagicLengthPack.ChannelAdded);

                // write real packet
                BufferHelpers.WriteStringToBuffer(ControlPacketQueue, channelName);
            }
        }

        void SendChannelRemoved(string channelName)
        {
            // send update
            lock (ControlPacketQueue.SyncRoot)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Removing channel \"" + channelName + "\", control packet added to queue";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // write magic packet length
                ControlPacketQueue.Writer.Write((int)MagicLengthPack.ChannelRemoved);

                // write real packet
                BufferHelpers.WriteStringToBuffer(ControlPacketQueue, channelName);
            }
        }

        void SendChannelYield(string channelName, bool value)
        {
            // send update
            lock (ControlPacketQueue.SyncRoot)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Sending new channel yield for \"" + channelName + "\", new value: " + value.ToString() +
                       ", control packet added to queue";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // write magic packet length
                ControlPacketQueue.Writer.Write((int)MagicLengthPack.ChannelYield);

                // write channelName
                BufferHelpers.WriteStringToBuffer(ControlPacketQueue, channelName);

                // write value
                int i = value ? 0 : -1;

                // todo temp
                // Console.WriteLine("Channel " + channelName + ", sending bandwidth: " + i.ToString());

                ControlPacketQueue.Writer.Write(i);
                ControlPacketQueue.Writer.Flush();
            }
        }

        void SendChannelBandwidth(ChannelDataType channelData, int value)
        {
            // todo temp
            // Console.WriteLine("Channel " + channelData.ChannelName + ", sending bandwidth: " + value.ToString());

            // send update
            lock (ControlPacketQueue.SyncRoot)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Sending new channel yield for \"" + channelData.ChannelName + "\", new value: " + value.ToString() +
                       ", control packet added to queue";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                channelData.ChannelYieldSend = value >= 0 && value < 10;

                // write magic packet length
                ControlPacketQueue.Writer.Write((int)MagicLengthPack.ChannelYield);

                // write channelName
                BufferHelpers.WriteStringToBuffer(ControlPacketQueue, channelData.ChannelName);

                // write value                
                ControlPacketQueue.Writer.Write(value);
                ControlPacketQueue.Writer.Flush();
            }
        }

        public void RequestChannel(string channelName)
        {
            // send update using a dynamicbuffer for composing
            lock (ControlPacketQueue.SyncRoot)
            {
                #region ---------------------------------------------------------------------------------------------------
#if (Logging)
                Logger.LogSocketFlowMessage(saeaHandler, () =>
                {
                    return "Adding new channel request: \"" + channelName + "\", control packet added to queue";
                });
#endif
                #endregion ------------------------------------------------------------------------------------------------

                // write magic packet length
                ControlPacketQueue.Writer.Write((int)MagicLengthPack.ChannelRequest);

                // write real packet
                BufferHelpers.WriteStringToBuffer(ControlPacketQueue, channelName);
            }

            TriggerWakeup();
        }

        public bool ChannelIsActive(string Channel)
        {
            return Channels.HaveChannel(Channel);
        }


        public void CloseChannel(string sessionId)
        {
            this.Channels[sessionId] = null;
        }

        public void TriggerWakeup()
        {
            ThreadPool.QueueUserWorkItem((State) =>
            {
                Signal("WakeupSignal");
            });
        }

        #endregion
    }
}
