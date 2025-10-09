// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Email.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.300.2025
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



using System.Text;
using System.Text.RegularExpressions;

namespace GenXdev.Helpers
{
    public static class Email
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
