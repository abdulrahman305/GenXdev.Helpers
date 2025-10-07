// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : EmailHelpers.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.292.2025
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



using System.Text;
using System.Text.RegularExpressions;

namespace GenXdev.Helpers
{
    public static class EmailHelpers
    {
        public static bool IsValidEmailAddress(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
              + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
              + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        public static string EscapeESMTPMailOrRcptParamValue(string value)
        {
            EscapeESMTPMailOrRcptParamValue(ref value);

            return value;
        }

        public static string UnescapeESMTPMailOrRcptParamValue(string value)
        {
            UnescapeESMTPMailOrRcptParamValue(ref value);

            return value;
        }

        public static void EscapeESMTPMailOrRcptParamValue(ref string value)
        {
            var sb = new StringBuilder(value);

            for (var i = sb.Length - 1; i >= 0; i--)
            {
                char c = sb[i];

                if ((c < '!') || (c > '~') || (c == '+') || (c == '='))
                {
                    sb.Remove(i, 1);
                    sb.Insert(i, "+" + Uri.HexEscape(c).Substring(1, 2));
                }
            }

            value = sb.ToString();
        }

        public static void UnescapeESMTPMailOrRcptParamValue(ref string value)
        {
            var sb = new StringBuilder(value);

            var i = 0;
            while (i < sb.Length)
            {
                char c = sb[i];

                if (c == '+')
                {
                    sb.Remove(i, 1);
                    int i2 = 0;
                    sb.Insert(i, Uri.HexUnescape("%" + sb.Remove(i, 2).ToString(), ref i2));
                }
                i++;
            }

            value = sb.ToString();
        }

        public static bool EmailAddressesAreEqual(string address1, string address2)
        {
            return address1.Trim().ToLowerInvariant().Equals(address2.Trim().ToLowerInvariant());
        }

        public static bool ContainsEmailAddress(string[] emailAddressList, string emailAddress)
        {
            foreach (var address in emailAddressList)
            {
                if (EmailAddressesAreEqual(address, emailAddress))
                    return true;
            }

            return false;
        }

        public static string GetDomainNameFromEmailAddress(string emailAddress)
        {
            // no contents?
            if (String.IsNullOrWhiteSpace(emailAddress))
                return String.Empty;

            // remove whitespace
            emailAddress = emailAddress.Trim();

            // find @
            int idx = emailAddress.LastIndexOf("@");

            // nothing follows?
            if ((idx >= 0) && (idx == emailAddress.Length - 1))
                return "anonymous";

            // not found?
            if (idx < 0)
                return emailAddress;

            // return domainname
            return emailAddress.Substring(idx + 1).ToLowerInvariant().Trim();
        }

        public static string GetAccountNameFromEmailAddress(string emailAddress)
        {
            // no contents?
            if (String.IsNullOrWhiteSpace(emailAddress))
                return String.Empty;

            // remove whitespace
            emailAddress = emailAddress.Trim();

            // find @
            int idx = emailAddress.LastIndexOf("@");

            // not found or nothing follows?
            if (idx < 0)
                return emailAddress;

            // return domainname
            return emailAddress.Substring(0, idx).Trim();
        }
    }
}
