// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : EmailHelpers.cs
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



using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GenXdev.Helpers
{
    /// <summary>
    /// Provides static helper methods for email address validation and manipulation.
    /// </summary>
    public static class EmailHelpers
    {
        /// <summary>
        /// Validates whether the provided string is a valid email address format.
        /// </summary>
        /// <param name="email">The email address string to validate.</param>
        /// <returns>True if the email address is valid, otherwise false.</returns>
        public static bool IsValidEmailAddress(string email)
        {

            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
              + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
              + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        /// <summary>
        /// Escapes special characters in the value for use in ESMTP MAIL or RCPT command parameters.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        public static string EscapeESMTPMailOrRcptParamValue(string value)
        {
            EscapeESMTPMailOrRcptParamValue(ref value);

            return value;
        }

        /// <summary>
        /// Unescapes previously escaped characters in the value from ESMTP MAIL or RCPT command parameters.
        /// </summary>
        /// <param name="value">The value to unescape.</param>
        /// <returns>The unescaped value.</returns>
        public static string UnescapeESMTPMailOrRcptParamValue(string value)
        {
            UnescapeESMTPMailOrRcptParamValue(ref value);

            return value;
        }

        /// <summary>
        /// Escapes special characters in the value for use in ESMTP MAIL or RCPT command parameters, modifying the input string.
        /// </summary>
        /// <param name="value">The value to escape, modified in place.</param>
        public static void EscapeESMTPMailOrRcptParamValue(ref string value)
        {

            var sb = new StringBuilder(value);

            // Iterate backwards to safely modify the string during replacement
            for (var i = sb.Length - 1; i >= 0; i--)
            {

                char c = sb[i];

                // Check if character requires escaping
                if ((c < '!') || (c > '~') || (c == '+') || (c == '='))
                {

                    sb.Remove(i, 1);
                    sb.Insert(i, "+" + Uri.HexEscape(c).Substring(1, 2));
                }
            }

            value = sb.ToString();
        }

        /// <summary>
        /// Unescapes previously escaped characters in the value from ESMTP MAIL or RCPT command parameters, modifying the input string.
        /// </summary>
        /// <param name="value">The value to unescape, modified in place.</param>
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

        /// <summary>
        /// Compares two email addresses for equality, ignoring case and whitespace.
        /// </summary>
        /// <param name="address1">The first email address.</param>
        /// <param name="address2">The second email address.</param>
        /// <returns>True if the addresses are equal, otherwise false.</returns>
        public static bool EmailAddressesAreEqual(string address1, string address2)
        {
            return address1.Trim().ToLowerInvariant().Equals(address2.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Checks if the specified email address is contained in the provided array of email addresses.
        /// </summary>
        /// <param name="emailAddressList">The array of email addresses to search.</param>
        /// <param name="emailAddress">The email address to find.</param>
        /// <returns>True if the email address is found, otherwise false.</returns>
        public static bool ContainsEmailAddress(string[] emailAddressList, string emailAddress)
        {

            foreach (var address in emailAddressList)
            {

                if (EmailAddressesAreEqual(address, emailAddress))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts the domain name from an email address.
        /// </summary>
        /// <param name="emailAddress">The email address to parse.</param>
        /// <returns>The domain name, or "anonymous" if no domain, or the original string if no @.</returns>
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

        /// <summary>
        /// Extracts the account name (local part) from an email address.
        /// </summary>
        /// <param name="emailAddress">The email address to parse.</param>
        /// <returns>The account name, or the original string if no @.</returns>
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

            // return account name
            return emailAddress.Substring(0, idx).Trim();
        }
    }
}
