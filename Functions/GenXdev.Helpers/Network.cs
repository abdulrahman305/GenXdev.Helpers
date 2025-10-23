// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Network.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 2.1.2025
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
using System.Net.Http;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Provides network utility methods for retrieving public IP addresses and hostnames.
    /// </para>
    ///
    /// <para type="description">
    /// This static class contains methods to obtain the public external IP address
    /// by querying various online services. It includes fallback mechanisms to ensure
    /// reliability in case some services are unavailable.
    /// </para>
    /// </summary>
    public static class Network
    {
        #region public ip lookup

        static object padLock = new object();

        // static string publicHostName = "localhost";

        /// <summary>
        /// <para type="synopsis">
        /// Retrieves the public external IP address or hostname.
        /// </para>
        ///
        /// <para type="description">
        /// This method attempts to fetch the public IP address from multiple online
        /// services in order of preference. If all services fail, it falls back to
        /// the local machine's hostname or a default IP address.
        /// </para>
        ///
        /// <para type="description">
        /// PARAMETERS
        /// </para>
        ///
        /// <para type="description">
        /// -IPAddress &lt;string&gt;<br/>
        /// Optional parameter for specifying an IP address (currently not used in
        /// the implementation).<br/>
        /// - <b>Position</b>: 0<br/>
        /// - <b>Default</b>: null<br/>
        /// </para>
        ///
        /// <example>
        /// <para>Get the public IP address</para>
        /// <para>This example retrieves the public external IP address.</para>
        /// <code>
        /// string ip = Network.GetPublicExternalHostname();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="IPAddress">Optional IP address parameter (currently unused).</param>
        /// <returns>The public IP address as a string, or fallback value.</returns>
        public static string GetPublicExternalHostname(string IPAddress = null)
        {
            #region ipify

            // Attempt to retrieve the public IP from the ipify service
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

            // Fallback: Attempt to retrieve the public IP from api.ipify.org
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

            // Second fallback: Attempt to retrieve the public IP from ipinfo.io
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

            // Third fallback: Attempt to retrieve the public IP from DynDns
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

            // Final fallback: Use the local machine's hostname
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
