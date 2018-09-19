using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DND.Common.Infrastructure.Helpers
{
    public class HashData
	{
        public static byte[] ComputeHashSha1(byte[] toBeHashed)
        {
            using (var sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(toBeHashed);
            }
        }

        public static string ComputeHashSha256Base58String(string toBeHashed)
        {
            var message = Encoding.UTF8.GetBytes(toBeHashed);
            return Base58Encode(ComputeHashSha256(message));
        }

        public static string Base58Encode(byte[] array)
        {
            const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            string retString = string.Empty;
            BigInteger encodeSize = ALPHABET.Length;
            BigInteger arrayToInt = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                arrayToInt = arrayToInt * 256 + (long)array[i];
            }
            while (arrayToInt > 0)
            {
                int rem = (int)(arrayToInt % encodeSize).IntValue();
                arrayToInt /= encodeSize;
                retString = ALPHABET[rem] + retString;
            }
            for (int i = 0; i < array.Length && array[i] == 0; ++i)
                retString = ALPHABET[0] + retString;

            return retString;
        }

        public static string ComputeHashSha256Base64String(string toBeHashed)
        {
            var message = Encoding.UTF8.GetBytes(toBeHashed);
            return ComputeHashSha256Base64String(message);
        }

        public static string ComputeHashSha256Base64String(Byte[] bytes)
        {
            var checksum = ComputeHashSha256(bytes);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public static byte[] ComputeHashSha256(byte[] toBeHashed)
		{
			using (var sha256 = SHA256.Create())
			{
				return sha256.ComputeHash(toBeHashed);
			}
		}

        public static byte[] ComputeHashSha512(byte[] toBeHashed)
        {
            using (var sha512 = SHA512.Create())
            {
                return sha512.ComputeHash(toBeHashed);
            }
        }

        public static string ComputeHashMd5(Stream stream)
        {
            var checksum = new byte[0];
            using (var md5 = MD5.Create())
            {
                checksum = md5.ComputeHash(stream);
            }
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public static byte[] ComputeHashMd5(byte[] toBeHashed)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(toBeHashed);
            }
        }

        public static byte[] GenerateSalt()
        {
            const int saltLength = 32;

            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[saltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }

        public static byte[] HashPasswordWithSalt(byte[] toBeHashed, byte[] salt)
        {
            return ComputeHashSha256(Combine(toBeHashed, salt));
        }

        public static byte[] HashPasswordWithSaltRfc2898(byte[] toBeHashed, byte[] salt, int numberOfRounds)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(toBeHashed, salt, numberOfRounds))
            {
                return rfc2898.GetBytes(32);
            }
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            var ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }

    }
}
