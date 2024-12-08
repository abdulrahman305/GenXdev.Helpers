using System.Text;

namespace GenXdev.Security
{
    public static class MD5
    {
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
