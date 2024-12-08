/*
 * All intellectual rights of this framework, including this source file belong to Appicacy, René Vaessen.
 * Customers of Appicacy, may copy and change it, as long as this header remains.
 * 
 */

namespace GenXdev.AsyncSockets.Handlers
{
    public interface IHttpResponseHandler 
    {
        IHttpRequestSocketHandler HttpRequestSocketHandler {get; set;}
    }
}
