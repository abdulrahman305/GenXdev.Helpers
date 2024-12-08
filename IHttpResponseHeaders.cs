/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */
namespace GenXdev.AsyncSockets.Containers
{
    public interface IHttpResponseHeaders : IDisposable
    {
        string this[string HeaderName] { get; }
    }
}
