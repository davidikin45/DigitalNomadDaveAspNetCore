using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DND.IDP
{
    public static class Config
    {
        // test users
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Frank",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "Main Road 1"),
                        new Claim("role", "Admin"),
                        new Claim("subscriptionlevel", "FreeUser"),
                        new Claim("country", "nl")
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Claire",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Claire"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "Big Street 2"),
                        new Claim("role", "PayingUser"),
                        new Claim("subscriptionlevel", "PayingUser"),
                        new Claim("country", "be")
                    }
                }
            };
        }

        // identity-related resources (scopes)
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource(
                    "roles",
                    "Your roles(s)",
                    new List<string>(){"role"} )
                    ,
                new IdentityResource(
                    "country",
                    "The country you're living in",
                    new List<string>() { "country" }),
                new IdentityResource(
                    "subscriptionlevel",
                    "Your subscription level",
                    new List<string>() { "subscriptionlevel" })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "My API", new List<string>() {"role" })
                {
                    ApiSecrets = {new Secret("apisecret".Sha256())} // Only required if AccessTokenType = AccessTokenType.Reference
                    ,Scopes =
                    {
                        new Scope()
                        {
                            Name = ApiScopes.Full,
                            DisplayName = "Full access to API"
                        },
                        new Scope
                        {
                            Name = ApiScopes.Create,
                            DisplayName = "Create only access to API"
                        },
                        new Scope
                        {
                            Name = ApiScopes.Read,
                            DisplayName = "Read only access to API"
                        },
                        new Scope
                        {
                            Name = ApiScopes.Update,
                            DisplayName = "Update only access to API"
                        },
                        new Scope
                        {
                            Name = ApiScopes.Delete,
                            DisplayName = "Delete only access to API"
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                 //Server Initiation to Server Api
                new Client
                {
                    ClientName = "API Client",
                    ClientId = "api_server_client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AccessTokenType = AccessTokenType.Jwt,
                    ClientSecrets = new[] { new Secret("secret".Sha256()) },
                    AllowedScopes = new List<string> {
                        ApiScopes.Full ,
                        ApiScopes.Create,
                        ApiScopes.Read,
                        ApiScopes.Update,
                        ApiScopes.Delete
                    }
                },
                //Client Initiation to Server Api
                new Client
                {
                    ClientName = "Spa",
                    ClientId = "spa",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        ApiScopes.Read
                    },
                    RedirectUris = {
                        "https://localhost:44372/SignInCallback.html",
                        "https://localhost:44332/redirect-silentrenew",
                    },
                    PostLogoutRedirectUris = {
                        "https://localhost:44372/SignOutCallback.html",
                        "https://localhost:44332/",
                    },
                    AllowedCorsOrigins = {
                        "https://localhost:44372",
                        "https://localhost:44332"
                    }, //CORS for IDP
                    RequireConsent = false,
                    AccessTokenLifetime = 1200
                },
                 //Calling APIs on behalf of user
                new Client
                {
                    ClientName = "MVC Client",
                    ClientId = "mvc_client",
                    AllowedGrantTypes = GrantTypes.Hybrid,//Hybrid for MVC Client
                    AccessTokenType = AccessTokenType.Reference, //More control over lifetime with Reference. Requires Api to have ApiSecret
                    RequireConsent = true,
                    //IdentityTokenLifetime = 
                    //AuthorizationCodeLifetime =
                    AccessTokenLifetime = 1200,
                    AllowOfflineAccess = true,
                    //AbsoluteRefreshTokenLifetime
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44372/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44372/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        ApiScopes.Read,
                        "country",
                        "subscriptionlevel"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
            };
        }
    }
}
