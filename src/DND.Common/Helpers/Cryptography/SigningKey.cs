using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Helpers.Cryptography
{
    public static class SigningKey
    {
        public static RsaSecurityKey LoadPrivateRsaSigningKey(string privateKeyPath)
        {
            var rsaParameters = AsymmetricEncryptionHelper.RsaWithPEMKey.GetPrivateKeyRSAParameters(privateKeyPath);
            var key = new RsaSecurityKey(rsaParameters);
            //key.KeyId = "IDP";
            return key;
        }
        public static RsaSecurityKey LoadPublicRsaSigningKey(string publicKeyPath)
        {
            var rsaParameters = AsymmetricEncryptionHelper.RsaWithPEMKey.GetPublicKeyRSAParameters(publicKeyPath);
            var key = new RsaSecurityKey(rsaParameters);
            //key.KeyId = "IDP";
            return key;
        }
    }
}
