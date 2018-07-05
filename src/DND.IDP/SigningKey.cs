using DND.IDP.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.IDP
{
    public static class SigningKey
    {
        public static RsaSecurityKey LoadPrivateRsaSigningKey(string privateKeyPath)
        {
            var rsaParameters = AsymmetricEncryptionHelper.RsaWithPEMKey.GetPrivateKeyRSAParameters(privateKeyPath);
            return new RsaSecurityKey(rsaParameters);
        }

        public static RsaSecurityKey LoadPublicRsaSigningKey(string publicKeyPath)
        {
            var rsaParameters = AsymmetricEncryptionHelper.RsaWithPEMKey.GetPublicKeyRSAParameters(publicKeyPath);
            return new RsaSecurityKey(rsaParameters);
        }
    }
}
