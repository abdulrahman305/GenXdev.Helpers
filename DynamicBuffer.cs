/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Text;
using System.Net.Sockets;
using GenXdev.MemoryManagement;
using GenXdev.Configuration;
using Ninject;
using GenXdev.Buffers;
using System.Diagnostics;

namespace GenXdev.Buffers
{
    public delegate void DynamicBufferConversionCallback(byte[] ConversionBuffer, int Index0ByteBatchOffset, int StartPositionOffset, int Length);

    [DebuggerDisplay("Count = {Count}, Pos = {Position}}")]
    public class DynamicBuffer : Stream
    {
        public event EventHandler<DynamicBufferAddedEventArgs> OnAdded;
        public event EventHandler<DynamicBufferEmptyEventArgs> OnRemoved;
        public event EventHandler<DynamicBufferEmptyEventArgs> OnEmpty;

        void TriggerAdded(int count)
        {
            var handler = OnAdded;

            if (handler != null)
            {
                handler(
                    this,
                    new DynamicBufferAddedEventArgs(
                        this,
                        count
                    )
                );
            }
        }

        void UpdateCappedPositionAndTriggerEmptyEventIfNeeded(int removed)
        {
            if (removed == 0)
                return;

            if (_CappedPosition.HasValue)
                _CappedPosition = _CappedPosition.Value - removed;

            var onRemovedHandler = OnRemoved;

            if (onRemovedHandler != null)
            {
                onRemovedHandler(
                    this,
                    new DynamicBufferEmptyEventArgs(this)
                );
            }

            if (Count > 0)
                return;

            var onEmptyHandler = OnEmpty;

            if (onEmptyHandler != null)
            {
                onEmptyHandler(
                    this,
                    new DynamicBufferEmptyEventArgs(this)
                );
            }

        }

        #region Initialization

        [Inject]
        public DynamicBuffer(IServiceMemoryManager MemoryManager, IMemoryManagerConfiguration MemoryConfig)
        {
            this.SyncRoot = new object();
            this.MemoryManager = MemoryManager;
            this.InternalFragmentBufferSize = MemoryConfig.DynamicBufferFragmentSize;
            this.Reader = new BinaryReader(this, new UTF8Encoding(false));
            this.Writer = new BinaryWriter(this, new UTF8Encoding(false));
        }

        ~DynamicBuffer()
        {
            Dispose(false);
        }

        #endregion

        #region Fields

        IServiceMemoryManager MemoryManager;
        internal List<byte[]> Buffers = new List<byte[]>();
        long _Position;
        int? _CappedPosition;
        internal int ReadOffset;
        internal int WriteOffset;
        protected internal int InternalFragmentBufferSize;
        public object SyncRoot { get; private set; }

        #endregion

        #region Public

