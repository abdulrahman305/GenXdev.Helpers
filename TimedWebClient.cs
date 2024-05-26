using System.Net;

namespace GenXdev.Helpers
{
    internal class TimedWebClient : WebClient
    {
        // Timeout in milliseconds, default = 600,000 msec
        public int Timeout { get; set; }

        public TimedWebClient(TimeSpan Timeout)
        {
            this.Timeout = Convert.ToInt32(Math.Floor(Timeout.TotalMilliseconds));
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = this.Timeout;
            return objWebRequest;
        }
    }
}
