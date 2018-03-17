using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Stubbl.Identity
{
    public class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("stubbl-api", "Stubbl API")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            yield return new Client
            {
                AllowOfflineAccess = true,
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },
                ClientId = "stubbl-identity-mvc-client",
                ClientName = "Stubbl Identity MVC Client",
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                PostLogoutRedirectUris = {"http://localhost:57255/signout-callback-oidc"},
                RedirectUris = {"http://localhost:57255/signin-oidc"},
                RequireConsent = true
            };

            yield return new Client
            {
                AllowOfflineAccess = true,
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "stubbl-api"
                },
                ClientId = "stubbl-app",
                ClientName = "Stubbl App",
                ClientSecrets = configuration.GetSection("StubblApp:ClientSecrets")
                    .AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => new Secret(x.Sha256()))
                    .ToList(),
                PostLogoutRedirectUris = configuration.GetSection("StubblApp:PostLogoutRedirectUris")
                    .AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .ToList(),
                RedirectUris = configuration.GetSection("StubblApp:RedirectUris")
                    .AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .ToList(),
                RequireConsent = false,
                UpdateAccessTokenClaimsOnRefresh = true
            };

            yield return new Client
            {
                AllowAccessTokensViaBrowser = true,
                AllowedCorsOrigins = configuration.GetSection("StubblApiSwagger:AllowedCorsOrigins")
                    .AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .ToList(),
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "stubbl-api"
                },
                ClientId = "stubbl-api-swagger",
                ClientName = "stubbl-api-swagger",
                ClientSecrets = configuration.GetSection("StubblApp:ClientSecrets")
                    .AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => new Secret(x.Sha256()))
                    .ToList(),
                RedirectUris = configuration.GetSection("StubblApiSwagger:RedirectUris")
                    .AsEnumerable()
                    .Select(kvp => kvp.Value)
                    .ToList(),
                RequireConsent = false,
                RequirePkce = false
            };
        }
    }
}