using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Driver;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public class MongoClientStore : IClientStore
    {
        private readonly IMongoCollection<Collections.Clients.Client> _clientsCollection;

        public MongoClientStore(IMongoCollection<Collections.Clients.Client> clientsCollection)
        {
            _clientsCollection = clientsCollection;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            if (clientId == null)
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            return await _clientsCollection.Find(c => c.ClientId == clientId)
                .Project(c => new Client
                {
                    AbsoluteRefreshTokenLifetime = c.AbsoluteRefreshTokenLifetime,
                    AccessTokenLifetime = c.AccessTokenLifetime,
                    AccessTokenType = c.AccessTokenType,
                    AllowAccessTokensViaBrowser = c.AllowAccessTokensViaBrowser,
                    AllowedCorsOrigins = c.AllowedCorsOrigins,
                    AllowedGrantTypes = c.AllowedGrantTypes,
                    AllowedScopes = c.AllowedScopes,
                    AllowOfflineAccess = c.AllowOfflineAccess,
                    AllowPlainTextPkce = c.AllowPlainTextPkce,
                    AllowRememberConsent = c.AllowRememberConsent,
                    AlwaysIncludeUserClaimsInIdToken = c.AlwaysIncludeUserClaimsInIdToken,
                    AlwaysSendClientClaims = c.AlwaysSendClientClaims,
                    AuthorizationCodeLifetime = c.AuthorizationCodeLifetime,
                    BackChannelLogoutSessionRequired = c.BackChannelLogoutSessionRequired,
                    BackChannelLogoutUri = c.BackChannelLogoutUri,
                    Claims = c.Claims.Select(cc =>
                        new Claim
                        (
                            cc.Type,
                            cc.Value
                        ))
                        .ToList(),
                    ClientClaimsPrefix = c.ClientClaimsPrefix,
                    ClientId = c.ClientId,
                    ClientName = c.ClientName,
                    ClientSecrets = c.ClientSecrets.Select(cs =>
                        new Secret
                        (
                            cs.Value,
                            cs.Description,
                            cs.Expiration
                        )
                        {
                            Type = cs.Type
                        })
                        .ToList(),
                    ClientUri = c.ClientUri,
                    ConsentLifetime = c.ConsentLifetime,
                    Enabled = c.Enabled,
                    EnableLocalLogin = c.EnableLocalLogin,
                    FrontChannelLogoutSessionRequired = c.FrontChannelLogoutSessionRequired,
                    FrontChannelLogoutUri = c.FrontChannelLogoutUri,
                    IdentityProviderRestrictions = c.IdentityProviderRestrictions,
                    IdentityTokenLifetime = c.IdentityTokenLifetime,
                    IncludeJwtId = c.IncludeJwtId,
                    LogoUri = c.LogoUri,
                    PairWiseSubjectSalt =c.PairWiseSubjectSalt ,
                    PostLogoutRedirectUris = c.PostLogoutRedirectUris,
                    Properties = c.Properties.ToDictionary(p => p.Key, p => p.Value),
                    ProtocolType = c.ProtocolType,
                    RedirectUris = c.RedirectUris,
                    RefreshTokenExpiration = c.RefreshTokenExpiration,
                    RefreshTokenUsage = c.RefreshTokenUsage,
                    RequireClientSecret = c.RequireClientSecret,
                    RequireConsent = c.RequireConsent,
                    RequirePkce = c.RequirePkce,
                    SlidingRefreshTokenLifetime = c.SlidingRefreshTokenLifetime,
                    UpdateAccessTokenClaimsOnRefresh = c.UpdateAccessTokenClaimsOnRefresh
                })
                .FirstOrDefaultAsync();
        }
    }
}