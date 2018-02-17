using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Stubbl.Identity
{
    public class IdentityServerConfig
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource( "stubbl-api", "Stubbl API" ),
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
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
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
                EnableLocalLogin = false,
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

            // StubblApiSwagger
        }
    }
}
