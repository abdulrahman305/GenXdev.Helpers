// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : HashHelpers.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.304.2025
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



using Org.BouncyCastle.Math;
using System.Security.Cryptography;
using System.Text;

namespace GenXdev.Helpers
{
    public static class Hash
    {
        /// <summary>
        /// Converts a hexadecimal string representation to its corresponding byte array.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert.</param>
        /// <returns>Byte array representation of the hexadecimal string.</returns>
        public static byte[] HexStringToByteArray(string hex)
        {
            // convert hex string to byte array by processing pairs of characters
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Formats a byte array as a lowercase hexadecimal string for consistent representation.
        /// </summary>
        /// <param name="value">The byte array to format.</param>
        /// <returns>Hexadecimal string representation of the byte array.</returns>
        public static string FormatBytesAsHexString(byte[] value)
        {
            // build hex string with consistent lowercase formatting
            StringBuilder hex = new StringBuilder(value.Length * 2);
            foreach (byte x in value)
            {
                hex.AppendFormat("{0:x2}", x);
            }
            return hex.ToString();
        }

        /// <summary>
        /// Computes SHA256 hash bytes from a string input using specified encoding.
        /// </summary>
        /// <param name="value">The string to hash.</param>
        /// <param name="encoding">The text encoding to use (defaults to UTF8 without BOM).</param>
        /// <returns>SHA256 hash as byte array.</returns>
        public static byte[] GetSha256BytesOfString(string value, Encoding encoding = null)
        {
            // use UTF8 encoding if none specified
            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            // compute hash of encoded string bytes
            return (SHA256.Create()).ComputeHash(encoding.GetBytes(value));
        }

        /// <summary>
        /// Computes SHA256 hash of a string and returns it as a base64-encoded string.
        /// </summary>
        /// <param name="value">The string to hash.</param>
        /// <returns>SHA256 hash as base64 string.</returns>
        public static string GetSha256Base64StringOfString(string value)
        {
            // convert hash bytes to base64 for compact string representation
            return Convert.ToBase64String(GetSha256BytesOfString(value));
        }

        /// <summary>
        /// Computes SHA256 hash from multiple byte array inputs concatenated together.
        /// </summary>
        /// <param name="input">Variable number of byte arrays to hash.</param>
        /// <returns>SHA256 hash as byte array.</returns>
        public static byte[] GetSha256Bytes(params byte[][] input)
        {
            // concatenate all input arrays into a single stream for hashing
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var buffer in input)
                {
                    stream.Write(buffer, 0, buffer.Length);
                }

                stream.Position = 0;

                // prevent hashing empty data which would be meaningless
                if (stream.Length == 0)
                    throw new InvalidOperationException("Don't calc hashes of empty data");

                var sha = SHA256.Create();

                return sha.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Computes SHA256 hash of a string and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="value">The string to hash.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>SHA256 hash as hexadecimal string.</returns>
        public static string GetSha256HexStringOfString(string value, Encoding encoding = null)
        {
            // use UTF8 encoding if none specified
            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            // format hash bytes as hex string for human-readable output
            return FormatBytesAsHexString(GetSha256BytesOfString(value, encoding));
        }

        /// <summary>
        /// Computes SHA256 hash of byte array and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="value">The byte array to hash.</param>
        /// <returns>SHA256 hash as hexadecimal string.</returns>
        public static string GetSha256HexStringOfBytes(byte[] value)
        {
            // format hash bytes as hex string for consistent representation
            return FormatBytesAsHexString(GetSha256Bytes(value));
        }

        /// <summary>
        /// Computes SHA512 hash of a string and returns it as a hexadecimal string with leading zero.
        /// </summary>
        /// <param name="Phrase">The string to hash.</param>
        /// <returns>SHA512 hash as hexadecimal string with leading zero.</returns>
        public static string GetSha512HexStringOfString(string Phrase)
        {
            // create SHA512 hasher instance
            SHA512 HashTool = SHA512.Create();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(Phrase); //getBytes(string.Concat(Phrase, "test");
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();

            // build hex string with consistent formatting
            StringBuilder hex = new StringBuilder(EncryptedBytes.Length * 2);
            foreach (byte b in EncryptedBytes)
                hex.AppendFormat("{0:x2}", b);

            // return with leading zero for consistency with other implementations
            return "0" + Convert.ToString(hex);
        }

        /// <summary>
        /// Computes SHA512 hash of a string and returns it as a base64-encoded string.
        /// </summary>
        /// <param name="Phrase">The string to hash.</param>
        /// <returns>SHA512 hash as base64 string.</returns>
        public static string GetSha512Base64StringOfString(string Phrase)
        {
            // create SHA512 hasher instance
            SHA512 HashTool = SHA512.Create();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(Phrase); //getBytes(string.Concat(Phrase, "test");
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();

            // build hex string first, then convert to BigInteger for base64 encoding
            StringBuilder hex = new StringBuilder(EncryptedBytes.Length * 2);
            foreach (byte b in EncryptedBytes)
                hex.AppendFormat("{0:x2}", b);
            var stringToParse = "0" + Convert.ToString(hex);
            var result = new BigInteger(stringToParse, 16);

            // convert to base64 for compact string representation
            return Convert.ToBase64String(result.ToByteArray());
        }
    }
}
