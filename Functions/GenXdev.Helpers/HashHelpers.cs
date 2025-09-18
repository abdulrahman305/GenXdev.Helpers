// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : HashHelpers.cs
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



using Org.BouncyCastle.Math;
using System.Security.Cryptography;
using System.Text;

namespace GenXdev.Helpers
{
    public static class Hash
    {
        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string FormatBytesAsHexString(byte[] value)
        {
            StringBuilder hex = new StringBuilder(value.Length * 2);
            foreach (byte x in value)
            {
                hex.AppendFormat("{0:x2}", x);
            }
            return hex.ToString();
        }

        public static byte[] GetSha256BytesOfString(string value, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            return (SHA256.Create()).ComputeHash(encoding.GetBytes(value));
        }

        public static string GetSha256Base64StringOfString(string value)
        {
            return Convert.ToBase64String(GetSha256BytesOfString(value));
        }

        public static byte[] GetSha256Bytes(params byte[][] input)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var buffer in input)
                {
                    stream.Write(buffer, 0, buffer.Length);
                }

                stream.Position = 0;

                if (stream.Length == 0)
                    throw new InvalidOperationException("Don't calc hashes of empty data");

                var sha = SHA256.Create();

                return sha.ComputeHash(stream);
            }
        }

        public static string GetSha256HexStringOfString(string value, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            return FormatBytesAsHexString(GetSha256BytesOfString(value, encoding));
        }

        public static string GetSha256HexStringOfBytes(byte[] value)
        {
            return FormatBytesAsHexString(GetSha256Bytes(value));
        }

        public static string GetSha512HexStringOfString(string Phrase)
        {
            SHA512 HashTool = SHA512.Create();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(Phrase); //getBytes(string.Concat(Phrase, "test");
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            StringBuilder hex = new StringBuilder(EncryptedBytes.Length * 2);
            foreach (byte b in EncryptedBytes)
                hex.AppendFormat("{0:x2}", b);
            return "0" + Convert.ToString(hex);
        }

        public static string GetSha512Base64StringOfString(string Phrase)
        {
            SHA512 HashTool = SHA512.Create();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(Phrase); //getBytes(string.Concat(Phrase, "test");
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            StringBuilder hex = new StringBuilder(EncryptedBytes.Length * 2);
            foreach (byte b in EncryptedBytes)
                hex.AppendFormat("{0:x2}", b);
            var stringToParse = "0" + Convert.ToString(hex);
            var result = new BigInteger(stringToParse, 16);
            return Convert.ToBase64String(result.ToByteArray());
        }
    }
}
