using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DND.Common.OpenIDConnect
{
    public class OpenIDConnectClient
    {
        private string idpServerEndpoint;
        private string userInfoEndpoint;
        private string tokenEndpoint;
        private string revocationEndpoint;
        private bool endPointsLoaded;

        public OpenIDConnectClient(string idpServerEndpoint)
        {
            this.idpServerEndpoint = idpServerEndpoint;
        }

        private async Task LoadEndpoints()
        {
            if (!endPointsLoaded)
            {
                var discoveryClient = new DiscoveryClient(idpServerEndpoint);
                var metaDataResponse = await discoveryClient.GetAsync();
                userInfoEndpoint = metaDataResponse.UserInfoEndpoint;
                tokenEndpoint = metaDataResponse.TokenEndpoint;
                revocationEndpoint = metaDataResponse.RevocationEndpoint;
            }
        }

        public async Task<IEnumerable<Claim>> GetUserInfo(HttpContext context)
        {
            return await GetUserInfo(await context.OIDCGetAccessTokenAsync());
        }

        //Get access token by await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
        public async Task<IEnumerable<Claim>> GetUserInfo(string accessToken)
        {
            await LoadEndpoints();
            var userInfoClient = new UserInfoClient(userInfoEndpoint);
            var response = await userInfoClient.GetAsync(accessToken);

            if (response.IsError)
            {
                throw new Exception("Problem accessing the UserInfo endpoint.", response.Exception);
            }

            return response.Claims;
        }

        public async Task<string> RenewTokens(HttpContext context)
        {
            await LoadEndpoints();
            var accessToken = await context.OIDCGetAccessTokenAsync();
            var tokenClient = new TokenClient(tokenEndpoint, "mvc", "secret");
            var currentRefreshToken = await context.OIDCGetRefreshTokenAsync();
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                var updatedTokens = new List<AuthenticationToken>();
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenResult.IdentityToken
                });

                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResult.AccessToken
                });

                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResult.RefreshToken
                });

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                updatedTokens.Add(new AuthenticationToken()
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                });

                var currentAuthenticationResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                currentAuthenticationResult.Properties.StoreTokens(updatedTokens);

                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, currentAuthenticationResult.Principal, currentAuthenticationResult.Properties);

                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception("Problem encountered while refreshing tokens.", tokenResult.Exception);
            }
        }

        public async Task LogOut(HttpContext context)
        {
            await RevokeAccess(context);
            await context.OIDCSignOutOfLocalAsync();
            await context.OIDCSignOutIDPAsync();
        }

        public async Task RevokeAccess(HttpContext context)
        {
            await LoadEndpoints();

            var revocationClient = new TokenRevocationClient(revocationEndpoint, "mvc", "secret");

            var accessToken = await context.OIDCGetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var revokeAccessTokenResponse = await revocationClient.RevokeAccessTokenAsync(accessToken);

                if (revokeAccessTokenResponse.IsError)
                {
                    throw new Exception("Problem encountered while revoking the access token.", revokeAccessTokenResponse.Exception);
                }
            }

            var refreshToken = await context.OIDCGetRefreshTokenAsync();

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var revokeRefreshTokenResponse = await revocationClient.RevokeRefreshTokenAsync(refreshToken);

                if (revokeRefreshTokenResponse.IsError)
                {
                    throw new Exception("Problem encountered while revoking the refresh token.", revokeRefreshTokenResponse.Exception);
                }
            }
        }
    }

    public static class OpenIDConnectExtensions
    {
        public static IEnumerable<Claim> OIDCClaims(this HttpContext context)
        {
            return context.User.Claims;
        }

        public static async Task OIDCSignOutOfLocalAsync(this HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public static async Task OIDCSignOutIDPAsync(this HttpContext context)
        {
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public static async Task<string> OIDCGetAccessTokenAsync(this HttpContext context)
        {
            return await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        }

        public static async Task<string> OIDCGetRefreshTokenAsync(this HttpContext context)
        {
            return await context.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
        }

        public static async Task<Nullable<DateTime>> OIDCGetTokenExpiryAsync(this HttpContext context)
        {
            var value = await context.GetTokenAsync("expires_at");
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(value).ToUniversalTime();
            }
        }
    }
}