        public unsafe bool StartsWithSameContentAs(DynamicBuffer dynBuffer)
        {
            try
            {
                if (dynBuffer == null)
                    return false;

                if (Count == dynBuffer.Count)
                {
                    if (Count == 0)
                        return true;
                }

                if (Count > dynBuffer.Count)
                {
                    return false;
                }

                int length = Count;
                var bufferEnumerator = new DynamicBufferDoubleReadEnumerator(this, 0, dynBuffer, 0, ref length);

                foreach (var bufferInfo in bufferEnumerator)
                {
                    fixed (byte* pb1 = Buffers[bufferInfo.Buffer1Index])
                    fixed (byte* pb2 = dynBuffer.Buffers[bufferInfo.Buffer2Index])
                    {
                        var p1 = pb1;
                        var p2 = pb2;

                        p1 += bufferInfo.Buffer1Offset;
                        p2 += bufferInfo.Buffer2Offset;

                        while (--bufferInfo.Length >= 0)
                        {
                            if (*p1++ != *p2++)
                                return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public unsafe bool HasSameContent(DynamicBuffer dynBuffer)
        {
            try
            {
                if (dynBuffer == null)
                    return false;
                if (Count != dynBuffer.Count)
                {
                    return false;
                }

                if (Count == 0)
                    return true;

                int length = Count;
                var bufferEnumerator = new DynamicBufferDoubleReadEnumerator(this, 0, dynBuffer, 0, ref length);

                foreach (var bufferInfo in bufferEnumerator)
                {
                    fixed (byte* pb1 = Buffers[bufferInfo.Buffer1Index])
                    fixed (byte* pb2 = dynBuffer.Buffers[bufferInfo.Buffer2Index])
                    {
                        var p1 = pb1;
                        var p2 = pb2;

                        p1 += bufferInfo.Buffer1Offset;
                        p2 += bufferInfo.Buffer2Offset;

                        while (--bufferInfo.Length >= 0)
                        {
                            if (*p1++ != *p2++)
                                return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public BinaryReader Reader { get; private set; }
        public BinaryWriter Writer { get; private set; }

        public int Add(byte[] Source)
        {
            lock (SyncRoot)
            {
                return Add(Source, 0, Source.Length);
            }
        }

        public int Add(byte[] source, int sourceOffset, int length)
        {
            lock (SyncRoot)
            {
                // check
                length = Math.Min(length, source.Length - sourceOffset);
                var bytesLeft = length;
                var written = length;
                int bytesDone = 0;

                // loop
                while (bytesLeft > 0)
                {
                    // get buffer offset
                    int bufferOffset = WriteOffset % InternalFragmentBufferSize;

                    // buffer full?
                    if (bufferOffset == 0)
                    {
                        // add new buffer
                        AddBuffer();
                    }

                    // get nr of bytes to put in this buffer
                    int bytesNow = Math.Min(InternalFragmentBufferSize - bufferOffset, bytesLeft);

                    // copy bytes
                    Buffer.BlockCopy(source, sourceOffset + bytesDone, Buffers[Buffers.Count - 1], bufferOffset, bytesNow);

                    // update offsets
                    WriteOffset += bytesNow;
                    bytesDone += bytesNow;
                    bytesLeft -= bytesNow;
                }

                TriggerAdded(written);
                return written;
            }
        }

        public void Insert(int offset, byte[] source, int sourceOffset, int length)
        {
            lock (SyncRoot)
            {
                length = Math.Max(0, Math.Min(length, source.Length - sourceOffset));

                if (offset > this.Count)
                {
                    throw new InvalidOperationException();
                }

                if (offset == this.Count)
                {
                    Add(source, sourceOffset, length);
                    return;
                }

                int extraBuffersNeeded = Convert.ToInt32(Math.Ceiling(

                    // needed length minus any additional freespace left in lastbuffer
                    (length - (
                       // last buffer full? -> minus nothing
                       WriteOffset % InternalFragmentBufferSize == 0 ? 0 :

                       // last buffer not full! -> minus freespace
                       InternalFragmentBufferSize - (WriteOffset % InternalFragmentBufferSize)
                       )
                    ) / Convert.ToDouble(InternalFragmentBufferSize)
                ));

                // add buffers
                for (var i = 0; i < extraBuffersNeeded; i++)
                    AddBuffer();

                // move data away from insertion point
                int moved = 0;
                var bufferMoveEnumerator = new DynamicBufferMoveDownEnumerator(this, offset, length);
                foreach (var bufferInfo in bufferMoveEnumerator)
                {
                    Buffer.BlockCopy(
                        Buffers[bufferInfo.SourceBufferIndex],
                        bufferInfo.SourceBufferOffset,

                        Buffers[bufferInfo.TargetBufferIndex],
                        bufferInfo.TargetBufferOffset,

                        bufferInfo.Length
                    );

                    moved += bufferInfo.Length;
                }
                WriteOffset += length;

                var bufferReadEnumerator = new DynamicBufferReadEnumerator(this, offset, ref length);

                int copied = 0;

                foreach (var bufferInfo in bufferReadEnumerator)
                {
                    Buffer.BlockCopy(

                        source,
                        sourceOffset + copied,

                        Buffers[bufferInfo.SourceBufferIndex],
                        bufferInfo.SourceBufferOffset,

                        bufferInfo.Length
                    );

                    copied += bufferInfo.Length;
                }

                TriggerAdded(length);
            }
        }

        public void Add(byte Source)
        {
            lock (SyncRoot)
            {
                // not enough buffers?
                if ((WriteOffset++ / InternalFragmentBufferSize) == Buffers.Count)
                    AddBuffer();

                // set byte
                this[RealCount - 1] = Source;

                TriggerAdded(1);
            }
        }

        public int Remove(byte[] target, int targetOffset, int length)
        {
            lock (SyncRoot)
            {
                // get nr of bytes to (re)move
                int nrOfBytes = Math.Min(Math.Min(length, Count), target.Length - targetOffset);

                // check
                if (nrOfBytes <= 0)
                    return 0;

                // loop
                int bytesProcessed = 0;

                while (bytesProcessed < nrOfBytes)
                {
                    // get nr of bytes from first buffer
                    int nrOfBytesFromFirstBuffer = Math.Min(InternalFragmentBufferSize - ReadOffset, nrOfBytes - bytesProcessed);

                    // copy bytes
                    Buffer.BlockCopy(Buffers[0], ReadOffset, target, targetOffset + bytesProcessed, nrOfBytesFromFirstBuffer);

                    // update cursors
                    bytesProcessed += nrOfBytesFromFirstBuffer;
                    ReadOffset += nrOfBytesFromFirstBuffer;
                    if (ReadOffset >= InternalFragmentBufferSize)
                    {
                        ReadOffset -= InternalFragmentBufferSize;
                        WriteOffset -= InternalFragmentBufferSize;
                        ReturnFirstBuffer();
                    }
                    if (RealCount == 0)
                        Reset();
                }

                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(nrOfBytes);

                return nrOfBytes;
            }
        }

        public int RemoveAt(int offset, byte[] target, int targetOffset, int length)
        {
            lock (SyncRoot)
            {
                if (offset == 0)
                {
                    return Remove(target, targetOffset, length);
                }

                var bufferEnumerator = new DynamicBufferReadEnumerator(this, offset, ref length);
                var removed = 0;

                foreach (var bufferInfo in bufferEnumerator)
                {
                    Buffer.BlockCopy(
                        Buffers[bufferInfo.SourceBufferIndex],
                        bufferInfo.SourceBufferOffset,

                        target,
                        targetOffset + removed,

                        bufferInfo.Length
                    );

                    removed += bufferInfo.Length;
                }

                if (RemoveAt(offset, length) != removed)
                {
                    throw new InvalidOperationException();
                }

                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(removed);

                return removed;
            }
        }

        UTF8Encoding utf8 = new UTF8Encoding(false);
        public string ToString(Encoding encoding = null, long? maxLength = null, int offset = 0)
        {
            try
            {
                lock (SyncRoot)
                {
                    offset = Math.Min(Count, Math.Max(0, offset));
                    var max = Math.Max(0, Math.Min(Count - offset, maxLength.HasValue ? maxLength.Value : 1024));

                    if (max == 0) return String.Empty;

                    encoding = encoding == null ? (utf8) : encoding;

                    var oldPosition = _Position;
                    try
                    {
                        _Position = offset;

                        var bytes = MemoryManager.TakeBuffer((int)max);
                        try
                        {
                            Read(bytes, 0, bytes.Length);

                            return encoding.GetString(bytes, 0, (int)max) + (

                                  max < Count - offset && !maxLength.HasValue ? "..[Truncated]" : String.Empty
                                );
                        }
                        finally
                        {
                            MemoryManager.ReturnBuffer(bytes);
                        }
                    }
                    finally
                    {
                        _Position = oldPosition;
                    }
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public int RemoveAt(int offset, int length)
        {
            lock (SyncRoot)
            {
                if (offset == 0)
                {
                    return Remove(length);
                }

                var bufferEnumerator = new DynamicBufferMoveUpEnumerator(this, offset, ref length);

                if (length == 0)
                    return 0;

                var movedUp = 0;

                foreach (var bufferInfo in bufferEnumerator)
                {
                    // move the bytes
                    Buffer.BlockCopy(
                        Buffers[bufferInfo.SourceBufferIndex],
                        bufferInfo.SourceBufferOffset,

                        Buffers[bufferInfo.TargetBufferIndex],
                        bufferInfo.TargetBufferOffset,

                        bufferInfo.Length
                    );

                    movedUp += bufferInfo.Length;
                }

                WriteOffset -= length;

                ReleaseUnneededBuffers();

                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(movedUp);

                return length;
            }
        }

        public int Remove(int NrOfBytes)
        {
            lock (SyncRoot)
            {
                NrOfBytes = Math.Min(Count, NrOfBytes);

                ReadOffset += NrOfBytes;

                while (ReadOffset >= InternalFragmentBufferSize)
                {
                    ReadOffset -= InternalFragmentBufferSize;
                    WriteOffset -= InternalFragmentBufferSize;
                    ReturnFirstBuffer();
                }
                if (RealCount == 0)
                    Reset();

                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(NrOfBytes);

                return NrOfBytes;
            }
        }

        public int RemoveAtEnd(int NrOfBytes)
        {
            lock (SyncRoot)
            {
                NrOfBytes = Math.Min(Count, NrOfBytes);

                if (_CappedPosition.HasValue)
                {
                    return RemoveAt(Count - NrOfBytes, NrOfBytes);
                }
                else
                {
                    WriteOffset -= NrOfBytes;

                    // release unneeded buffers
                    ReleaseUnneededBuffers();
                }

                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(NrOfBytes);

                return NrOfBytes;
            }
        }

        public int IndexOf(byte[] searchSequence, int startPosition = 0, int reducedSearchSeqTailLength = 0)
        {
            lock (SyncRoot)
            {
                startPosition = Math.Max(0, startPosition);

                var seqLength = Math.Max(0, searchSequence.Length - reducedSearchSeqTailLength);

                if (seqLength == 0)
                    return -1;

                if (startPosition > (Count - seqLength))
                {
                    return -1;
                }

                int matchCount = 0;

                for (var i = startPosition; i < Count; i++)
                {
                    if (this[i] == searchSequence[matchCount])
                    {
                        matchCount++;

                        if (matchCount == seqLength)
                            return (i + 1) - seqLength;
                    }
                    else
                    {
                        matchCount = 0;
                        if (this[i] == searchSequence[matchCount])
                        {
                            matchCount++;

                            if (matchCount == seqLength)
                                return (i + 1) - seqLength;
                        }
                    }
                }

                return -1;
            }
        }

        public int IndexOf(ref int startPosition, byte[] searchSequence)
        {
            var result = IndexOf(searchSequence, startPosition);

            if (result < 0)
            {
                startPosition = AdvanceSearchStartPosition(startPosition, searchSequence);
            }
            else
            {
                startPosition = result + 1;
            }

            return result;
        }

        public int IndexOf(ref int startPosition, ref int? count, bool SslStarted, byte[] searchSequence)
        {
            var result = IndexOf(searchSequence, startPosition);

            if (result < 0)
            {
                startPosition = AdvanceSearchStartPosition(startPosition, searchSequence);

                if (SslStarted)
                {
                    count = Math.Min(searchSequence.Length, Math.Max(1, searchSequence.Length - (Count - startPosition)));
                }
            }
            else
            {
                startPosition = result + 1;

                if (SslStarted)
                {
                    count = 1;
                }
            }

            return result;
        }

        private int AdvanceSearchStartPosition(int startPosition, byte[] searchSequence)
        {
            for (
                // start by expecting that only one byte from the sequence is missing
                startPosition = Math.Max(0, Count - (searchSequence.Length - 1));

                // it is possible that we become sure, we have none off the bytes of the search sequence in the buffer
                // by looking at the last meaningfull ones at the end of the buffer
                (startPosition < Count) &&

                // keep checking
                (IndexOf(searchSequence, startPosition, searchSequence.Length - (Count - startPosition)) < 0);

                // move startposition forward, while we notice that we have no match on the reduced searchSequence
                startPosition++
             ) { }
            return startPosition;
        }

        public void WriteToStream(Stream stream)
        {
            lock (SyncRoot)
            {
                int length = Count;
                var bufferEnumerator = new DynamicBufferReadEnumerator(this, 0, ref length);

                foreach (var bufferInfo in bufferEnumerator)
                {
                    stream.Write(
                        Buffers[bufferInfo.SourceBufferIndex],
                        bufferInfo.SourceBufferOffset,
                        bufferInfo.Length
                    );
                }
            }
        }

        public byte[] RemoveBytes(int? count = null)
        {
            lock (SyncRoot)
            {
                if (!count.HasValue)
                {
                    count = Count;
                }
                else
                {
                    count = Math.Min(Count, count.Value);
                }

                var result = new byte[count.Value];

                Remove(result, 0, count.Value);

                return result;
            }
        }

        public void Add(SocketAsyncEventArgs saea)
        {
            Add(saea.Buffer, saea.Offset, saea.BytesTransferred);
        }

        public int MoveTo(Stream stream, int MaxLength, DynamicBufferConversionCallback ConversionCallback = null)
        {
            lock (SyncRoot)
            {
                // check
                MaxLength = Math.Min(MaxLength, Count);

                // remember
                int NrOfBytesMoved = MaxLength;

                while (MaxLength > 0)
                {
                    // get nr of bytes to move from the first buffer
                    int NrOfBytesFromFirstBuffer = Math.Min(MaxLength, InternalFragmentBufferSize - ReadOffset);

                    // converting?
                    if (ConversionCallback != null)
                    {
                        // call conversion callback
                        ConversionCallback(Buffers[0], (NrOfBytesMoved - MaxLength) - ReadOffset, ReadOffset, NrOfBytesFromFirstBuffer);
                    }

                    // write to stream
                    stream.Write(Buffers[0],
                        // offset
                        ReadOffset,
                        // count
                        NrOfBytesFromFirstBuffer);

                    // update cursors
                    MaxLength -= NrOfBytesFromFirstBuffer;
                    ReadOffset += NrOfBytesFromFirstBuffer;
                    if (ReadOffset >= InternalFragmentBufferSize)
                    {
                        ReadOffset -= InternalFragmentBufferSize;
                        WriteOffset -= InternalFragmentBufferSize;
                        ReturnFirstBuffer();
                    }
                    if (RealCount == 0)
                        Reset();
                }

                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(NrOfBytesMoved);

                return NrOfBytesMoved;
            }
        }

        public int IndexOf(byte SearchValue, int offset = 0, int length = -1)
        {
            lock (SyncRoot)
            {
                if (offset < 0) offset = 0;
                if (offset > Count - 1) return -1;

                length = (length < 0) ? Count : Math.Min(Count, length - offset);

                var bufferEnumerator = new DynamicBufferReadEnumerator(this, offset, ref length);

                foreach (var bufferInfo in bufferEnumerator)
                {
                    var idx = Array.IndexOf<byte>(
                        Buffers[bufferInfo.SourceBufferIndex],
                        SearchValue,
                        bufferInfo.SourceBufferOffset,
                        bufferInfo.Length
                    );

                    if (idx >= 0)
                    {
                        return (idx - ReadOffset) + (bufferInfo.SourceBufferIndex * InternalFragmentBufferSize);
                    }
                }

                return -1;
            }
        }

        public void FillAndSetSendBuffer(SocketAsyncEventArgs saea, int maxBytes, bool AddToExistingBufferData = false)
        {
            lock (SyncRoot)
            {
                maxBytes = Math.Min(maxBytes + (AddToExistingBufferData ? saea.Count : 0), Count);

                if ((saea.Buffer == null) || (saea.Buffer.Length != maxBytes) || (saea.Offset > 0))
                {
                    saea.SetBuffer(new byte[maxBytes], 0, maxBytes);
                }
                if (AddToExistingBufferData)
                {
                    Remove(saea.Buffer, saea.Count, maxBytes);
                }
                else
                {
                    Remove(saea.Buffer, 0, maxBytes);
                }
            }
        }

        public int MoveTo(DynamicBuffer Target)
        {
            int count;

            lock (SyncRoot)
            {
                // reference
                count = Count;
                // nothing to do?
                if (count == 0) return 0;
                // not a candidate for fast buffer swap?
                if (RealCount != count) return MoveTo(Target, Count);

                // lock other buffer
                lock (Target.SyncRoot)
                {
                    // not a candidate for fast buffer swap ?
                    if (Target.RealCount != 0) return MoveTo(Target, Count);

                    // remember own settings
                    var _Buffers = Buffers;
                    var _ReadOffset = ReadOffset;
                    var _WriteOffset = WriteOffset;

                    // swap buffers
                    Buffers = Target.Buffers;

                    // Reset self
                    _Position = 0;
                    ReadOffset = 0;
                    WriteOffset = 0;
                    CappedPosition = null;
                    StreamEndOffset = null;

                    // swap buffers and cursors
                    Target.Buffers = _Buffers;
                    Target.ReadOffset = _ReadOffset;
                    Target.WriteOffset = _WriteOffset;
                }

                // trigger event
                UpdateCappedPositionAndTriggerEmptyEventIfNeeded(count);
            }

            // trigger event
            Target.TriggerAdded(count);

            return count;
        }

        public int MoveTo(DynamicBuffer Target, int MaxBytes, DynamicBufferConversionCallback ConversionCallback = null)
        {
            lock (SyncRoot)
            {
                // determine how many bytes to move
                int BytesToMove = Math.Min(MaxBytes, Count);

                // remember how many bytes
                int BytesMoved = 0;

                // create temporary buffer
                byte[] TempBuffer = MemoryManager.TakeBuffer(Math.Min(InternalFragmentBufferSize, BytesToMove));

                try
                {
                    //  loop
                    while (BytesToMove > 0)
                    {
                        // remove next block
                        int BytesRemoved = Remove(TempBuffer, 0, Math.Min(BytesToMove, TempBuffer.Length));

                        // converting?
                        if (ConversionCallback != null)
                        {
                            // call convertion callback
                            ConversionCallback(TempBuffer, BytesMoved, 0, BytesRemoved);
                        }

                        // add it to target
                        Target.Add(TempBuffer, 0, BytesRemoved);

                        // update counter
                        BytesToMove -= BytesRemoved;
                        BytesMoved += BytesRemoved;
                    }
                }
                finally
                {
                    // return temporary buffer
                    MemoryManager.ReturnBuffer(TempBuffer);
                }

                // return count
                return BytesMoved;
            }
        }

        public void Reset()
        {
            lock (SyncRoot)
            {
                var oldSize = Count;

                ReadOffset = 0;
                WriteOffset = 0;
                Position = 0;
                CappedPosition = null;
                StreamEndOffset = null;

                foreach (var buffer in Buffers)
                {
                    MemoryManager.ReturnBuffer(buffer);
                }
                Buffers.Clear();

                if (oldSize > 0)
                {
                    UpdateCappedPositionAndTriggerEmptyEventIfNeeded(oldSize);
                }
            }
        }

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    if (CappedPosition.HasValue)
                        return CappedPosition.Value;

                    return WriteOffset - ReadOffset;
                }
            }
        }

        public int? CappedPosition
        {
            get
            {
                lock (SyncRoot)
                {
                    return _CappedPosition;
                }
            }

            set
            {
                lock (SyncRoot)
                {
                    _CappedPosition = value;
                }
            }
        }

        public int RealCount
        {
            get
            {
                lock (SyncRoot)
                {
                    return WriteOffset - ReadOffset;
                }
            }
        }

        public byte this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    index += ReadOffset;
                    return Buffers[index / InternalFragmentBufferSize][index % InternalFragmentBufferSize];
                }
            }

            set
            {
                lock (SyncRoot)
                {
                    index += ReadOffset;
                    Buffers[index / InternalFragmentBufferSize][index % InternalFragmentBufferSize] = value;
                }
            }
        }

        public int MoveTo(Encoding SourceEncoding, DynamicBuffer Target, Encoding TargetEncoding, int MaxBytes,
                          DynamicBufferConversionCallback ConversionCallback = null, int startPosition = 0)
        {
            lock (SyncRoot)
            {
                // check
                if (Count == 0)
                    return 0;

                // create counter;
                int TotalNrOfBytesToConvert = Convert.ToInt32(Math.Min(MaxBytes, Count - startPosition));
                int BytesLeft = TotalNrOfBytesToConvert;

                // init buffer variables
                byte[] TargetConversionBuffer = null;
                byte[] SourceUnicodeConversionBuffer = null;
                byte[] SourceEncodingByteBuffer = null;

                try
                {
                    // create buffer to hold the bytes that are moved and converted
                    int SourceEncodingByteBufferLength = Convert.ToInt32(Math.Ceiling(Math.Min(Convert.ToDouble(TotalNrOfBytesToConvert),
                            Convert.ToDouble(Math.Min(Convert.ToDouble(InternalFragmentBufferSize),
                            Convert.ToDouble(InternalFragmentBufferSize / (2d * Math.Max(1d, (TargetEncoding.GetMaxByteCount(1) / 2d)))))))));

                    SourceEncodingByteBuffer = MemoryManager.TakeBuffer(SourceEncodingByteBufferLength);

                    // check
                    if ((SourceEncodingByteBuffer == null) || (SourceEncodingByteBuffer.Length < SourceEncodingByteBufferLength))
                    {
                        throw new InvalidOperationException("Out of memory");
                    }

                    // get buffer for conversion from Source to unicode
                    int SourceUnicodeConversionBufferLength = SourceEncodingByteBufferLength * 2;
                    SourceUnicodeConversionBuffer = MemoryManager.TakeBuffer(SourceUnicodeConversionBufferLength);

                    // failed?
                    if ((SourceUnicodeConversionBuffer == null) || (SourceUnicodeConversionBuffer.Length < SourceUnicodeConversionBufferLength))
                    {
                        throw new InvalidOperationException("Out of memory");
                    }

                    // get large enough buffer for conversion from unicode to target encoding
                    int TargetConversionBufferLength = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(SourceUnicodeConversionBufferLength) / (2d / Convert.ToDouble(TargetEncoding.GetMaxByteCount(1)))));
                    TargetConversionBuffer = MemoryManager.TakeBuffer(TargetConversionBufferLength);

                    // failed?
                    if ((TargetConversionBuffer == null) || (TargetConversionBuffer.Length < TargetConversionBufferLength))
                    {
                        throw new InvalidOperationException("Out of memory");
                    }

                    // loop
                    while (BytesLeft > 0)
                    {
                        // get the payload of this batch
                        int NrOfBytesToConvert = RemoveAt(startPosition, SourceEncodingByteBuffer, 0, Math.Min(SourceEncodingByteBufferLength, BytesLeft));

                        // let caller convert bytes if requested on a fixed size byte to byte basis
                        if (ConversionCallback != null)
                        {
                            ConversionCallback(SourceEncodingByteBuffer, TotalNrOfBytesToConvert - BytesLeft, 0, NrOfBytesToConvert);
                        }

                        int bytesConverted;
                        unsafe
                        {
                            // get byte pointer to Target conversionbuffer
                            fixed (byte* TargetConversionBufferBytePointer = &TargetConversionBuffer[0])
                            {
                                // get byte pointer to sourcerencoding conversion buffer
                                fixed (byte* SourceEncodingByteBufferBytePointer = &SourceEncodingByteBuffer[0])
                                {
                                    // get byte pointer to Source conversion buffer
                                    fixed (byte* SourceUnicodeConversionBufferBytePointer = &SourceUnicodeConversionBuffer[0])
                                    {
                                        // get char pointer to Source conversion buffer
                                        char* SourceUnicodeConversionBufferCharPointer = (char*)SourceUnicodeConversionBufferBytePointer;

                                        // convert!
                                        bytesConverted = TargetEncoding.GetBytes(SourceUnicodeConversionBufferCharPointer,
                                            SourceEncoding.GetChars(SourceEncodingByteBufferBytePointer, NrOfBytesToConvert, SourceUnicodeConversionBufferCharPointer,
                                            NrOfBytesToConvert),
                                            TargetConversionBufferBytePointer, TargetConversionBufferLength);
                                    }
                                }
                            }
                        }

                        // copy result to target
                        Target.Add(TargetConversionBuffer, 0, bytesConverted);

                        // update cursors
                        BytesLeft -= NrOfBytesToConvert;
                    }
                }
                finally
                {
                    if (SourceEncodingByteBuffer != null)
                    {
                        MemoryManager.ReturnBuffer(SourceEncodingByteBuffer);
                    }
                    if (TargetConversionBuffer != null)
                    {
                        MemoryManager.ReturnBuffer(TargetConversionBuffer);
                    }
                    if (SourceUnicodeConversionBuffer != null)
                    {
                        MemoryManager.ReturnBuffer(SourceUnicodeConversionBuffer);
                    }
                }

                return TotalNrOfBytesToConvert;
            }
        }

        public int Add(Encoding SourceEncoding, byte[] Source, int SourceOffset, Encoding TargetEncoding, int Length, DynamicBufferConversionCallback ConversionCallback = null)
        {
            lock (SyncRoot)
            {
                // check
                if (Length <= 0)
                    return 0;

                // create counter;
                int TotalNrOfBytesToConvert = Convert.ToInt32(Math.Min(Length, Source.Length - SourceOffset));
                int BytesLeft = TotalNrOfBytesToConvert;
                int BytesAdded = 0;

                // init buffer variables
                byte[] TargetConversionBuffer = null;
                byte[] SourceUnicodeConversionBuffer = null;

                try
                {
                    // create buffer to hold the bytes that are moved and converted
                    int MaxBatchSize = Convert.ToInt32(Math.Ceiling(Math.Min(Convert.ToDouble(TotalNrOfBytesToConvert),
                            Convert.ToDouble(Math.Min(Convert.ToDouble(InternalFragmentBufferSize),
                            Convert.ToDouble(InternalFragmentBufferSize / (2d * Math.Max(1d, (TargetEncoding.GetMaxByteCount(1) / 2d)))))))));

                    // get buffer for conversion from Source to unicode
                    int SourceUnicodeConversionBufferLength = MaxBatchSize * 2;
                    SourceUnicodeConversionBuffer = MemoryManager.TakeBuffer(SourceUnicodeConversionBufferLength);

                    // failed?
                    if ((SourceUnicodeConversionBuffer == null) || (SourceUnicodeConversionBuffer.Length < SourceUnicodeConversionBufferLength))
                    {
                        throw new InvalidOperationException("Out of memory");
                    }

                    // get large enough buffer for conversion from unicode to target encoding
                    int TargetConversionBufferLength = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(SourceUnicodeConversionBufferLength) /
                                                         (2d / Convert.ToDouble(TargetEncoding.GetMaxByteCount(1)))));
                    TargetConversionBuffer = MemoryManager.TakeBuffer(TargetConversionBufferLength);

                    // failed?
                    if ((TargetConversionBuffer == null) || (TargetConversionBuffer.Length < TargetConversionBufferLength))
                    {
                        throw new InvalidOperationException("Out of memory");
                    }

                    // loop
                    while (BytesLeft > 0)
                    {
                        // get the payload of this batch
                        int NrOfBytesToConvert = Math.Min(MaxBatchSize, BytesLeft);

                        // let caller convert bytes if requested on a fixed size byte to byte basis
                        if (ConversionCallback != null)
                        {
                            ConversionCallback(Source, (TotalNrOfBytesToConvert - BytesLeft) - SourceOffset, (TotalNrOfBytesToConvert - BytesLeft) + SourceOffset, NrOfBytesToConvert);
                        }

                        int bytesConverted;
                        unsafe
                        {
                            // get byte pointer to Target conversionbuffer
                            fixed (byte* TargetConversionBufferBytePointer = &TargetConversionBuffer[0])
                            {
                                // get byte pointer to sourcerencoding conversion buffer
                                fixed (byte* SourceEncodingByteBufferBytePointer = &Source[(TotalNrOfBytesToConvert - BytesLeft) + SourceOffset])
                                {
                                    // get byte pointer to Source conversion buffer
                                    fixed (byte* SourceUnicodeConversionBufferBytePointer = &SourceUnicodeConversionBuffer[0])
                                    {
                                        // get char pointer to Source conversion buffer
                                        char* SourceUnicodeConversionBufferCharPointer = (char*)SourceUnicodeConversionBufferBytePointer;

                                        // convert!
                                        bytesConverted = TargetEncoding.GetBytes(SourceUnicodeConversionBufferCharPointer,
                                            SourceEncoding.GetChars(SourceEncodingByteBufferBytePointer, NrOfBytesToConvert, SourceUnicodeConversionBufferCharPointer,
                                            NrOfBytesToConvert),
                                            TargetConversionBufferBytePointer, TargetConversionBufferLength);
                                    }
                                }
                            }
                        }

                        // copy result to target
                        Add(TargetConversionBuffer, 0, bytesConverted);

                        // update counters
                        BytesAdded += NrOfBytesToConvert;
                        BytesLeft -= NrOfBytesToConvert;
                    }
                }
                finally
                {
                    if (TargetConversionBuffer != null)
                    {
                        MemoryManager.ReturnBuffer(TargetConversionBuffer);
                    }
                    if (SourceUnicodeConversionBuffer != null)
                    {
                        MemoryManager.ReturnBuffer(SourceUnicodeConversionBuffer);
                    }
                }

                return BytesAdded;
            }
        }

        #endregion

        #region Private

        void ReleaseUnneededBuffers()
        {
            lock (SyncRoot)
            {
                // release unneeded buffers
                for (var i = Buffers.Count - 1; i > (WriteOffset / InternalFragmentBufferSize); i--)
                {
                    MemoryManager.ReturnBuffer(Buffers[i]);

                    Buffers.RemoveAt(i);
                }
                while (ReadOffset >= InternalFragmentBufferSize)
                {
                    ReturnFirstBuffer();
                    ReadOffset -= InternalFragmentBufferSize;
                }
                if ((Buffers.Count == 1) && (ReadOffset >= WriteOffset))
                {
                    Reset();
                }
            }
        }

        void AddBuffer()
        {
            byte[] buffer = MemoryManager.TakeBuffer(InternalFragmentBufferSize);

            if ((buffer == null) || (buffer.Length < InternalFragmentBufferSize))
            {
                throw new InvalidOperationException("Out of memory when creating dynamic buffer");
            }

            Buffers.Add(buffer);
        }

        void InsertBuffer(int position)
        {
            byte[] buffer = MemoryManager.TakeBuffer(InternalFragmentBufferSize);

            if ((buffer == null) || (buffer.Length < InternalFragmentBufferSize))
            {
                throw new InvalidOperationException("Out of memory when creating dynamic buffer");
            }

            Buffers.Insert(position, buffer);
        }

        void ReturnFirstBuffer()
        {
            MemoryManager.ReturnBuffer(Buffers[0]);
            Buffers.RemoveAt(0);
        }

        #endregion

        #region Stream

        public int? StreamEndOffset;

        // Summary:
        //     When overridden in a derived class, gets a value indicating whether the current
        //     stream supports reading.
        //
        // Returns:
        //     true if the stream supports reading; otherwise, false.
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        //
        // Summary:
        //     When overridden in a derived class, gets a value indicating whether the current
        //     stream supports seeking.
        //
        // Returns:
        //     true if the stream supports seeking; otherwise, false.
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }
        //
        // Summary:
        //     Gets a value that determines whether the current stream can time out.
        //
        // Returns:
        //     A value that determines whether the current stream can time out.
        public override bool CanTimeout
        {
            get
            {
                return false;
            }
        }

        //
        // Summary:
        //     When overridden in a derived class, gets a value indicating whether the current
        //     stream supports writing.
        //
        // Returns:
        //     true if the stream supports writing; otherwise, false.
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        //
        // Summary:
        //     When overridden in a derived class, gets the length in bytes of the stream.
        //
        // Returns:
        //     A long value representing the length of the stream in bytes.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     A class derived from Stream does not support seeking.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override long Length
        {
            get
            {
                return Count;
            }
        }
        //
        // Summary:
        //     When overridden in a derived class, gets or sets the position within the
        //     current stream.
        //
        // Returns:
        //     The current position within the stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support seeking.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override long Position
        {
            get
            {
                lock (SyncRoot)
                {
                    return _Position;
                }
            }

            set
            {
                lock (SyncRoot)
                {
                    _Position = value;
                }
            }
        }

        // Summary:
        //     Begins an asynchronous read operation.
        //
        // Parameters:
        //   buffer:
        //     The buffer to read the data into.
        //
        //   offset:
        //     The byte offset in buffer at which to begin writing data read from the stream.
        //
        //   count:
        //     The maximum number of bytes to read.
        //
        //   callback:
        //     An optional asynchronous callback, to be called when the read is complete.
        //
        //   state:
        //     A user-provided object that distinguishes this particular asynchronous read
        //     request from other requests.
        //
        // Returns:
        //     An System.IAsyncResult that represents the asynchronous read, which could
        //     still be pending.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     Attempted an asynchronous read past the end of the stream, or a disk error
        //     occurs.
        //
        //   System.ArgumentException:
        //     One or more of the arguments is invalid.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        //
        //   System.NotSupportedException:
        //     The current Stream implementation does not support the read operation.
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        //
        // Summary:
        //     Begins an asynchronous write operation.
        //
        // Parameters:
        //   buffer:
        //     The buffer to write data from.
        //
        //   offset:
        //     The byte offset in buffer from which to begin writing.
        //
        //   count:
        //     The maximum number of bytes to write.
        //
        //   callback:
        //     An optional asynchronous callback, to be called when the write is complete.
        //
        //   state:
        //     A user-provided object that distinguishes this particular asynchronous write
        //     request from other requests.
        //
        // Returns:
        //     An IAsyncResult that represents the asynchronous write, which could still
        //     be pending.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     Attempted an asynchronous write past the end of the stream, or a disk error
        //     occurs.
        //
        //   System.ArgumentException:
        //     One or more of the arguments is invalid.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        //
        //   System.NotSupportedException:
        //     The current Stream implementation does not support the write operation.
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        //
        // Summary:
        //     Closes the current stream and releases any resources (such as sockets and
        //     file handles) associated with the current stream.
        public override void Close()
        {
            Reset();
        }

        //
        // Summary:
        //     Releases the unmanaged resources used by the System.IO.Stream and optionally
        //     releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only
        //     unmanaged resources.
        protected override void Dispose(bool disposing)
        {
            try
            {
                lock (SyncRoot)
                {
                    if (disposing)
                    {
                        OnAdded = null;
                        OnEmpty = null;

                        Reset();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //
        // Summary:
        //     When overridden in a derived class, clears all buffers for this stream and
        //     causes any buffered data to be written to the underlying device.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        public override void Flush()
        {
        }

        //
        // Summary:
        //     When overridden in a derived class, reads a sequence of bytes from the current
        //     stream and advances the position within the stream by the number of bytes
        //     read.
        //
        // Parameters:
        //   buffer:
        //     An array of bytes. When this method returns, the buffer contains the specified
        //     byte array with the values between offset and (offset + count - 1) replaced
        //     by the bytes read from the current source.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin storing the data read
        //     from the current stream.
        //
        //   count:
        //     The maximum number of bytes to be read from the current stream.
        //
        // Returns:
        //     The total number of bytes read into the buffer. This can be less than the
        //     number of bytes requested if that many bytes are not currently available,
        //     or zero (0) if the end of the stream has been reached.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The sum of offset and count is larger than the buffer length.
        //
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset or count is negative.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support reading.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override int Read(byte[] buffer, int offset, int length)
        {
            lock (SyncRoot)
            {
                var bufferEnumerator = new DynamicBufferReadEnumerator(this, (int)Position, ref length);
                int written = 0;

                foreach (var bufferInfo in bufferEnumerator)
                {
                    Buffer.BlockCopy(
                        Buffers[bufferInfo.SourceBufferIndex],
                        bufferInfo.SourceBufferOffset,

                        buffer,
                        offset + written,
                        bufferInfo.Length
                    );

                    written += bufferInfo.Length;
                    Position += bufferInfo.Length;
                }

                return written;
            }
        }

        //
        // Summary:
        //     Reads a byte from the stream and advances the position within the stream
        //     by one byte, or returns -1 if at the end of the stream.
        //
        // Returns:
        //     The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The stream does not support reading.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override int ReadByte()
        {
            lock (SyncRoot)
            {
                int result;
                if (Position < (StreamEndOffset.HasValue ? StreamEndOffset.Value : Count))
                {
                    result = this[Convert.ToInt32(Position)];
                }
                else
                {
                    return -1;
                }
                _Position++;

                return result;
            }
        }

        //
        // Summary:
        //     When overridden in a derived class, sets the position within the current
        //     stream.
        //
        // Parameters:
        //   offset:
        //     A byte offset relative to the origin parameter.
        //
        //   origin:
        //     A value of type System.IO.SeekOrigin indicating the reference point used
        //     to obtain the new position.
        //
        // Returns:
        //     The new position within the current stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support seeking, such as if the stream is constructed
        //     from a pipe or console output.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (SyncRoot)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        _Position = offset;
                        break;
                    case SeekOrigin.Current:
                        _Position += offset;
                        break;
                    case SeekOrigin.End:
                        _Position = (StreamEndOffset.HasValue ? StreamEndOffset.Value : Count) - offset;
                        break;
                }
                return Position;
            }
        }
        //
        // Summary:
        //     When overridden in a derived class, sets the length of the current stream.
        //
        // Parameters:
        //   value:
        //     The desired length of the current stream in bytes.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support both writing and seeking, such as if the stream
        //     is constructed from a pipe or console output.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        //
        // Summary:
        //     When overridden in a derived class, writes a sequence of bytes to the current
        //     stream and advances the current position within this stream by the number
        //     of bytes written.
        //
        // Parameters:
        //   buffer:
        //     An array of bytes. This method copies count bytes from buffer to the current
        //     stream.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin copying bytes to the
        //     current stream.
        //
        //   count:
        //     The number of bytes to be written to the current stream.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The sum of offset and count is greater than the buffer length.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (SyncRoot)
            {
                Add(buffer, 0, count);
            }
        }

        //
        // Summary:
        //     Writes a byte to the current position in the stream and advances the position
        //     within the stream by one byte.
        //
        // Parameters:
        //   value:
        //     The byte to write to the stream.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        public override void WriteByte(byte value)
        {
            Add(value);
        }

        #endregion

        #region crc32
        public const uint DefaultSeed = 0xffffffff;

        readonly static uint[] crc32LookupTable = new uint[] {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
            0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
            0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
            0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
            0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
            0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
            0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
            0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
            0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
            0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
            0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
            0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
            0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
            0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
            0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
            0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
            0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
            0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
            0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
            0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
            0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
            0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
            0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
            0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
            0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
            0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
            0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
            0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
            0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
            0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
            0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
            0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
            0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
            0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
            0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
            0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
            0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
            0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
            0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
            0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
            0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
            0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
            0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
            0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
            0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
            0x2D02EF8D
        };

        public UInt32 Calccrc32(int offset = 0, int length = -1)
        {
            lock (SyncRoot)
            {
                // init
                uint _crc = 0 ^ DefaultSeed;

                Updatecrc32(ref _crc, offset, length);

                _crc ^= DefaultSeed;

                return _crc;
            }
        }

        public static UInt32 Initializecrc32(ref uint _crc)
        {
            // init
            _crc = 0 ^ DefaultSeed;

            return _crc;
        }

        public static unsafe void Updatecrc32(ref uint _crc, byte[] buffer, int offset = -1, int count = -1)
        {
            // offset
            if (offset < 0)
            {
                offset = 0;
            }

            // safeguard and check
            if (count < 0)
            {
                count = buffer.Length - offset;
            }
            else
            {
                count = Math.Min(buffer.Length - offset, count);
            }

            fixed (uint* tabP = crc32LookupTable)
            fixed (byte* bufP = buffer)
            {
                var until = offset + count;
                var buf = bufP;
                buf += offset;
                while (offset++ < until)
                    unchecked
                    {
                        _crc = tabP[(_crc ^ *buf++) & 0xFF] ^ (_crc >> 8);
                    }
            }
        }

        public unsafe UInt32 Updatecrc32(ref uint _crc, int offset = 0, int length = -1)
        {
            lock (SyncRoot)
            {
                if (length < 0)
                    length = Count - offset;

                var bufferEnumerator = new DynamicBufferReadEnumerator(this, offset, ref length);

                fixed (uint* tabP = crc32LookupTable)
                    foreach (var bufferInfo in bufferEnumerator)
                    {
                        fixed (byte* bufP = Buffers[bufferInfo.SourceBufferIndex])
                        {
                            var buf = bufP;
                            buf += bufferInfo.SourceBufferOffset;

                            while (--bufferInfo.Length >= 0)
                                unchecked
                                {
                                    _crc = tabP[(_crc ^ *buf++) & 0xFF] ^ (_crc >> 8);
                                }
                        }
                    }

                return _crc;
            }
        }

        public static UInt32 Finishcrc32(ref uint _crc)
        {
            _crc ^= DefaultSeed;

            return _crc;
        }

        #endregion
    }
}


internal delegate void MoveCallback(DynamicBufferMoveEnumArgs e);
internal class DynamicBufferMoveEnumArgs
{
    public int SourceBufferIndex;
    public int SourceBufferOffset;
    public int TargetBufferIndex;
    public int TargetBufferOffset;
    public int Length;

}

internal class DynamicBufferMoveUpEnumerator : IEnumerator<DynamicBufferMoveEnumArgs>, IEnumerable<DynamicBufferMoveEnumArgs>
{
    DynamicBuffer Buffer;
    int FromUserIndex;
    int FromBufferIndex;
    int LengthToMove;
    int RemovedLength;
    int LengthAlreadyMoved;
    DynamicBufferMoveEnumArgs current;

