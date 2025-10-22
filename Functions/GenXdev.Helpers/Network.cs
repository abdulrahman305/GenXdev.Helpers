// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Network.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.308.2025
// ################################################################################
// Copyright (c)  René Vaessen / GenXdev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ################################################################################



using System.Net;

namespace GenXdev.Helpers
{
    public static class Network
    {
        #region public ip lookup

        static object padLock = new object();
        // static string publicHostName = "localhost";

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
