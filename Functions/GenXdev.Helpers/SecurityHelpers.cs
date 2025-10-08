// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : SecurityHelpers.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.298.2025
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
using System.Text;

namespace GenXdev.Helpers
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Sanitizes a filename by replacing invalid characters with underscores and optionally adding a unique suffix.
        /// </summary>
        /// <param name="name">The filename to sanitize.</param>
        /// <param name="giveUniqueSuffix">Whether to append a unique hash-based suffix to prevent collisions.</param>
        /// <returns>Sanitized filename safe for filesystem operations.</returns>
        public static string SanitizeFileName(string name, bool giveUniqueSuffix = false)
        {
            // get system-defined invalid path characters
            var invalidChars = Path.GetInvalidPathChars();

            // build sanitized string character by character
            var sb = new StringBuilder(name.Length);

            foreach (var c in name.ToCharArray())
            {
                // replace invalid characters with underscore
                if (Array.IndexOf(invalidChars, c) >= 0)
                {
                    sb.Append('_');
                }
                else
                    // allow only safe characters for filenames
                    switch (Char.ToLower(c))
                    {
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case 'g':
                        case 'h':
                        case 'i':
                        case 'j':
                        case 'k':
                        case 'l':
                        case 'm':
                        case 'n':
                        case 'o':
                        case 'p':
                        case 'q':
                        case 'r':
                        case 's':
                        case 't':
                        case 'u':
                        case 'v':
                        case 'w':
                        case 'x':
                        case 'y':
                        case 'z':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '0':
                        case '!':
                        case '@':
                        case '#':
                        case '$':
                        case '&':
                        case '(':
                        case ')':
                        case '_':
                        case '-':
                        case '+':
                        case '=':
                        case '{':
                        case '}':
                        case '[':
                        case ']':
                        case '.':
                        case ',':
                            sb.Append(c);
                            break;
                        default:
                            // replace unsafe characters with underscore
                            sb.Append('_');
                            break;
                    }
            }

            // append unique suffix based on hash if requested
            if (giveUniqueSuffix)
            {
                sb.Append("_" + ((UInt32)name.Trim().ToLowerInvariant().GetHashCode()).ToString().PadLeft(10, '0'));
            }

            // clean up double underscores
            return sb.ToString().Replace("__", "_");
        }

        /// <summary>
        /// Determines if a URL string points to a safe public host by checking if all resolved IP addresses are public.
        /// </summary>
        /// <param name="URL">The URL string to validate.</param>
        /// <returns>True if the URL resolves to public IP addresses only.</returns>
        public static bool IsSafePublicURL(string URL)
        {
            try
            {
                // parse URL and check host
                Uri url = new Uri(URL);
                return HostOrIPPublic(url.Host);
            }
            catch
            {
                // invalid URL format is not safe
                return false;
            }
        }

        /// <summary>
        /// Determines if a Uri object points to a safe public host by checking if all resolved IP addresses are public.
        /// </summary>
        /// <param name="URL">The Uri object to validate.</param>
        /// <returns>True if the Uri resolves to public IP addresses only.</returns>
        public static bool IsSafePublicURL(Uri URL)
        {
            // delegate to host checking logic
            return HostOrIPPublic(URL.Host);
        }

        /// <summary>
        /// Checks if a hostname resolves to public IP addresses only (not private or local).
        /// </summary>
        /// <param name="HostName">The hostname to check.</param>
        /// <returns>True if all resolved addresses are public.</returns>
        public static bool HostOrIPPublic(string HostName)
        {
            // assume public until proven otherwise
            bool result = true;
            try
            {
                // resolve hostname to IP addresses
                IPAddress[] addresslist = Dns.GetHostAddresses(HostName.Trim());

                // check each resolved address
                foreach (IPAddress address in addresslist)
                {
                    if (!IsPublicIpAddress(address))
                    {
                        // any private address makes the host not safe
                        return false;
                    }
                }
            }
            catch
            {
                // DNS resolution failure means not safe
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Asynchronously checks if a hostname resolves to public IP addresses only (not private or local).
        /// </summary>
        /// <param name="HostName">The hostname to check.</param>
        /// <returns>True if all resolved addresses are public.</returns>
        public static async Task<bool> HostOrIPPublicAsync(string HostName)
        {
            // assume public until proven otherwise
            bool result = true;
            try
            {
                // asynchronously resolve hostname to IP addresses
                var addresslist = await Task.Factory.FromAsync<IPAddress[]>(
                    Dns.BeginGetHostAddresses(HostName.Trim(), null, null),
                    Dns.EndGetHostAddresses);

                // check each resolved address
                foreach (IPAddress address in addresslist)
                {
                    if (!IsPublicIpAddress(address))
                    {
                        // any private address makes the host not safe
                        return false;
                    }
                }
            }
            catch
            {
                // DNS resolution failure means not safe
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Determines if an IP address is public (not private, local, or reserved).
        /// </summary>
        /// <param name="address">The IP address to check.</param>
        /// <returns>True if the address is public and safe for external connections.</returns>
        public static bool IsPublicIpAddress(IPAddress address)
        {
            try
            {
                // parse IPv4 address into octets
                String[] straryIPAddress = address.ToString().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                int[] iaryIPAddress = new int[] { int.Parse(straryIPAddress[0]), int.Parse(straryIPAddress[1]), int.Parse(straryIPAddress[2]), int.Parse(straryIPAddress[3]) };

                // check for private IPv4 ranges
                if (iaryIPAddress[0] == 10 ||  // 10.0.0.0/8
                    (iaryIPAddress[0] == 127 && (iaryIPAddress[1] == 0) && (iaryIPAddress[2] == 0) && (iaryIPAddress[3] == 1)) ||  // localhost
                    (iaryIPAddress[0] == 192 && iaryIPAddress[1] == 168) ||  // 192.168.0.0/16
                    (iaryIPAddress[0] == 172 && (iaryIPAddress[1] >= 16 && iaryIPAddress[1] <= 31))  // 172.16.0.0/12
                   )
                {
                    return false;
                }
            }
            catch
            {
                // parsing error means not safe
            }

            return true;
        }
    }
}
