using GenXdev.Buffers;
using System.Text;

namespace GenXdev.Helpers
{
    public static class BufferHelpers
    {
        public static byte[] Concat(byte[] buffer1, byte[] buffer2)
        {
            var result = new byte[buffer1.Length + buffer2.Length];

            Buffer.BlockCopy(buffer1, 0, result, 0, buffer1.Length);
            Buffer.BlockCopy(buffer2, 0, result, buffer1.Length, buffer2.Length);

            return result;
        }

        public static bool TryGetStringFromBuffer(ref int? Count, DynamicBuffer Buffer, out string Value, int maxLength)
        {
            // init output variaible
            Value = null;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int))
            {
                Count = sizeof(int) - Buffer.Count;
                return false;
            }

            // read string length
            Buffer.Position = 0;
            var stringLength = Buffer.Reader.ReadInt32();

            if (stringLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check

            {
                if (stringLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("String is too long, possibly a client speaking an incompatible protocol.");
                }
            }

            // does not have the whole string in the buffer?
            if (Buffer.Count < sizeof(int) + stringLength)
            {
                Count = (sizeof(int) + stringLength) - Buffer.Count;
                return false;
            }

            // remove length
            Buffer.Remove(sizeof(int));

            // convert it
            Value = Encoding.UTF8.GetString(

                Buffer.RemoveBytes(stringLength)
            );

            return true;
        }