    public DynamicBufferMoveUpEnumerator(DynamicBuffer buffer, int fromOffset, ref int length)
    {
        this.Buffer = buffer;
        this.FromUserIndex = fromOffset;
        this.RemovedLength = length;
        length = Math.Max(0, Math.Min(length, buffer.Count - fromOffset));
        this.LengthToMove = Math.Max(0, buffer.RealCount - (fromOffset + length));
        this.FromBufferIndex = (fromOffset + Buffer.ReadOffset) / buffer.InternalFragmentBufferSize;
    }

    public DynamicBufferMoveEnumArgs Current
    {
        get
        {
            return current;
        }
    }

    public void Dispose()
    {
        this.Buffer = null;
    }

    object System.Collections.IEnumerator.Current
    {
        get
        {
            DynamicBufferMoveEnumArgs r = this.Current;
            return r;
        }
    }

    public bool MoveNext()
    {
        int lengthStillToMove = this.LengthToMove - this.LengthAlreadyMoved;

        if (lengthStillToMove == 0)
        {
            current = null;
            return false;
        }

        int idxSource = FromUserIndex + Buffer.ReadOffset + RemovedLength + LengthAlreadyMoved;
        int idxTarget = FromUserIndex + Buffer.ReadOffset + LengthAlreadyMoved;

        int sourceBufferIndex = idxSource / Buffer.InternalFragmentBufferSize;
        int sourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize;
        int targetBufferIndex = idxTarget / Buffer.InternalFragmentBufferSize;
        int targetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize;

        int sourceLength = Math.Max(0, Math.Min(lengthStillToMove,
               sourceBufferIndex == Buffer.Buffers.Count - 1 ?
               Buffer.WriteOffset - sourceBufferOffset :
               Buffer.InternalFragmentBufferSize - sourceBufferOffset
            ));

        int targetLength = Math.Max(0, Math.Min(lengthStillToMove,
               targetBufferIndex == sourceBufferIndex ?
               sourceBufferOffset - targetBufferOffset :
               Buffer.InternalFragmentBufferSize - targetBufferOffset
            ));

        int length = Math.Min(sourceLength, targetLength);

        if (length == 0)
        {
            if (LengthAlreadyMoved < LengthToMove)
                throw new InvalidOperationException();

            current = null;
            return false;
        }

        LengthAlreadyMoved += length;

        current = new DynamicBufferMoveEnumArgs()
        {
            SourceBufferIndex = idxSource / Buffer.InternalFragmentBufferSize,
            SourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize,
            TargetBufferIndex = idxTarget / Buffer.InternalFragmentBufferSize,
            TargetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize,
            Length = length
        };

        return true;
    }

