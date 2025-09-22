// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : HashHelpers.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.278.2025
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
