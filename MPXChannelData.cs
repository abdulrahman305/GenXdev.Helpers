using GenXdev.Buffers;
using GenXdev.Configuration;
using Ninject;

namespace GenXdev.AsyncSockets.Containers
{
    public class MPXChannelData : EventArgs, IDisposable, IEquatable<MPXChannelData>
    {
        public static GenXdev.Additional.HiPerfTimer.NativeMethods Timer = new GenXdev.Additional.HiPerfTimer.NativeMethods(true);
        protected object SyncRoot { get; private set; } = new object();

        // factor of consumer rx buffer
        internal static long MinimumFactorBeforeSendingChannelYield = 6;
        internal static long MinimumFactorBeforeDisconnecting = 20;
        internal static long MinimumFactorBeforeSendingChannelContinue = 4;

        // factor of consumer tx buffer
        internal static long MinimumFactorBeforeUploadReady = 2;

        #region Properties

        string _ChannelName;
        public string ChannelName
        {
            get
            {
                lock (SyncRoot)
                {
                    return _ChannelName;
                }
            }

            internal set
            {
                lock (SyncRoot)
                {
                    _ChannelName = value;
                }
            }
        }
        DynamicBuffer _RxBuffer;
        public DynamicBuffer RxBuffer
        {
            get
            {
                lock (SyncRoot)
                {
                    return _RxBuffer;
                }
            }

            internal set
            {
                lock (SyncRoot)
                {
                    _RxBuffer = value;
                }
            }
        }
        DynamicBuffer _TxBuffer;
        public DynamicBuffer TxBuffer
        {
            get
            {
                lock (SyncRoot)
                {
                    return _TxBuffer;
                }
            }

            internal set
            {
                lock (SyncRoot)
                {
                    _TxBuffer = value;
                }
            }
        }

        IMemoryManagerConfiguration MemoryConfiguration;

        bool _ChannelYieldSetByPeer;
        public bool ChannelYieldSetByPeer
        {
            get
            {
                lock (SyncRoot)
                {
                    return _ChannelYieldSetByPeer;
                }
            }

            internal set
            {
                lock (SyncRoot)
                {
                    _ChannelYieldSetByPeer = value;
                }
            }
        }

        bool _ChannelYieldSend;
        public bool ChannelYieldSend
        {
            get
            {
                lock (SyncRoot)
                {
                    return _ChannelYieldSend;
                }
            }

            internal set
            {
                lock (SyncRoot)
                {
                    _ChannelYieldSend = value;
                }
            }
        }

        double? _WeightFactor;
        public double? WeightFactor
        {
            get
            {
                lock (SyncRoot)
                {
                    return _WeightFactor;
                }
            }

            set
            {
                lock (SyncRoot)
                {
                    _WeightFactor = value;
                }
            }
        }

        #endregion

        public bool ChannelIsReadyForMoreUploads
        {
            get
            {
                // yielded?
                if (_ChannelYieldSetByPeer)
                {
                    return false;
                }

                return TxBuffer.Count < MemoryConfiguration.SendBufferSize * MinimumFactorBeforeUploadReady;
            }
        }

        public bool PrepareBeforeTransmit()
        {
            lock (SyncRoot)
            {
                // yielded?
                if (_ChannelYieldSetByPeer)
                {
                    return false;
                }

                return true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~MPXChannelData()
        {
            Dispose(false);
        }

        //
        // Summary:
        //     Releases the unmanaged resources used by the System.IO.Stream and optionally
        //     releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only unmanaged
        //     resources.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (RxBuffer != null) RxBuffer.Dispose();
                if (TxBuffer != null) TxBuffer.Dispose();

                RxBuffer = null;
                TxBuffer = null;
            }
        }

        [Inject]
        public MPXChannelData(IKernel Kernel)
        {
            this.RxBuffer = Kernel.Get<DynamicBuffer>();
            this.TxBuffer = Kernel.Get<DynamicBuffer>();
            this.MemoryConfiguration = Kernel.Get<IMemoryManagerConfiguration>();
        }

        public bool Equals(MPXChannelData other)
        {
            return other.RxBuffer == RxBuffer &&
                other.TxBuffer == TxBuffer;
        }
    }
}