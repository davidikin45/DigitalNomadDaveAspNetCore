using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Helpers
{
    //HMAC = Integrity + Authentication
    //Digital Signature = Authentication + Non-repudiation
    public static class CryptographyHelper
    {
        public static string GetHash(Byte[] bytes)
        {
            var checksum = new byte[0];
            checksum = MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public static string GetHash(Stream stream)
        {
            var checksum = new byte[0];
            checksum = MD5.Create().ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
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

        public static Byte[] ComputeHashSha256(Byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(bytes);
            }
        }

        public static string ComputeHashSha256Base64StringWithHMACKey(string toBeHashed, string key)
        {
            var keyToUse = Encoding.UTF8.GetBytes(key);
            var message = Encoding.UTF8.GetBytes(toBeHashed);

            using (var hmac = new HMACSHA256(keyToUse))
            {
                return Convert.ToBase64String((hmac.ComputeHash(message)));
            }
        }

        private static RSAParameters _publicKey;
        private static RSAParameters _privateKey;

        public static void AssignNewEncryptionKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                _publicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);

            }
        }

        public static byte[] CreateDigitalSignature(byte[] hashOfDataToSign)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                rsa.ImportParameters(_privateKey);

                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("SHA256");

                return rsaFormatter.CreateSignature(hashOfDataToSign);
            }
        }

        public static bool VerifySignature(byte[] hashOfDataToSign, byte[] signature)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportParameters(_publicKey);

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");

                return rsaDeformatter.VerifySignature(hashOfDataToSign, signature);
            }
        }

    }
}
