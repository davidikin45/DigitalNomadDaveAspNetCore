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
    //https://www.scottbrady91.com/C-Sharp/JWT-Signing-using-ECDSA-in-dotnet-Core
    //https://pascalpares.gitbooks.io/implementation-of-the-bitcoin-system/content/1-transaction-4-structure.html Bitcoin datastructure
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
            byte[] PreHashQ = AppendCompressed(AppendBitcoinNetwork(HexToByte(privateKeyHex), Globals.ProdDumpKeyVersion[0]), 1);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }

        public static string GetPrivateKeyUncompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(HexToByte(privateKeyHex), Globals.ProdDumpKeyVersion[0]);
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

        #region Signing
        public static string SignHashAsHexWithPrivateKeyWIF(byte[] hash, string privateKeyWIF)
        {
            var key = new PrivateKey(Globals.ProdDumpKeyVersion, privateKeyWIF);
            return key.SignHashAsHex(hash);
        }

        public static string SignHashasHexWithPrivateKeyHex(byte[] hash, string privateKeyHex)
        {
            var key = new PrivateKey(Globals.ProdDumpKeyVersion, privateKeyHex, true);
            return key.SignHashAsHex(hash);
        }

        public static string SignHashAsHexWithPrivateKey(byte[] hash, byte[] privateKey)
        {
            var key = new PrivateKey(Globals.ProdDumpKeyVersion, privateKey);
            return key.SignHashAsHex(hash);
        }

        public static bool VerifySignatureWithPublicKeyHex(byte[] hash, string signatureHex, string publicKeyHex)
        {
            var key = new PublicKey(publicKeyHex, Globals.ProdDumpKeyVersion);
            return key.VerifySignature(hash, signatureHex);
        }

        public static bool VerifySignatureWithPublicKey(byte[] hash, string signatureHex, byte[] publicKey)
        {
            var key = new PublicKey(publicKey, Globals.ProdDumpKeyVersion);
            return key.VerifySignature(hash, signatureHex);
        }
        #endregion

        #region Prod Address
        public static (string compressedAddress, string uncompressedAddress, string publicKeyCompressedHex, string publicKeyUncompressedHex, string privateKeyHex, string privateKeyCompressedWIF, string privateKeyUncompressedWIF, string privateKeyBase64, PrivateKey privateKey, PublicKey publickey) CreateNewAddress()
        {
            var privKey = PrivateKey.CreatePrivateKey(Globals.ProdDumpKeyVersion, true);
            var privateKeyCompressedWIF = privKey.ToString();
            var privateKeyHex = ByteToHex(privKey.PrivateKeyBytes);
            var privateKeyUncompressedWIF = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var privateKeyBase64 = GetPrivateKeyBase64FromPrivateKeyHex(privateKeyHex);

            var publickey = privKey.PublicKey;
            var publicKeyWIF = privKey.PublicKey.ToString();
            var publicKeyCompressedHex = ByteToHex(privKey.PublicKey.PublicKeyBytes);
            var publicKeyUncompressedHex = GetPublicKeyUncompressedHexFromPrivateKeyHex(privateKeyHex);

            var compressedAddress = GetAddressFromPublicKeyHex(publicKeyCompressedHex);
            var uncompressedAddress = GetAddressFromPublicKeyHex(publicKeyUncompressedHex);

            return (compressedAddress: compressedAddress, uncompressedAddress: uncompressedAddress, publicKeyCompressedHex: publicKeyCompressedHex, publicKeyUncompressedHex: publicKeyUncompressedHex, privateKeyHex: privateKeyHex, privateKeyCompressedWIF: privateKeyCompressedWIF, privateKeyUncompressedWIF: privateKeyUncompressedWIF, privateKeyBase64: privateKeyBase64, privateKey: privKey, publickey: publickey);
        }

        public static string GetAddressFromPrivateKeyHex(string privateKeyHex)
        {
          var publicKeyHex = GetPublicKeyCompressedHexFromPrivateKeyHex(privateKeyHex);
            return GetAddressFromPublicKeyHex(publicKeyHex);
        }

        public static string GetAddressFromPublicKeyHex(string publicKeyHex)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(RipeMD160(Sha256(HexToByte(publicKeyHex))), Globals.ProdAddressVersion[0]);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }
        #endregion

        #region Helper Methods

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
            return Base58.Encode(array);
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