        public static bool TryGetStringAndInt32FromBuffer(ref int? Count, DynamicBuffer Buffer, out string Value, out Int32 IntValue, int maxLength)
        {
            // init output variaible
            Value = null;
            IntValue = 0;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int))
            {
                Count = sizeof(int) - Buffer.Count;
                return false;
            }

            // read string length
            Buffer.Position = 0;
            var stringLength = Buffer.Reader.ReadInt32();

            if (stringLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check
            if (stringLength > maxLength)
            {
                // disconnect client
                throw new InvalidOperationException("String is too long, possibly a client speaking an incompatible protocol.");
            }

            // does not have the whole string in the buffer?
            if (Buffer.Count < sizeof(int) + stringLength + sizeof(Int32))
            {
                Count = (sizeof(int) + stringLength + sizeof(Int32)) - Buffer.Count;
                return false;
            }

            // remove length
            Buffer.Remove(sizeof(int));

            // convert it
            Value = Encoding.UTF8.GetString(

                Buffer.RemoveBytes(stringLength)
            );

            // get Int32 value
            var pos = Buffer.Position;
            Buffer.Position = 0;
            IntValue = Buffer.Reader.ReadInt32();
            Buffer.Position = pos;
            Buffer.Remove(sizeof(Int32));

            return true;
        }

        public static bool TryGetStringFromBuffer(DynamicBuffer Buffer, out string Value, int maxLength)
        {
            // init output variaible
            Value = null;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int))
            {
                return false;
            }

            // read string length
            Buffer.Position = 0;
            var stringLength = Buffer.Reader.ReadInt32();

            if (stringLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check

            {
                if (stringLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("String is too long, possibly a client speaking an incompatible protocol.");
                }
            }

            // does not have the whole string in the buffer?
            if (Buffer.Count < sizeof(int) + stringLength)
            {
                return false;
            }

            // remove length
            Buffer.Remove(sizeof(int));

            // convert it
            Value = Encoding.UTF8.GetString(

                Buffer.RemoveBytes(stringLength)
            );

            return true;
        }

        public static bool TryGetStringAndInt32FromBuffer(DynamicBuffer Buffer, out string Value, out Int32 IntValue, int maxLength)
        {
            // init output variaible
            Value = null;
            IntValue = 0;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int))
            {
                return false;
            }

            // read string length
            Buffer.Position = 0;
            var stringLength = Buffer.Reader.ReadInt32();

            if (stringLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check
            if (stringLength > maxLength)
            {
                // disconnect client
                throw new InvalidOperationException("String is too long, possibly a client speaking an incompatible protocol.");
            }

            // does not have the whole string in the buffer?
            if (Buffer.Count < sizeof(int) + stringLength + sizeof(Int32))
            {
                return false;
            }

            // remove length
            Buffer.Remove(sizeof(int));

            // convert it
            Value = Encoding.UTF8.GetString(

                Buffer.RemoveBytes(stringLength)
            );

            // get Int32 value
            var pos = Buffer.Position;
            Buffer.Position = 0;
            IntValue = Buffer.Reader.ReadInt32();
            Buffer.Position = pos;
            Buffer.Remove(sizeof(Int32));

            return true;
        }

        public static bool TryGetStringFromBuffer(ref int? Count, DynamicBuffer Buffer, out bool FirstResultByte, out string Value, int maxLength)
        {
            // init output variaible
            Value = null;
            FirstResultByte = false;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int) + 1)
            {
                Count = (sizeof(int) + 1) - Buffer.Count;
                return false;
            }

            // read string length
            Buffer.Position = 0;
            FirstResultByte = Buffer.Reader.ReadByte() == 1;
            var stringLength = Buffer.Reader.ReadInt32();

            if (stringLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check

            {
                if (stringLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("String is too long, possibly a client speaking an incompatible protocol.");
                }
            }

            // does not have the whole string in the buffer?
            if (Buffer.Count < (sizeof(int) + 1) + stringLength)
            {
                Count = ((sizeof(int) + 1) + stringLength) - Buffer.Count;

                return false;
            }

            // remove length + result byte
            Buffer.Remove(sizeof(int) + 1);

            // convert it
            Value = Encoding.UTF8.GetString(

                Buffer.RemoveBytes(stringLength)
            );

            return true;
        }

        public static bool TryGetStringAndInt32FromBuffer(ref int? Count, DynamicBuffer Buffer, out bool FirstResultByte, out string Value, out Int32 IntValue, int maxLength)
        {
            // init output variaible
            Value = null;
            IntValue = 0;
            FirstResultByte = false;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int) + 1)
            {
                Count = (sizeof(int) + 1) - Buffer.Count;
                return false;
            }

            // read string length
            Buffer.Position = 0;
            FirstResultByte = Buffer.Reader.ReadByte() == 1;
            var stringLength = Buffer.Reader.ReadInt32();

            if (stringLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check
            if (stringLength > maxLength)
            {
                // disconnect client
                throw new InvalidOperationException("String is too long, possibly a client speaking an incompatible protocol.");
            }

            // does not have the whole string in the buffer?
            if (Buffer.Count < (sizeof(int) + 1) + stringLength + sizeof(Int32))
            {
                Count = ((sizeof(int) + 1) + stringLength + sizeof(Int32)) - Buffer.Count;

                return false;
            }

            // remove length + result byte
            Buffer.Remove(sizeof(int) + 1);

            // convert it
            Value = Encoding.UTF8.GetString(

                Buffer.RemoveBytes(stringLength)
            );

            // get Int32 value
            var pos = Buffer.Position;
            Buffer.Position = 0;
            IntValue = Buffer.Reader.ReadInt32();
            Buffer.Position = pos;
            Buffer.Remove(sizeof(Int32));

            return true;
        }

        public static bool TryGetBytesFromBuffer(ref int? Count, DynamicBuffer Buffer, out byte[] Bytes, int maxLength)
        {
            // init output variaible
            Bytes = null;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int))
            {
                Count = (sizeof(int)) - Buffer.Count;

                return false;
            }

            // read length
            Buffer.Position = 0;
            var packetLength = Buffer.Reader.ReadInt32();

            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check

            {
                if (packetLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("Length is too long.");
                }
            }

            // does not have the whole packet in the buffer?
            if (Buffer.Length < sizeof(int) + packetLength)
            {
                Count = (sizeof(int) + packetLength) - Buffer.Count;

                return false;
            }

            // createbuffer
            Bytes = new byte[packetLength];

            // remove length
            Buffer.Remove(sizeof(int));

            // remove from buffer
            Buffer.Remove(Bytes, 0, packetLength);

            return true;
        }

        public static bool TryGetBytesFromBuffer(DynamicBuffer Buffer, out byte[] Bytes, int maxLength)
        {
            // init output variaible
            Bytes = null;

            // does not have length value in buffer?
            if (Buffer.Count < sizeof(int))
            {
                return false;
            }

            // read string length
            Buffer.Position = 0;
            var packetLength = Buffer.Reader.ReadInt32();

            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length int too small < 0, possibly a client speaking an incompatible protocol.");
            }

            // check

            {
                if (packetLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("Length is too long.");
                }
            }

            // does not have the whole packet in the buffer?
            if (Buffer.Length < sizeof(int) + packetLength)
            {
                return false;
            }

            // createbuffer
            Bytes = new byte[packetLength];

            // remove length
            Buffer.Remove(sizeof(int));

            // remove from buffer
            Buffer.Remove(Bytes, 0, packetLength);

            return true;
        }

        public static void WriteBytesToBuffer(DynamicBuffer Buffer, byte[] Bytes, int offset = 0, int? length = null)
        {
            // init length
            length = length.HasValue ? length.Value : Bytes.Length;

            // limit length
            length = Math.Min(Bytes.Length - offset, length.Value);

            // write length 
            Buffer.Writer.Write(length.Value);
            Buffer.Writer.Flush();

            // write content
            Buffer.Add(Bytes, offset, length.Value);
        }

        public static void WriteStringToBuffer(DynamicBuffer Buffer, string Value)
        {
            var bytes = (new UTF8Encoding(false)).GetBytes(Value);
            Buffer.Writer.Write(bytes.Length);
            Buffer.Writer.Flush();
            Buffer.Add(bytes);
        }

        public static void WriteObjectJsonToBuffer(DynamicBuffer Buffer, object objectToWrite)
        {
            WriteStringToBuffer(Buffer, GenXdev.Helpers.Serialization.ToJson(objectToWrite));
        }

        public static bool TryGetObjectFromBuffer<T>(ref int? Count, DynamicBuffer Buffer, out T objectRead, int maxLength)
        {
            // init 
            objectRead = default(T);
            string json = String.Empty;

            // got the whole json string out of the buffer?
            if (TryGetStringFromBuffer(ref Count, Buffer, out json, maxLength))
            {
                // de-serialize it, throw exception if failed, should never occur
                objectRead = GenXdev.Helpers.Serialization.FromJson<T>(json);

                return true;
            }

            return false;
        }

        public static bool TryGetBytesFromBuffer(ref int? Count, DynamicBuffer buffer, out byte[] bytes, out int additionalInt, int maxLength)
        {
            // init output variaible
            bytes = null;
            additionalInt = 0;

            // does not have length value in buffer?
            if (buffer.Count < sizeof(int) * 2)
            {
                Count = (sizeof(int) * 2) - buffer.Count;
                return false;
            }

            // read string length
            buffer.Position = 0;
            additionalInt = buffer.Reader.ReadInt32();
            var packetLength = buffer.Reader.ReadInt32();

            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }

            // check

            if (packetLength > maxLength)
            {
                // disconnect client
                throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }


            // does not have the whole in the buffer?
            if (buffer.Length < (sizeof(int) * 2) + packetLength)
            {
                Count = ((sizeof(int) * 2) + packetLength) - buffer.Count;

                return false;
            }

            // remove integers
            buffer.Remove(sizeof(int) * 2);

            // create buffer
            bytes = new byte[packetLength];

            // remove from buffer
            buffer.Remove(bytes, 0, packetLength);

            return true;
        }

        public static bool TryGetBytesFromBuffer(ref int? Count, DynamicBuffer buffer, out byte[] bytes, out string additionalString, int maxLength, int maxStringLength)
        {
            // init output variaible
            bytes = null;
            additionalString = "";

            // does not have length value in buffer?
            if (buffer.Count < sizeof(int) * 2)
            {
                Count = (sizeof(int) * 2) - buffer.Count;
                return false;
            }

            // read string length
            buffer.Position = 0;
            var additionalStrLength = buffer.Reader.ReadInt32();
            var packetLength = buffer.Reader.ReadInt32();
            var requiredBytes = (sizeof(int) * 2) + additionalStrLength + packetLength;

            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }
            if (additionalStrLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + additionalStrLength.ToString());
            }

            // check
            if (packetLength > maxLength)
            {
                // disconnect client
                throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }
            if (additionalStrLength > maxStringLength)
            {
                // disconnect client
                throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol, maxlength: " + maxStringLength.ToString() + ", received string length: " + additionalStrLength.ToString());
            }
            if (buffer.Count < requiredBytes)
            {
                Count = requiredBytes;
                return false;
            }

            // remove integers
            buffer.Remove(sizeof(int) * 2);

            if (additionalStrLength > 0)
            {
                additionalString = Encoding.UTF8.GetString(buffer.RemoveBytes(additionalStrLength));
            }

            // create buffer
            bytes = new byte[packetLength];

            // remove from buffer
            buffer.Remove(bytes, 0, packetLength);

            return true;
        }

        public static int TryGetBytesFromBuffer(ref int? Count, byte[] bytes, DynamicBuffer buffer, out int additionalInt, int maxLength)
        {
            // init output variaible            
            additionalInt = 0;

            // does not have length value in buffer?
            if (buffer.Count < sizeof(int) * 2)
            {
                Count = (sizeof(int) * 2) - buffer.Count;

                return 0;
            }

            // read additional int
            buffer.Position = 0;
            additionalInt = buffer.Reader.ReadInt32();


            // read length
            var packetLength = buffer.Reader.ReadInt32();

            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }

            // check


            if (packetLength > maxLength)
            {
                // disconnect client
                throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol.");
            }


            // check buffer
            if (bytes.Length < packetLength)
                throw new InvalidOperationException("Provided buffer to small");

            // does not have the it whole in the buffer?
            if (buffer.Length < (sizeof(int) * 2) + packetLength)
            {
                Count = ((sizeof(int) * 2) + packetLength) - buffer.Count;

                return 0;
            }

            // remove integers
            buffer.Remove(sizeof(int) * 2);

            // remove from buffer
            return buffer.Remove(bytes, 0, packetLength);
        }

        public static int TryGetBytesFromBuffer(ref int? Count, byte[] bytes, DynamicBuffer buffer, out string additionalString, int maxLength, int maxStringLength)
        {
            // init output variaible
            bytes = null;
            additionalString = "";

            // does not have length value in buffer?
            if (buffer.Count < sizeof(int) * 2)
            {
                Count = (sizeof(int) * 2) - buffer.Count;
                return 0;
            }

            // read string length
            buffer.Position = 0;
            var additionalStrLength = buffer.Reader.ReadInt32();
            var packetLength = buffer.Reader.ReadInt32();
            var requiredBytes = (sizeof(int) * 2) + additionalStrLength + packetLength;

            if (additionalStrLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + additionalStrLength.ToString());
            }
            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }
            // check

            {
                if (packetLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
                }
                if (additionalStrLength > maxStringLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol, maxlength: " + maxStringLength.ToString() + ", received string length: " + additionalStrLength.ToString());
                }
            }

            // check buffer
            if (bytes.Length < packetLength)
                throw new InvalidOperationException("Provided buffer to small");

            // does not have the it whole in the buffer?
            if (buffer.Length < (sizeof(int) * 2) + (packetLength + additionalStrLength))
            {
                Count = ((sizeof(int) * 2) + packetLength) - buffer.Count;

                return 0;
            }

            // remove integers
            buffer.Remove(sizeof(int) * 2);

            if (additionalStrLength > 0)
            {
                additionalString = Encoding.UTF8.GetString(buffer.RemoveBytes(additionalStrLength));
            }

            // remove from buffer
            return buffer.Remove(bytes, 0, packetLength);
        }

        public static int TryGetBytesFromBuffer(byte[] bytes, DynamicBuffer buffer, out int additionalInt, int maxLength)
        {
            // init output variaible            
            additionalInt = 0;

            // does not have length value in buffer?
            if (buffer.Count < sizeof(int) * 2)
            {
                return 0;
            }

            // read additional int
            buffer.Position = 0;
            additionalInt = buffer.Reader.ReadInt32();

            // read length
            var packetLength = buffer.Reader.ReadInt32();

            if (packetLength < 0)
            {
                // disconnect client
                throw new InvalidOperationException("Length is less then zero, possibly a client speaking an incompatible protocol, maxlength: " + maxLength.ToString() + ", received packet length: " + packetLength.ToString());
            }

            // check

            {
                if (packetLength > maxLength)
                {
                    // disconnect client
                    throw new InvalidOperationException("Length is too long, possibly a client speaking an incompatible protocol.");
                }
            }

            // check buffer
            if (bytes.Length < packetLength)
                throw new InvalidOperationException("Provided buffer to small");

            // does not have the it whole in the buffer?
            if (buffer.Length < (sizeof(int) * 2) + packetLength)
            {
                return 0;
            }

            // remove integers
            buffer.Remove(sizeof(int) * 2);

            // remove from buffer
            return buffer.Remove(bytes, 0, packetLength);
        }

        public static void WriteBytesToBuffer(DynamicBuffer buffer, byte[] bytes, int additionalInt, int offset = 0, int? length = null)
        {
            // init length
            int maxBytes = length.HasValue ? length.Value : bytes.Length;

            // limit length
            maxBytes = Math.Min(maxBytes, bytes.Length - offset);

            // write additional int
            buffer.Writer.Write(additionalInt);

            // write length 
            buffer.Writer.Write(maxBytes);
            buffer.Writer.Flush();

            // write content
            buffer.Add(bytes, offset, maxBytes);
        }

        public static void WriteBytesToBuffer(DynamicBuffer buffer, byte[] bytes, string additionalString, int offset = 0, int? length = null)
        {
            // init length
            int maxBytes = length.HasValue ? length.Value : bytes.Length;

            // limit length
            maxBytes = Math.Min(maxBytes, bytes.Length - offset);

            if (additionalString.Length > 0)
            {
                var strBytes = (new UTF8Encoding(false)).GetBytes(additionalString);

                // write length
                buffer.Writer.Write(strBytes.Length);
                buffer.Writer.Flush();

                // write length 
                buffer.Writer.Write(maxBytes);
                buffer.Writer.Flush();

                // add string
                buffer.Add(strBytes);
            }
            else
            {
                // write additional int
                buffer.Writer.Write(additionalString.Length);

                // write length 
                buffer.Writer.Write(maxBytes);
                buffer.Writer.Flush();
            }

            // write content
            buffer.Add(bytes, offset, maxBytes);
        }

        public static unsafe bool ByteArraysAreEqual(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
                return false;
            if (buffer1.Length == 0)
                return true;

            fixed (byte* pb1 = buffer1)
            fixed (byte* pb2 = buffer2)
            {
                byte* b1 = pb1;
                byte* b2 = pb2;

                for (var i = 0; i < buffer1.Length; i++)
                {
                    if (*b1++ != *b2++)
                        return false;
                }
            }

            return true;
        }
    }
}