    public void Reset()
    {
        current = null;
        LengthAlreadyMoved = 0;
    }

    public IEnumerator<DynamicBufferMoveEnumArgs> GetEnumerator()
    {
        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this;
    }
}

internal class DynamicBufferMoveDownEnumerator : IEnumerator<DynamicBufferMoveEnumArgs>, IEnumerable<DynamicBufferMoveEnumArgs>
{
    DynamicBuffer Buffer;
    int FromUserIndex;
    int FromBufferIndex;
    int LengthToInsert;
    int LengthToMove;
    int LengthAlreadyMoved;
    DynamicBufferMoveEnumArgs current;

    public DynamicBufferMoveDownEnumerator(DynamicBuffer buffer, int fromOffset, int length)
    {
        this.Buffer = buffer;
        this.FromUserIndex = fromOffset;

        this.LengthToInsert = length;
        this.LengthToMove = Math.Max(0, buffer.RealCount - fromOffset);

        if (Buffer.RealCount + length > (Buffer.Buffers.Count * Buffer.InternalFragmentBufferSize))
        {
            throw new InvalidOperationException();
        }
        this.FromBufferIndex = (fromOffset + Buffer.ReadOffset) / buffer.InternalFragmentBufferSize;
    }

    public DynamicBufferMoveEnumArgs Current
    {
        get
        {
            return current;
        }
    }

