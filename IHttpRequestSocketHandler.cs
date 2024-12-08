/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using System.Text;
using GenXdev.AsyncSockets.Containers;
using GenXdev.AsyncSockets.Configuration;

namespace GenXdev.AsyncSockets.Handlers
{
    public interface IHttpRequestSocketHandler
    {
        ISocketHandlerTLSConfiguration TLSConfiguration { get; }

        // headers
        IHttpRequestHeaders HttpRequestHeaders { get; }

        // encoding
        UTF8Encoding UTF8Encoding { get; }

        // compression
        SuitableContentEncoding GetSuitableContentEncoding();
        Stream GetCompressionStream(SuitableContentEncoding encoding, Stream ResponseStream);

        //double HandlerStartTimestamp { get; }
        double RequestStartTimestamp { get; }
        string ServiceName { get; }

        // flags
        bool IsFirstRequest { get; }

        // configuration
        IHttpSocketHandlerConfiguration HttpConfiguration { get; }
    }
}
