/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Text;
using GenXdev.Configuration;

namespace GenXdev.AsyncSockets.Containers
{
    public unsafe class HttpResponseHeaders : IHttpResponseHeaders
    {
        public byte[] MemoryBuffer;
        int CharCount;
        int CharByteCount;

        UTF8Encoding UTF8Encoding;

        public Uri Location { get; private set; }
        public bool IsValidRequest { get; private set; }
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                MemoryBuffer = null;
                CharCount = 0;
                CharByteCount = 0;
            }
        }

        ~HttpResponseHeaders()
        {
            Dispose(false);
        }

        public HttpResponseHeaders(IMemoryManagerConfiguration Configuration, byte[] MemoryBuffer, byte[] RequestBuffer, int Length, UTF8Encoding UTF8Encoding)
        {
            // store references
            this.MemoryBuffer = MemoryBuffer;
            this.UTF8Encoding = UTF8Encoding;
            this.IsValidRequest = true;

            // only really unsafe if memorybuffer is not at least 2x RequestBuffer.length due to UTF8 -> Unicode chars translation
            unsafe
            {
                // get pointer to first byte element in requestbuffer
                fixed (byte* requestBufferBytePointer = &RequestBuffer[0])
                {
                    // get pointer to first byte element in memorybuffer
                    fixed (byte* memoryBufferBytePointer = &MemoryBuffer[0])
                    {
                        // convert byte pointer to char pointer
                        char* memoryBufferCharPointer = (char*)memoryBufferBytePointer;

                        // get the unicode chars and put them in the memory buffer
                        CharCount = UTF8Encoding.GetChars(requestBufferBytePointer, Length, memoryBufferCharPointer, Convert.ToInt32(Math.Floor(MemoryBuffer.Length / 2.0)));
                        CharByteCount = CharCount * 2;
                    }
                }
            }
        }

        // make sure HeaderName parameter is lowercase!
        public string this[string HeaderName]
        {
            get
            {
                int matchCount = 0;
                int cursorBytePosition = 0;
                int valueStart = 0;
                bool inValue = false;
                bool matched = false;
                var value = new StringBuilder(50);
                bool inHeaders = false;

                // loop
                while (cursorBytePosition < CharByteCount)
                {
                    // get pointer to character at cursor
                    fixed (byte* memoryBufferBytePointer = &MemoryBuffer[cursorBytePosition])
                    {
                        // get pointer to the character at the cursor position
                        char* c = (char*)memoryBufferBytePointer;

                        // are we matching a headername?
                        if ((inHeaders) && (!inValue) && (matchCount < HeaderName.Length) && (HeaderName[matchCount] == char.ToLower(*c)))
                        {
                            // increase nr of matched characters
                            matchCount++;

                            // have matched all characters of the requested headername?
                            if (matchCount == HeaderName.Length)
                            {
                                // set flag
                                matched = true;
                            }
                        }
                        else
                        {
                            // not reached the actual headers yet?
                            if (!inHeaders)
                            {
                                // maybe now?
                                if (*c == '\n')
                                {
                                    // set the flag
                                    inHeaders = true;
                                }
                            }
                            else
                                // have we encountered the headername/value seperator?
                                if ((!inValue) && (*c == ':'))
                            {
                                // toggle the inValue flag
                                inValue = true;
                                valueStart = cursorBytePosition + 2;
                            }
                            else
                                    // encountered a space at the beginning of a header value?
                                    if (inValue && (*c == ' ') && (valueStart == cursorBytePosition))
                            {
                                // trim! 
                                valueStart = cursorBytePosition;
                            }
                            else
                                        // headername longer then requested headername?
                                        if (matched && !inValue)
                            {
                                // then we don't have a match!
                                matched = false;
                            }
                            else
                                            // append this character to the result if we are collecting the value of the requested headername
                                            if (matched && (*c != '\r')) value.Append(*c);

                            // end of value?
                            if (inValue && ((*c == '\r') || (cursorBytePosition == CharByteCount - 2)))
                            {

                                // toggle inValue flag
                                inValue = false;

                                // we were collecting the value, because this is the value of the requested headername?
                                if (matched)
                                {
                                    // return the results
                                    return value.ToString();
                                }
                            }

                            // reset the match count
                            matchCount = 0;
                        }
                    }

                    // move cursor to next unicode character
                    cursorBytePosition += 2;
                }

                return "";
            }
        }
    }
}
