using Bitcoin.BitcoinUtilities;
using Bitcoin.KeyCore;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DND.Common.Helpers
{
    public class DigitalSignature
    {
        private RSAParameters _publicKey;
        private RSAParameters _privateKey;

        public void AssignNewKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                _publicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);
            }
        }

        public static (string publicKey, string privateKey) GetNewECDSAHexKeys()
        {
            CngKeyCreationParameters keyCreateParms = new CngKeyCreationParameters();
            keyCreateParms.KeyUsage = CngKeyUsages.Signing;
            keyCreateParms.ExportPolicy = CngExportPolicies.AllowPlaintextExport;

            using (CngKey DSKey = CngKey.Create(CngAlgorithm.ECDsaP256, null, keyCreateParms))
            {
                var privateKeyByte = DSKey.Export(CngKeyBlobFormat.EccPrivateBlob);
                var privateKey = BitConverter.ToString(privateKeyByte).Replace("-", string.Empty);

                var publicKeyByte = DSKey.Export(CngKeyBlobFormat.EccPublicBlob);
                var publicKey = BitConverter.ToString(publicKeyByte).Replace("-", string.Empty);

                return (publicKey: publicKey, privateKey: privateKey);
            }
        }

        public static byte[] SignHashECDSA(string hexPrivateKey, byte[] hashOfDataToSign)
        {
            CngKey key = CngKey.Import(HexToByte(hexPrivateKey),CngKeyBlobFormat.EccPrivateBlob);
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                return dsa.SignHash(hashOfDataToSign);
            }
        }

        public static byte[] SignDataECDSA(string hexPrivateKey, byte[] dataToSign)
        {
            CngKey key = CngKey.Import(HexToByte(hexPrivateKey), CngKeyBlobFormat.EccPrivateBlob);
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                dsa.HashAlgorithm = CngAlgorithm.Sha256;
                return dsa.SignData(dataToSign);
            }
        }

        public static bool VerfiySignatureFromSha256HashOfDataECDSA(string hexPublicKey, byte[] hashOfDataToSign,byte[] signature)
        {
            CngKey key = CngKey.Import(HexToByte(hexPublicKey), CngKeyBlobFormat.EccPublicBlob);
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                return dsa.VerifyHash(hashOfDataToSign, signature);
            }
        }

        public static bool VerfiySignatureFromDataECDSA(string hexPublicKey, byte[] data, byte[] signature)
        {
            CngKey key = CngKey.Import(HexToByte(hexPublicKey), CngKeyBlobFormat.EccPublicBlob);
            using (ECDsaCng dsa = new ECDsaCng(key))
            {
                dsa.HashAlgorithm = CngAlgorithm.Sha256;
                return dsa.VerifyData(data, signature);
            }
        }

        public static byte[] HexToByte(string HexString)
        {
            if (HexString.Length % 2 != 0)
                throw new Exception("Invalid HEX");
            byte[] retArray = new byte[HexString.Length / 2];
            for (int i = 0; i < retArray.Length; ++i)
            {
                retArray[i] = byte.Parse(HexString.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return retArray;
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

        public byte[] SignData(byte[] hashOfDataToSign)
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

        public bool VerifySignature(byte[] hashOfDataToSign, byte[] signature)
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
