// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : Email.cs
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



using System.Text;
using System.Text.RegularExpressions;

namespace GenXdev.Helpers
{
    /// <summary>
    /// <para type="synopsis">
    /// Provides static utility methods for email address validation and manipulation.
    /// </para>
    ///
    /// <para type="description">
    /// The Email class contains helper methods for working with email addresses,
    /// including validation, escaping/unescaping for ESMTP parameters, address comparison,
    /// and extracting domain or account names from email addresses.
    /// </para>
    /// </summary>
    public static class Email
    {
        /// <summary>
        /// Validates whether the provided string is a valid email address format.
        /// </summary>
        /// <param name="email">The email address string to validate.</param>
        /// <returns>True if the email address matches the expected format, otherwise false.</returns>
        public static bool IsValidEmailAddress(string email)
        {
            // Define the regular expression pattern for email validation
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
              + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
              + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            // Create regex object with case-insensitive matching
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Return whether the email matches the pattern
            return regex.IsMatch(email);
        }

        /// <summary>
        /// Escapes a value for use in ESMTP MAIL FROM or RCPT TO parameters.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        public static string EscapeESMTPMailOrRcptParamValue(string value)
        {
            // Call the ref version to perform the escaping
            EscapeESMTPMailOrRcptParamValue(ref value);

            // Return the modified value
            return value;
        }

        /// <summary>
        /// Unescapes a value that was previously escaped for ESMTP parameters.
        /// </summary>
        /// <param name="value">The value to unescape.</param>
        /// <returns>The unescaped value.</returns>
        public static string UnescapeESMTPMailOrRcptParamValue(string value)
        {
            // Call the ref version to perform the unescaping
            UnescapeESMTPMailOrRcptParamValue(ref value);

            // Return the modified value
            return value;
        }

        /// <summary>
        /// Escapes special characters in a value for ESMTP MAIL FROM or RCPT TO parameters.
        /// Modifies the input string in place.
        /// </summary>
        /// <param name="value">The value to escape, modified by reference.</param>
        public static void EscapeESMTPMailOrRcptParamValue(ref string value)
        {
            // Create a string builder from the input value
            var sb = new StringBuilder(value);

            // Iterate backwards through the string to handle replacements
            for (var i = sb.Length - 1; i >= 0; i--)
            {
                // Get the current character
                char c = sb[i];

                // Check if character needs escaping (control chars, non-ASCII, +, or =)
                if ((c < '!') || (c > '~') || (c == '+') || (c == '='))
                {
                    // Remove the character and insert its hex-encoded version
                    sb.Remove(i, 1);
                    sb.Insert(i, "+" + Uri.HexEscape(c).Substring(1, 2));
                }
            }

            // Update the original value with the escaped string
            value = sb.ToString();
        }

        /// <summary>
        /// Unescapes special characters in a value that was escaped for ESMTP parameters.
        /// Modifies the input string in place.
        /// </summary>
        /// <param name="value">The value to unescape, modified by reference.</param>
        public static void UnescapeESMTPMailOrRcptParamValue(ref string value)
        {
            // Create a string builder from the input value
            var sb = new StringBuilder(value);

            // Initialize index for iteration
            var i = 0;

            // Iterate through the string to find and replace escape sequences
            while (i < sb.Length)
            {
                // Get the current character
                char c = sb[i];

                // Check if this is an escape sequence start (+)
                if (c == '+')
                {
                    // Remove the + character
                    sb.Remove(i, 1);

                    // Extract the next two characters as hex
                    int i2 = 0;
                    sb.Insert(i, Uri.HexUnescape("%" + sb.Remove(i, 2).ToString(), ref i2));
                }

                // Move to the next character
                i++;
            }

            // Update the original value with the unescaped string
            value = sb.ToString();
        }

        /// <summary>
        /// Compares two email addresses for equality, ignoring case and whitespace.
        /// </summary>
        /// <param name="address1">The first email address to compare.</param>
        /// <param name="address2">The second email address to compare.</param>
        /// <returns>True if the addresses are equal, otherwise false.</returns>
        public static bool EmailAddressesAreEqual(string address1, string address2)
        {
            // Trim whitespace and convert to lowercase for comparison
            return address1.Trim().ToLowerInvariant().Equals(address2.Trim().ToLowerInvariant());
        }

        /// <summary>
        /// Checks if a given email address exists in an array of email addresses.
        /// </summary>
        /// <param name="emailAddressList">The array of email addresses to search.</param>
        /// <param name="emailAddress">The email address to find.</param>
        /// <returns>True if the email address is found in the list, otherwise false.</returns>
        public static bool ContainsEmailAddress(string[] emailAddressList, string emailAddress)
        {
            // Iterate through each address in the list
            foreach (var address in emailAddressList)
            {
                // Check if current address matches the target
                if (EmailAddressesAreEqual(address, emailAddress))
                    return true;
            }

            // Return false if no match found
            return false;
        }

        /// <summary>
        /// Extracts the domain name from an email address.
        /// </summary>
        /// <param name="emailAddress">The email address to parse.</param>
        /// <returns>The domain name part of the email address, or "anonymous" if no domain, or the original string if no @ found.</returns>
        public static string GetDomainNameFromEmailAddress(string emailAddress)
        {
            // Check if input is null or whitespace
            if (String.IsNullOrWhiteSpace(emailAddress))
                return String.Empty;

            // Remove leading/trailing whitespace
            emailAddress = emailAddress.Trim();

            // Find the last @ symbol
            int idx = emailAddress.LastIndexOf("@");

            // If @ is at the end, return "anonymous"
            if ((idx >= 0) && (idx == emailAddress.Length - 1))
                return "anonymous";

            // If no @ found, return the whole string
            if (idx < 0)
                return emailAddress;

            // Return the domain part after @
            return emailAddress.Substring(idx + 1).ToLowerInvariant().Trim();
        }

        /// <summary>
        /// Extracts the account name (local part) from an email address.
        /// </summary>
        /// <param name="emailAddress">The email address to parse.</param>
        /// <returns>The account name part before the @ symbol, or the original string if no @ found.</returns>
        public static string GetAccountNameFromEmailAddress(string emailAddress)
        {
            // Check if input is null or whitespace
            if (String.IsNullOrWhiteSpace(emailAddress))
                return String.Empty;

            // Remove leading/trailing whitespace
            emailAddress = emailAddress.Trim();

            // Find the last @ symbol
            int idx = emailAddress.LastIndexOf("@");

            // If no @ found, return the whole string
            if (idx < 0)
                return emailAddress;

            // Return the account part before @
            return emailAddress.Substring(0, idx).Trim();
        }
    }
}
