﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var result = new List<IdentityResource>
            {
                new IdentityResources.OpenId(), // OpenId connect
                new IdentityResources.Profile()
            };

            return result;
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            var result = new List<ApiResource>
            {
                new ApiResource("api1", "API")
            };

            return result;
        }

        public static IEnumerable<Client> GetClients()
        {
            var result = new List<Client>
            {
                new Client
                {
                    ClientId = "hybridclient",
                    ClientName = "Hybrid client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AccessTokenLifetime = 120, // 2 minutes
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowAccessTokensViaBrowser = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
                    SlidingRefreshTokenLifetime = 1296000, // 15 days
                    RefreshTokenExpiration = TokenExpiration.Sliding, // once a new refresh token is requested its life time will be renewed by the amount of SlidingRefreshTokenLifetime but the refresh token will never exceed AbsoluteRefreshTokenLifetime
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:5000/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:5000/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new Client
                {
                    ClientId = "clientcredentialsclient",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AccessTokenLifetime = 120, // 2 minutes
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
                    SlidingRefreshTokenLifetime = 1296000, // 15 days
                    RefreshTokenExpiration = TokenExpiration.Sliding, // once a new refresh token is requested its life time will be renewed by the amount of SlidingRefreshTokenLifetime but the refresh token will never exceed AbsoluteRefreshTokenLifetime
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new Client
                {
                    ClientId = "resourceownerclient",
                    ClientName = "Resource Owner Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenLifetime = 120, // 2 minutes
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
                    SlidingRefreshTokenLifetime = 1296000, // 15 days
                    RefreshTokenExpiration = TokenExpiration.Sliding, // once a new refresh token is requested its life time will be renewed by the amount of SlidingRefreshTokenLifetime but the refresh token will never exceed AbsoluteRefreshTokenLifetime
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };

            return result;
        }

        public static List<TestUser> GetTestUsers()
        {
            var result = new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = Guid.NewGuid().ToString(),
                    Username = "Test",
                    Password = "test123",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Test")
                    }
                }
            };

            return result;
        }
    }
}
