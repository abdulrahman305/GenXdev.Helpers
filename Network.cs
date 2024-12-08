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

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(uri).Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadAsStringAsync().Result;

                    return result.Trim();
                }
            }
            catch { }

            #endregion

            #region api.ipify.org

            try
            {
                string uri = "https://api.ipify.org";

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(uri).Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadAsStringAsync().Result;

                    return result.Trim();
                }
            }
            catch { }

            #endregion

            #region ipinfo.io

            try
            {
                string uri = "http://ipinfo.io/ip";

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(uri).Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadAsStringAsync().Result;

                    return result.Trim();
                }
            }
            catch { }

            #endregion

            #region DynDns

            try
            {
                string uri = "http://checkip.dyndns.org/";

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(uri).Result;
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadAsStringAsync().Result;

                    return result.Trim().Split(':')[1].Split('<')[0].Trim();
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
