using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.JwtTokens
{
    public static class JwtTokenHelper
    {
        //Assymetric
        public static dynamic CreateJwtTokenSigningWithRsaSecurityKey(string email, string userName, IEnumerable<string> roles, int minuteExpiry, RsaSecurityKey key, string issuer, string audience)
        {
            var claims = new List<Claim>()
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                        };

            //Add roles
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // SecurityAlgorithms.HmacSha256
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            return CreateJwtToken(minuteExpiry, issuer, audience, claims, creds);
        }

        //Symmetric
        public static dynamic CreateJwtTokenSigningWithKey(string email, string userName, IEnumerable<string> roles, int minuteExpiry, string hmacKey, string issuer, string audience)
        {
            var claims = new List<Claim>()
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                        };

            //Add roles
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(hmacKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return CreateJwtToken(minuteExpiry, issuer, audience, claims, creds);
        }

        private static dynamic CreateJwtToken(int minuteExpiry, string issuer, string audience, List<Claim> claims, SigningCredentials creds)
        {
            var token = new JwtSecurityToken(
                        issuer,
                        audience,
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(minuteExpiry),
                        signingCredentials: creds);

            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };

            return results;
        }
    }
}
