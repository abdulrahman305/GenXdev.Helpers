/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
using GenXdev.Buffers;

namespace GenXdev.AsyncSockets.Containers
{
    public interface IHttpRequestHeaders : IDisposable
    {
        DynamicBuffer HeadersBuffer { get; }
        bool IsValidRequest { get; }
        string this[string HeaderName] { get; }
        string HttpVersion { get; }
        string Method { get; }
        Uri Location { get; }
    }
}