    public void Dispose()
    {
        this.Buffer = null;
    }

    object System.Collections.IEnumerator.Current
    {
        get
        {
            DynamicBufferMoveEnumArgs r = this.Current;
            return r;
        }
    }

    public bool MoveNext()
    {
        if (LengthAlreadyMoved == LengthToMove)
        {
            current = null;
            return false;
        }

        int length = 0;
        int stillNeeded = LengthToMove - LengthAlreadyMoved;

        // determine the position where writing can start
        int idxTarget = (((Buffer.WriteOffset + this.LengthToInsert) - LengthAlreadyMoved) - 1);

        // in what buffer is that?
        int targetBufferIndex = idxTarget / Buffer.InternalFragmentBufferSize;
        int targetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize;

        // determine the position where reading should start
        int idxSource = ((Buffer.WriteOffset) - LengthAlreadyMoved) - 1;

        // in what buffer is that?
        int sourceBufferIndex = idxSource / Buffer.InternalFragmentBufferSize;
        int sourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize;

        // different buffer?
        if (sourceBufferIndex != targetBufferIndex)
        {
            MoveNext_DifferentBuffers(stillNeeded,
                ref idxTarget, ref targetBufferIndex, ref targetBufferOffset,
                ref idxSource, ref sourceBufferIndex, ref sourceBufferOffset,
                out length
            );
        }
        else
        {
            MoveNext_SameBuffers(stillNeeded,
                ref idxTarget, ref targetBufferIndex, ref targetBufferOffset,
                ref idxSource, ref sourceBufferIndex, ref sourceBufferOffset,
                out length
            );
        }

        if (length <= 0)
        {
            if (LengthAlreadyMoved < LengthToMove)
                throw new InvalidOperationException();

            current = null;
            return false;
        }

        LengthAlreadyMoved += length;

        current = new DynamicBufferMoveEnumArgs()
        {
            SourceBufferIndex = idxSource / Buffer.InternalFragmentBufferSize,
            SourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize,
            TargetBufferIndex = idxTarget / Buffer.InternalFragmentBufferSize,
            TargetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize,
            Length = length
        };

        return true;
    }

