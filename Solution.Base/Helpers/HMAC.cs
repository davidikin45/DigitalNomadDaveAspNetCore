using System;
using System.Security.Cryptography;
using System.Text;

namespace Solution.Base.Helpers
{
	public class Hmac
	{
		private const int KeySize = 32;

        public static string GenerateKeyBase64()
        {
            return Convert.ToBase64String(GenerateKey());
        }

        public static byte[] GenerateKey()
		{
			using (var randomNumberGenerator = new RNGCryptoServiceProvider())
			{
				var randomNumber = new byte[KeySize];
				randomNumberGenerator.GetBytes(randomNumber);

				return randomNumber;
			}
		}

        public static string ComputeHashSha256Base64StringWithHMACKey(string toBeHashed, string key)
        {
            var keyToUse = Encoding.UTF8.GetBytes(key);
            var message = Encoding.UTF8.GetBytes(toBeHashed);

            return Convert.ToBase64String(ComputeHmacsha256(message, keyToUse));
        }

        public static byte[] ComputeHmacsha256(byte[] toBeHashed, byte[] key)
		{
			using (var hmac = new HMACSHA256(key))
			{
				return hmac.ComputeHash(toBeHashed);
			}
		}

        public static byte[] ComputeHmacsha1(byte[] toBeHashed, byte[] key)
        {
            using (var hmac = new HMACSHA1(key))
            {
                return hmac.ComputeHash(toBeHashed);
            }
        }

        public static byte[] ComputeHmacsha512(byte[] toBeHashed, byte[] key)
        {
            using (var hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(toBeHashed);
            }
        }

        public static byte[] ComputeHmacmd5(byte[] toBeHashed, byte[] key)
        {
            using (var hmac = new HMACMD5(key))
            {
                return hmac.ComputeHash(toBeHashed);
            }
        }
    }
}
