using Bitcoin.BitcoinUtilities;
using Bitcoin.KeyCore;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

namespace DND.Common.Helpers
{
    //https://www.bitaddress.org
    //https://en.bitcoin.it/wiki/Technical_background_of_version_1_Bitcoin_addresses Bitcoin address
    //https://en.bitcoin.it/wiki/Wallet_import_format WIF
    public static class BlockChain
    {
        #region Public Key 
        public static string GetPublicKeyUncompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privKey = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubicKeyWIF = privKey.PublicKey.ToString();
            return pubicKeyWIF;
        }

        public static string GetPublicKeyUncompressedHexFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privKey = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubKey = privKey.PublicKey;
            return ByteToHex(pubKey.PublicKeyBytes);
        }

        public static string GetPublicKeyCompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyCompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privKey = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubicKeyWIF = privKey.PublicKey.ToString();
            return pubicKeyWIF;
        }

        public static string GetPublicKeyCompressedHexFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyCompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privKey = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubKey = privKey.PublicKey;
            return ByteToHex(pubKey.PublicKeyBytes);
        }
        #endregion

        #region Private Key
        public static string GetPrivateKeyBase64FromPrivateKeyHex(string privateKeyHex)
        {
            return ByteToBase64(HexToByte(privateKeyHex));
        }

        public static string GetPrivateKeyCompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            byte[] PreHashQ = AppendCompressed(AppendBitcoinNetwork(HexToByte(privateKeyHex), 128), 1);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }

        public static string GetPrivateKeyUncompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(HexToByte(privateKeyHex), 128);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }

        public static bool PrivateKeyWIFIsValid(string privateKeyWIF)
        {
            try
            {
                var privKey = new PrivateKey(Globals.ProdDumpKeyVersion, privateKeyWIF);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Address
        public static (string compressedAddress, string uncompressedAddress, string publicKeyCompressedHex, string publicKeyUncompressedHex, string privateKeyHex, string privateKeyCompressedWIF, string privateKeyUncompressedWIF, string privateKeyBase64) CreateNewAddress()
        {
            var privKey = PrivateKey.CreatePrivateKey(Globals.ProdDumpKeyVersion, true);
            var privateKeyCompressedWIF = privKey.ToString();
            var privateKeyHex = ByteToHex(privKey.PrivateKeyBytes);
            var privateKeyUncompressedWIF = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privateKeyBase64 = GetPrivateKeyBase64FromPrivateKeyHex(privateKeyHex);

            var publicKeyWIF = privKey.PublicKey.ToString();
            var publicKeyCompressedHex = ByteToHex(privKey.PublicKey.PublicKeyBytes);
            var publicKeyUncompressedHex = GetPublicKeyUncompressedHexFromPrivateKeyHex(privateKeyHex);

            var compressedAddress = GetAddressFromPublicKeyHex(publicKeyCompressedHex);
            var uncompressedAddress = GetAddressFromPublicKeyHex(publicKeyUncompressedHex);

            return (compressedAddress: compressedAddress, uncompressedAddress: uncompressedAddress, publicKeyCompressedHex: publicKeyCompressedHex, publicKeyUncompressedHex: publicKeyUncompressedHex, privateKeyHex: privateKeyHex, privateKeyCompressedWIF: privateKeyCompressedWIF, privateKeyUncompressedWIF: privateKeyUncompressedWIF, privateKeyBase64: privateKeyBase64);
        }

        public static string GetAddressFromPrivateKeyHex(string privateKeyHex)
        {
          var publicKeyHex = GetPublicKeyCompressedHexFromPrivateKeyHex(privateKeyHex);
            return GetAddressFromPublicKeyHex(publicKeyHex);
        }

        public static string GetAddressFromPublicKeyHex(string publicKeyHex)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(RipeMD160(Sha256(HexToByte(publicKeyHex))), 0);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }

        public static string GetTestAddressFromPublicKeyHex(string publicKeyHex)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(RipeMD160(Sha256(HexToByte(publicKeyHex))), 111);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }

        public static (string compressedAddress, string uncompressedAddress, string publicKeyCompressedHex, string publicKeyUncompressedHex, string privateKeyHex, string privateKeyCompressedWIF, string privateKeyUncompressedWIF, string privateKeyBase64) CreateNewTestAddress()
        {
            var privKey = PrivateKey.CreatePrivateKey(Globals.TestAddressVersion, true);
            var privateKeyCompressedWIF = privKey.ToString();
            var privateKeyHex = ByteToHex(privKey.PrivateKeyBytes);
            var privateKeyUncompressedWIF = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privateKeyBase64 = GetPrivateKeyBase64FromPrivateKeyHex(privateKeyHex);

            var pubKey = new PublicKey(privKey, Globals.TestAddressVersion);
            var publicKeyWIF = pubKey.ToString();
            var publicKeyCompressedHex = ByteToHex(pubKey.PublicKeyBytes);
            var publicKeyUncompressedHex = GetPublicKeyUncompressedHexFromPrivateKeyHex(privateKeyHex);

            var compressedAddress = GetTestAddressFromPublicKeyHex(publicKeyCompressedHex);
            var uncompressedAddress = GetTestAddressFromPublicKeyHex(publicKeyUncompressedHex);

            return (compressedAddress: compressedAddress, uncompressedAddress: uncompressedAddress, publicKeyCompressedHex: publicKeyCompressedHex, publicKeyUncompressedHex: publicKeyUncompressedHex, privateKeyHex: privateKeyHex, privateKeyCompressedWIF: privateKeyCompressedWIF, privateKeyUncompressedWIF: privateKeyUncompressedWIF, privateKeyBase64: privateKeyBase64);
        }

        public static string GetTestAddress(string publicKeyHex)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(RipeMD160(Sha256(HexToByte(publicKeyHex))), 111);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }
        #endregion

        #region Helper Methods
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

        public static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(array);
        }

        public static byte[] RipeMD160(byte[] array)
        {
            RIPEMD160Managed hashstring = new RIPEMD160Managed();
            return hashstring.ComputeHash(array);
        }

        public static byte[] AppendBitcoinNetwork(byte[] RipeHash, byte Network)
        {
            byte[] extended = new byte[RipeHash.Length + 1];
            extended[0] = (byte)Network;
            Array.Copy(RipeHash, 0, extended, 1, RipeHash.Length);
            return extended;
        }

        public static byte[] AppendCompressed(byte[] array, byte compress)
        {
            byte[] extended = new byte[array.Length + 1];
            extended[extended.Length-1] = (byte)compress;
            Array.Copy(array, 0, extended, 0, array.Length);
            return extended;
        }

        public static byte[] ConcatAddress(byte[] RipeHash, byte[] Checksum)
        {
            byte[] ret = new byte[RipeHash.Length + 4];
            Array.Copy(RipeHash, ret, RipeHash.Length);
            Array.Copy(Checksum, 0, ret, RipeHash.Length, 4);
            return ret;
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

        public static string ByteToHex(byte [] array)
        {
            return BitConverter.ToString(array).Replace("-", string.Empty);
        }

        public static string ByteToBase64(byte[] array)
        {
            return Convert.ToBase64String(array);
        }
        #endregion
    }
}