    void MoveNext_DifferentBuffers(int stillNeeded, ref int idxTarget, ref int targetBufferIndex, ref int targetBufferOffset, ref int idxSource, ref int sourceBufferIndex, ref int sourceBufferOffset, out int length)
    {
        // extend target global index
        idxTarget = Math.Max(
            // the last absolute index we could possible write to
            FromUserIndex + Buffer.ReadOffset + LengthToInsert,
            // beginning of buffer if possible, or less if almost finished
            idxTarget - Math.Min(stillNeeded - 1, targetBufferOffset)
        );

        // determine new buffer offset
        int newTargetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize;

        // determine length
        int targetLength = Math.Max(0, Math.Min(stillNeeded, (targetBufferOffset - newTargetBufferOffset) + 1));

        // extend source global index
        idxSource = Math.Max(
            // the last absolute index we could possible read from
            FromUserIndex + Buffer.ReadOffset,
            // beginning of buffer if possible, or less if almost finished
            idxSource - Math.Min(stillNeeded - 1, sourceBufferOffset)
        );

        // determine new buffer offset
        int newSourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize;

        // determine length
        int sourceLength = Math.Max(0, Math.Min(stillNeeded, (sourceBufferOffset - newSourceBufferOffset) + 1));

        // now take the shortest
        length = Math.Min(sourceLength, targetLength);

        // now adjust indexes

        // determine the position where writing can start
        idxTarget -= length - 1;

        // in what buffer is that?
        targetBufferIndex = idxTarget / Buffer.InternalFragmentBufferSize;
        targetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize;

        // determine the position where reading should start
        idxSource -= length - 1;

        // in what buffer is that?
        sourceBufferIndex = idxSource / Buffer.InternalFragmentBufferSize;
        sourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize;
    }

