/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.Buffers;
using Ninject;
using System.Text;

namespace GenXdev.AsyncSockets.Containers
{
    public unsafe class HttpRequestHeaders : IHttpRequestHeaders
    {
        UTF8Encoding UTF8Encoding = new UTF8Encoding(false);

        public Uri Location { get; private set; }
        public bool IsValidRequest { get; private set; }
        public DynamicBuffer HeadersBuffer { get; private set; }

        public void Reset()
        {
            HeadersBuffer.Reset();
        }

        public void Init(bool isHttps, int HttpsListeningPort, int HttpListeningPort, DynamicBuffer RequestBuffer, int Length)
        {
            // store references
            this.IsValidRequest = true;

            // get headers
            Reset();
            RequestBuffer.MoveTo(HeadersBuffer, Length);

            try
            {
                // set location Uri
                SetLocation(isHttps, HttpsListeningPort, HttpListeningPort);
            }
            catch (HTTPRequestHeadersBadRequestException)
            {
                this.IsValidRequest = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HeadersBuffer.Dispose();
            }
        }

        ~HttpRequestHeaders()
        {
            Dispose(false);
        }

        public HttpRequestHeaders(IKernel Kernel)
        {
            // store references
            this.IsValidRequest = false;
            this.HeadersBuffer = Kernel.Get<DynamicBuffer>();
        }

        // make sure HeaderName parameter is lowercase!
        public string this[string HeaderName]
        {
            get
            {
                // find end of opening
                int start = HeadersBuffer.IndexOf((byte)'\r');

                // start there and keep going
                while (start > 0 && start < HeadersBuffer.Count - 1)
                {
                    // find end http header name
                    int endNameIdx = HeadersBuffer.IndexOf((byte)':', start + 1);

                    // get http header name
                    var endName = HeadersBuffer.ToString(UTF8Encoding, endNameIdx - start, start).Trim("\r\n ".ToCharArray());

                    // match?
                    if (endName.ToLowerInvariant() == HeaderName)
                    {
                        // find end
                        int endIdx = HeadersBuffer.IndexOf((byte)'\r', endNameIdx);

                        // or default to end of buffer
                        endIdx = endIdx < 0 ? HeadersBuffer.Count : endIdx;

                        // get value
                        return HeadersBuffer.ToString(
                            UTF8Encoding,
                            (endIdx - endNameIdx) - 1,
                            endNameIdx + 1
                        ).Trim("\r\n ".ToCharArray());
                    }

                    if (endNameIdx < 0)
                    {
                        start = HeadersBuffer.IndexOf((byte)'\r', start + 1);
                        continue;
                    }

                    // find next
                    start = HeadersBuffer.IndexOf((byte)'\r', endNameIdx);
                }

                return String.Empty;
            }
        }

        string GetMainHeaderSection(int section)
        {
            int i = HeadersBuffer.IndexOf((byte)'\r');

            if (i > 0)
            {
                return HeadersBuffer.ToString(UTF8Encoding, i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[section];
            }

            return String.Empty;
        }

        public string HttpVersion
        {
            get
            {
                return GetMainHeaderSection(2).ToUpper();
            }
        }

        public string Method
        {
            get
            {
                return GetMainHeaderSection(0).ToUpper();
            }
        }

        void SetLocation(bool isHttps, int HttpsListeningPort, int HttpListeningPort)
        {
            string location = GetMainHeaderSection(1);
            string host = this["host"];
            if (host == "") host = "localhost";
            if (host.Contains(":")) host = host.Substring(0, host.IndexOf(":"));
            try
            {
                if (location.Contains("://"))
                {
                    Location = new Uri(location, UriKind.Absolute);
                }
                else
                {
                    if (isHttps)
                    {
                        Location = new Uri("https://" + host + ":" + HttpsListeningPort.ToString() + location, UriKind.Absolute);
                    }
                    else
                    {
                        Location = new Uri("http://" + host + ":" + HttpListeningPort.ToString() + location, UriKind.Absolute);
                    }
                }
            }
            catch (Exception e)
            {
                throw new HTTPRequestHeadersBadRequestException("Bad Request", e);
            }
        }
    }

    [Serializable]
    public class HTTPRequestHeadersBadRequestException : Exception
    {
        public HTTPRequestHeadersBadRequestException(String Message, Exception InnerException = null) : base(Message, InnerException) { }
    };

}
