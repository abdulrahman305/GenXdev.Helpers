using System.Net;

namespace GenXdev.Helpers
{
    public static class Network
    {
        #region public ip lookup

        static object padLock = new object();
        static string publicHostName = "localhost";

        public static string GetPublicExternalHostname(string IPAddress = null)
        {
            #region ipify

            try
            {
                string uri = "https://api.ipify.org";

                using (var client = new TimedWebClient(TimeSpan.FromSeconds(20)))
                {
                    return client.DownloadString(uri).Trim();
                }
            }
            catch { }

            #endregion

            #region api.ipify.org

            try
            {
                string uri = "https://api.ipify.org";

                using (var client = new TimedWebClient(TimeSpan.FromSeconds(20)))
                {
                    return client.DownloadString(uri).Trim();                                        
                }
            }
            catch { }

            #endregion

            #region ipinfo.io

            try
            {
                string uri = "http://ipinfo.io/ip";

                using (var client = new TimedWebClient(TimeSpan.FromSeconds(20)))
                {
                    return client.DownloadString(uri).Trim();
                }
            }
            catch { }

            #endregion

            #region DynDns

            try
            {
                string uri = "http://checkip.dyndns.org/";

                using (var client = new TimedWebClient(TimeSpan.FromSeconds(20)))
                {
                    var result = client.DownloadString(uri);

                    return result.Split(':')[1].Split('<')[0].Trim();
                }
            }
            catch { }

            #endregion

            #region Local Machine Hostname

            try
            {
                lock (padLock)
                    return Dns.GetHostName().ToLower();
            }
            catch { }

            #endregion

            return "127.0.0.1";
        }

        #endregion
    }
}