    void MoveNext_SameBuffers(int stillNeeded, ref int idxTarget, ref int targetBufferIndex, ref int targetBufferOffset, ref int idxSource, ref int sourceBufferIndex, ref int sourceBufferOffset, out int length)
    {
        // determine limits
        int lowerBoundery =
            // is it the last buffer?
            targetBufferIndex == FromBufferIndex ?

            // then insertionpoint is lower limit
            (FromUserIndex + Buffer.ReadOffset) % Buffer.InternalFragmentBufferSize :

            // or else
            0;

        int upperBoundery = targetBufferOffset;

        int count = Math.Min(stillNeeded, (upperBoundery - lowerBoundery) + 1);

        // last bit?
        if (count == 1)
        {
            length = 1;
            return;
        }

        // determine how much we can copy
        length = count / 2;

        // now adjust indexes

        // determine the position where writing can start
        //idxTarget = (((Buffer.WriteOffset + this.LengthToInsert) - LengthAlreadyMoved) - length);
        idxTarget -= length - 1;

        // in what buffer is that?
        targetBufferIndex = idxTarget / Buffer.InternalFragmentBufferSize;
        targetBufferOffset = idxTarget % Buffer.InternalFragmentBufferSize;

        // determine the position where reading should start
        //idxSource = ((Buffer.WriteOffset) - LengthAlreadyMoved) - length;
        idxSource -= length - 1;

        // in what buffer is that?
        sourceBufferIndex = idxSource / Buffer.InternalFragmentBufferSize;
        sourceBufferOffset = idxSource % Buffer.InternalFragmentBufferSize;
    }

