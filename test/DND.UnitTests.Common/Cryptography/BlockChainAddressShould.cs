﻿using DND.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DND.UnitTests.Common.Cryptography
{
    public class BlockChainShould
    {
        [Fact]
        public void CreateNewAddressAsExpected()
        {
            var data = BlockChain.CreateNewAddress();
            Assert.Equal('1', data.compressedAddress.First());
        }

        [Fact]
        public void GetAddressFromCompressedPublicKeyHexAsExpected()
        {
            var address = BlockChain.GetAddressFromPublicKeyHex("0250863ad64a87ae8a2fe83c1af1a8403cb53f53e486d8511dad8a04887e5b2352");
            Assert.Equal("1PMycacnJaSqwwJqjawXBErnLsZ7RkXUAs", address);
        }

        [Fact]
        public void GetAddressFromCompressedPrivateKeyHexAsExpected()
        {
            var address = BlockChain.GetAddressFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("1PMycacnJaSqwwJqjawXBErnLsZ7RkXUAs", address);
        }

        [Fact]
        public void GetAddressFromUncompressedPublicKeyHexAsExpected()
        {
            var uncompressedAddress = BlockChain.GetAddressFromPublicKeyHex("0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6");
            Assert.Equal("16UwLL9Risc3QfPqBUvKofHmBQ7wMtjvM", uncompressedAddress);
        }

        [Fact]
        public void GetUncompressedWIFFromPrivateKeyHexAsExpected()
        {
            var wif = BlockChain.GetPrivateKeyUncompressedWIFFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("5J1F7GHadZG3sCCKHCwg8Jvys9xUbFsjLnGec4H125Ny1V9nR6V", wif);

            Assert.True(BlockChain.PrivateKeyWIFIsValid(wif));
        }

        [Fact]
        public void GetCompressedWIFFromPrivateKeyHexAsExpected()
        {
            var wif = BlockChain.GetPrivateKeyCompressedWIFFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("Kx45GeUBSMPReYQwgXiKhG9FzNXrnCeutJp4yjTd5kKxCitadm3C", wif);

            Assert.True(BlockChain.PrivateKeyWIFIsValid(wif));
        }

        [Fact]
        public void GetPublicKeyUncompressedWIFFromPrivateKeyHexAsExpected()
        {
            var wif = BlockChain.GetPublicKeyUncompressedWIFFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("13VWBsMbW9bjxPtcLeSRJAGXcw7BsxQnPVV5hw6HkGpAbzjCT6X2DmKx2WqgQH6dfqHftVkUojz7ca3YFKbDVnzmZGE9H7R", wif);
        }

        [Fact]
        public void GetPublicKeyCompressedWIFFromPrivateKeyHexAsExpected()
        {
            var wif = BlockChain.GetPublicKeyCompressedWIFFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("15VxErdtxa1pnqrMbqj2hcU1ZKD8o263yLcjiMX3efcHWoJBHBj", wif);
        }

        [Fact]
        public void GetPublicKeyUncompressedHexFromPrivateKeyHexAsExpected()
        {
            var publicKeyUncompressedHex = BlockChain.GetPublicKeyUncompressedHexFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6", publicKeyUncompressedHex);
        }

        [Fact]
        public void GetPublicKeyCompressedHexFromPrivateKeyHexAsExpected()
        {
            var publicKeyCompressedHex = BlockChain.GetPublicKeyCompressedHexFromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("0250863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B2352", publicKeyCompressedHex);
        }

        [Fact]
        public void GetPrivateKeyBase64FromPrivateKeyHexAsExpected()
        {
            var base64 = BlockChain.GetPrivateKeyBase64FromPrivateKeyHex("18e14a7b6a307f426a94f8114701e7c8e774e7f9a47e2c2035db29a206321725");
            Assert.Equal("GOFKe2owf0JqlPgRRwHnyOd05/mkfiwgNdspogYyFyU=", base64);
        }

    }
}
