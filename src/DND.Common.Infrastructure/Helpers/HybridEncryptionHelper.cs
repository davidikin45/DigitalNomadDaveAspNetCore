using DND.Common.Infrastructure.Cryptography;
using System.Security.Cryptography;

namespace DND.Common.Infrastructure.Helpers
{
    public class HybridEncryption
    {

        public EncryptedPacket EncryptData(byte[] original, AsymmetricEncryptionHelper.RsaWithRsaParameterKey rsaParams, AsymmetricEncryptionHelper.RsaWithRsaParameterKey digitalSignature)
        {
            var sessionKey = SymmetricEncryptionHelper.AesEncryption.GenerateRandomNumber(32);
            var encryptedPacket = new EncryptedPacket { Iv = SymmetricEncryptionHelper.AesEncryption.GenerateRandomNumber(16) };

            // Encrypt data with AES and AES Key with RSA
            encryptedPacket.EncryptedData = SymmetricEncryptionHelper.AesEncryption.Encrypt(original, sessionKey, encryptedPacket.Iv);
            encryptedPacket.EncryptedSessionKey = rsaParams.EncryptData(sessionKey);

            using (var hmac = new HMACSHA256(sessionKey))
            {
                encryptedPacket.Hmac = hmac.ComputeHash(encryptedPacket.EncryptedData);
            }

            encryptedPacket.Signature = digitalSignature.CreateDigitalSignature(encryptedPacket.Hmac);

            return encryptedPacket;
        }

        public byte[] DecryptData(EncryptedPacket encryptedPacket, AsymmetricEncryptionHelper.RsaWithRsaParameterKey rsaParams, AsymmetricEncryptionHelper.RsaWithRsaParameterKey digitalSignature)
        {
            // Decrypt AES Key with RSA and then decrypt data with AES.
            var decryptedSessionKey = rsaParams.DecryptData(encryptedPacket.EncryptedSessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(encryptedPacket.EncryptedData);

                if (!Compare(encryptedPacket.Hmac, hmacToCheck))
                {
                    throw new CryptographicException("HMAC for decryption does not match encrypted packet.");
                }

                if (!digitalSignature.VerifySignature(encryptedPacket.Hmac,
                                                      encryptedPacket.Signature))
                {
                    throw new CryptographicException(
                        "Digital Signature can not be verified.");
                }
            }

            var decryptedData = SymmetricEncryptionHelper.AesEncryption.Decrypt(encryptedPacket.EncryptedData, decryptedSessionKey, encryptedPacket.Iv);

            return decryptedData;
        }

        private static bool Compare(byte[] array1, byte[] array2)
        {
            var result = array1.Length == array2.Length;

            for (var i = 0; i < array1.Length && i < array2.Length; ++i)
            {
                result &= array1[i] == array2[i];
            }

            return result;
        }

    }
}