    public void Reset()
    {
        current = null;
        LengthAlreadyMoved = 0;
    }


    public IEnumerator<DynamicBufferMoveEnumArgs> GetEnumerator()
    {
        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this;
    }
}

internal delegate void ReadCallback(DynamicBufferReadEnumArgs e);
internal class DynamicBufferReadEnumArgs
{
    public int SourceBufferIndex;
    public int SourceBufferOffset;
    public int Length;
}

internal class DynamicBufferDoubleReadEnumArgs
{
    public int Buffer1Index;
    public int Buffer1Offset;
    public int Buffer2Index;
    public int Buffer2Offset;
    public int Length;
}

internal class DynamicBufferReadEnumerator : IEnumerator<DynamicBufferReadEnumArgs>, IEnumerable<DynamicBufferReadEnumArgs>
{
    DynamicBuffer Buffer;
    int FromUserIndex;
    int FromBufferIndex;
    int Length;
    int done;
    DynamicBufferReadEnumArgs current;

    public DynamicBufferReadEnumerator(DynamicBuffer buffer, int fromOffset, ref int length)
    {
        this.Buffer = buffer;
        this.FromUserIndex = fromOffset;
        length = Math.Max(0, Math.Min(length, buffer.Count - fromOffset));
        this.Length = length;
        this.FromBufferIndex = (fromOffset + Buffer.ReadOffset) / buffer.InternalFragmentBufferSize;
    }

    public DynamicBufferReadEnumArgs Current
    {
        get
        {
            return current;
        }
    }

    public void Dispose()
    {
        this.Buffer = null;
    }

    object System.Collections.IEnumerator.Current
    {
        get
        {
            DynamicBufferReadEnumArgs r = this.Current;
            return r;
        }
    }

    public bool MoveNext()
    {
        if (done == Length)
        {
            current = null;
            return false;
        }

        int idx = FromUserIndex + Buffer.ReadOffset + done;

        int sourceBufferIndex = idx / Buffer.InternalFragmentBufferSize;
        int sourceBufferOffset = idx % Buffer.InternalFragmentBufferSize;

        int length = Math.Max(0, Math.Min(this.Length - this.done,
               sourceBufferIndex == Buffer.Buffers.Count - 1 ?
               Buffer.WriteOffset - sourceBufferOffset :
               Buffer.InternalFragmentBufferSize - sourceBufferOffset
            ));

        if (length == 0)
        {
            current = null;
            return false;
        }

        done += length;

        current = new DynamicBufferReadEnumArgs()
        {
            SourceBufferIndex = sourceBufferIndex,
            SourceBufferOffset = sourceBufferOffset,
            Length = length
        };

        return true;
    }

    public void Reset()
    {
        current = null;
        done = 0;
    }

    public IEnumerator<DynamicBufferReadEnumArgs> GetEnumerator()
    {
        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this;
    }
}

internal class DynamicBufferDoubleReadEnumerator : IEnumerator<DynamicBufferDoubleReadEnumArgs>, IEnumerable<DynamicBufferDoubleReadEnumArgs>
{
    DynamicBuffer Buffer1;
    DynamicBuffer Buffer2;
    int FromUserIndex1;
    int FromUserIndex2;
    int FromBufferIndex1;
    int FromBufferIndex2;
    int Length;
    int done;
    DynamicBufferDoubleReadEnumArgs current;

    public DynamicBufferDoubleReadEnumerator(DynamicBuffer buffer1, int fromOffset1, DynamicBuffer buffer2, int fromOffset2, ref int length)
    {
        this.Buffer1 = buffer1;
        this.Buffer2 = buffer2;

        this.FromUserIndex1 = fromOffset1;
        this.FromUserIndex2 = fromOffset2;

        this.Length = Math.Max(0, Math.Min(length, Math.Min(buffer1.Count - fromOffset1, buffer2.Count - fromOffset2)));
        length = this.Length;

        this.FromBufferIndex1 = (fromOffset1 + Buffer1.ReadOffset) / buffer1.InternalFragmentBufferSize;
        this.FromBufferIndex2 = (fromOffset2 + Buffer2.ReadOffset) / buffer2.InternalFragmentBufferSize;
    }

    public DynamicBufferDoubleReadEnumArgs Current
    {
        get
        {
            return current;
        }
    }

    public void Dispose()
    {
        this.Buffer1 = null;
        this.Buffer2 = null;
    }

    object System.Collections.IEnumerator.Current
    {
        get
        {
            DynamicBufferDoubleReadEnumArgs r = this.Current;
            return r;
        }
    }

    public bool MoveNext()
    {
        if (done == Length)
        {
            current = null;
            return false;
        }

        int idx1 = FromUserIndex1 + Buffer1.ReadOffset + done;

        int sourceBufferIndex1 = idx1 / Buffer1.InternalFragmentBufferSize;
        int sourceBufferOffset1 = idx1 % Buffer1.InternalFragmentBufferSize;

        int length1 = Math.Max(0, Math.Min(this.Length - this.done,
               sourceBufferIndex1 == Buffer1.Buffers.Count - 1 ?
               Buffer1.WriteOffset - sourceBufferOffset1 :
               Buffer1.InternalFragmentBufferSize - sourceBufferOffset1
            ));

        int idx2 = FromUserIndex2 + Buffer2.ReadOffset + done;

        int sourceBufferIndex2 = idx2 / Buffer2.InternalFragmentBufferSize;
        int sourceBufferOffset2 = idx2 % Buffer2.InternalFragmentBufferSize;

        int length2 = Math.Max(0, Math.Min(this.Length - this.done,
               sourceBufferIndex2 == Buffer2.Buffers.Count - 1 ?
               Buffer2.WriteOffset - sourceBufferOffset2 :
               Buffer2.InternalFragmentBufferSize - sourceBufferOffset2
            ));

        int length = Math.Min(length1, length2);

        if (length == 0)
        {
            current = null;
            return false;
        }

        done += length;

        current = new DynamicBufferDoubleReadEnumArgs()
        {
            Buffer1Index = sourceBufferIndex1,
            Buffer1Offset = sourceBufferOffset1,
            Buffer2Index = sourceBufferIndex2,
            Buffer2Offset = sourceBufferOffset2,
            Length = length
        };

        return true;
    }

    public void Reset()
    {
        current = null;
        done = 0;
    }

    public IEnumerator<DynamicBufferDoubleReadEnumArgs> GetEnumerator()
    {
        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this;
    }
}
