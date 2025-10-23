// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : MD5.cs
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
using System.IO;
using System.Text;

namespace GenXdev.Helpers
{
    public static class MD5
    {
        /// <summary>
        /// Calculates the MD5 hash of the input string using ASCII encoding and returns it as a lowercase hexadecimal string.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>A string representing the MD5 hash in lowercase hexadecimal format.</returns>
        public static string CalculateMD5Hash(string input)
        {

            // step 1, calculate MD5 hash from input
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = System.Security.Cryptography.MD5.HashData(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {

                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        /// <summary>
        /// Calculates the MD5 hash of the input stream and returns it as a lowercase hexadecimal string.
        /// </summary>
        /// <param name="s">The input stream to be hashed.</param>
        /// <returns>A string representing the MD5 hash in lowercase hexadecimal format.</returns>
        public static string CalculateMD5Hash(Stream s)
        {

            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(s);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {

                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        public static string CalculateMD5Base64Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = System.Security.Cryptography.MD5.HashData(inputBytes);

            return Convert.ToBase64String(hash).ToLowerInvariant();
        }
    }
}
