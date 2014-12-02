using System.Text;
using xBrainLab.Security.Cryptography;

namespace GrooveSharkClient.Helpers
{
    public static class HmacMd5
    {

        public static string Hash(string key, string content)
        {
            HMACMD5 hmac = new HMACMD5(key);
            var byteArray = hmac.ComputeHash(content);
            return ByteArrayToString(byteArray);
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
