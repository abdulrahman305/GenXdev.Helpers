// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : SecurityHelpers.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.276.2025
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
        public static string SanitizeFileName(string name, bool giveUniqueSuffix = false)
        {
            var invalidChars = Path.GetInvalidPathChars();

            var sb = new StringBuilder(name.Length);

            foreach (var c in name.ToCharArray())
            {
                if (Array.IndexOf(invalidChars, c) >= 0)
                {
                    sb.Append('_');
                }
                else
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
                            sb.Append('_');
                            break;
                    }
            }

            if (giveUniqueSuffix)
            {
                sb.Append("_" + ((UInt32)name.Trim().ToLowerInvariant().GetHashCode()).ToString().PadLeft(10, '0'));
            }

            return sb.ToString().Replace("__", "_");
        }

        public static bool IsSafePublicURL(string URL)
        {
            try
            {
                Uri url = new Uri(URL);
                return HostOrIPPublic(url.Host);
            }
            catch
            {
                return false;
            }
        }
        public static bool IsSafePublicURL(Uri URL)
        {
            return HostOrIPPublic(URL.Host);
        }

        public static bool HostOrIPPublic(string HostName)
        {
            bool result = true;
            try
            {
                IPAddress[] addresslist = Dns.GetHostAddresses(HostName.Trim());

                foreach (IPAddress address in addresslist)
                {
                    if (!IsPublicIpAddress(address))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static async Task<bool> HostOrIPPublicAsync(string HostName)
        {
            bool result = true;
            try
            {
                var addresslist = await Task.Factory.FromAsync<IPAddress[]>(
                    Dns.BeginGetHostAddresses(HostName.Trim(), null, null),
                    Dns.EndGetHostAddresses);

                foreach (IPAddress address in addresslist)
                {
                    if (!IsPublicIpAddress(address))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static bool IsPublicIpAddress(IPAddress address)
        {
            try
            {
                // (address.AddressFamily == AddressFamily.InterNetwork);

                String[] straryIPAddress = address.ToString().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                int[] iaryIPAddress = new int[] { int.Parse(straryIPAddress[0]), int.Parse(straryIPAddress[1]), int.Parse(straryIPAddress[2]), int.Parse(straryIPAddress[3]) };
                if (iaryIPAddress[0] == 10 ||
                    (iaryIPAddress[0] == 127 && (iaryIPAddress[1] == 0) && (iaryIPAddress[2] == 0) && (iaryIPAddress[3] == 1)) ||
                    (iaryIPAddress[0] == 192 && iaryIPAddress[1] == 168) ||
                    (iaryIPAddress[0] == 172 && (iaryIPAddress[1] >= 16 && iaryIPAddress[1] <= 31))
                   )
                {
                    return false;
                }
            }
            catch
            {
            }

            return true;
        }
    }
}
