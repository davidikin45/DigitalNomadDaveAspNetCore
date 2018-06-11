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
    public static class BlockChainAddress
    {
        #region Public Key 
        public static string GetPublicKeyUncompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var pk = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubicKeyWIF = pk.PublicKey.ToString();
            return pubicKeyWIF;
        }

        public static string GetPublicKeyUncompressedHexFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyUncompressedWIFFromPrivateKeyHex(privateKeyHex);
            var pk = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubKey = pk.PublicKey;
            return ByteToHex(pubKey.PublicKeyBytes);
        }

        public static string GetPublicKeyCompressedWIFFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyCompressedWIFFromPrivateKeyHex(privateKeyHex);
            var pk = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubicKeyWIF = pk.PublicKey.ToString();
            return pubicKeyWIF;
        }

        public static string GetPublicKeyCompressedHexFromPrivateKeyHex(string privateKeyHex)
        {
            string wif = GetPrivateKeyCompressedWIFFromPrivateKeyHex(privateKeyHex);
            var pk = new PrivateKey(Globals.ProdDumpKeyVersion, wif);
            var pubKey = pk.PublicKey;
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
        #endregion

        #region Address
        public static (string address, string publicKeyHex, string privateKeyHex) CreateNewAddress()
        {
            var keys = DigitalSignature.GetNewECDSAHexKeys();
            var address = GetAddressFromPublicKeyHex(keys.publicKey);
            return (address: address, publicKeyHex: keys.publicKey, privateKeyHex: keys.privateKey);
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

        public static (string address, string publicKeyHex, string privateKeyHex) CreateNewTestAddress()
        {
            var keys = DigitalSignature.GetNewECDSAHexKeys();
            var address = GetTestAddress(keys.publicKey);
            return (address: address, publicKeyHex: keys.publicKey, privateKeyHex: keys.privateKey);
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

        private const string _alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private static readonly Int32 _base = 58;
        public static byte[] Base58DecodeChecked(string input)
        {
            var tmp = Decode(input);
            if (tmp.Length < 4)
                throw new Exception("Input too short");
            var checksum = new byte[4];
            Array.Copy(tmp, tmp.Length - 4, checksum, 0, 4);
            var bytes = new byte[tmp.Length - 4];
            Array.Copy(tmp, 0, bytes, 0, tmp.Length - 4);
            tmp = Utilities.DoubleDigest(bytes);
            var hash = new byte[4];
            Array.Copy(tmp, 0, hash, 0, 4);
            if (!hash.SequenceEqual(checksum))
                throw new Exception("Checksum does not validate");
            return bytes;
        }

        public static byte[] Decode(string input)
        {
            var bytes = DecodeToBigInteger(input).getBytes();
            // We may have got one more byte than we wanted, if the high bit of the next-to-last byte was not zero. This
            // is because BigIntegers are represented with twos-compliment notation, thus if the high bit of the last
            // byte happens to be 1 another 8 zero bits will be added to ensure the number parses as positive. Detect
            // that case here and chop it off.
            var stripSignByte = bytes.Length > 1 && bytes[0] == 0 && bytes[1] >= 0x80;
            // Count the leading zeros, if any.
            var leadingZeros = 0;
            for (var i = 0; input[i] == _alphabet[0]; i++)
            {
                leadingZeros++;
            }
            // Now cut/pad correctly. Java 6 has a convenience for this, but Android can't use it.
            var tmp = new byte[bytes.Length - (stripSignByte ? 1 : 0) + leadingZeros];
            Array.Copy(bytes, stripSignByte ? 1 : 0, tmp, leadingZeros, tmp.Length - leadingZeros);
            return tmp;
        }

        public static BigInteger DecodeToBigInteger(string input)
        {
            var bi = new BigInteger(0);
            // Work backwards through the string.
            for (var i = input.Length - 1; i >= 0; i--)
            {
                var alphaIndex = _alphabet.IndexOf(input[i]);
                if (alphaIndex == -1)
                {
                    throw new Exception("Illegal character " + input[i] + " at " + i);
                }
                bi = bi + new BigInteger(alphaIndex)*(new BigInteger((long)Math.Pow(_base, (input.Length - 1 - i))));
            }
            return bi;
        }
        #endregion
    }
}
