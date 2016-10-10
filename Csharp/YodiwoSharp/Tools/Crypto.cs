using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class Crypto
    {
        public static string GenerateRandomBase64(int length)
        {
            int rawLength = (int)Math.Floor((double)(6 * length) / 8);

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buf = new byte[rawLength];
            rng.GetBytes(buf);
            string b64 = Convert.ToBase64String(buf)
                                .TrimEnd('=')
                                .Replace('+', '-')
                                .Replace('/', '_');
            return b64.Length <= length ? b64 : b64.Substring(0, length);
        }

        public static byte[] EncryptText_AES(string input, string key)
        {
            return EncryptBytes_AES(Encoding.UTF8.GetBytes(input), key);
        }

        public static byte[] EncryptBytes_AES(byte[] input, string key)
        {
            DebugEx.Assert(key.Length == 32, "key must be 32 bytes (256bits)");

            //setup provider key
            var aesAlg = new RijndaelManaged();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(key.Substring(0, 16));

            //create encryptor
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            // encrypt
            return encryptor.TransformFinalBlock(input, 0, input.Length);
        }

        public static string DecryptText_AES(byte[] input, string key)
        {
            return Encoding.UTF8.GetString(DecryptBytes_AES(input, key));
        }

        public static byte[] DecryptBytes_AES(byte[] input, string key)
        {
            DebugEx.Assert(key.Length == 32, "key must be 32 bytes (256bits)");

            //setup provider key
            var aesAlg = new RijndaelManaged();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(key.Substring(0, 16));

            // Create a decrytor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            // decrypt
            try { return decryptor.TransformFinalBlock(input, 0, input.Length); }
            catch { return null; }
        }
    }
}
