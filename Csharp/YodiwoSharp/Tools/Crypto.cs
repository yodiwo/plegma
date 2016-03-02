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
            string b64 = Convert.ToBase64String(buf);
            return b64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
