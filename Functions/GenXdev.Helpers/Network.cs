// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Network.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.284.2025
// ################################################################################
// MIT License
//
// Copyright 2021-2025 GenXdev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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
