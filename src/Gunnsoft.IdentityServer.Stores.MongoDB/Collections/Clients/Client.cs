using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Gunnsoft.IdentityServer.Stores.MongoDB.Collections.Clients
{
    public class Client
    {
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int AccessTokenLifetime { get; set; } = 3600;
        public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Jwt;
        public bool AllowAccessTokensViaBrowser { get; set; }
        public List<string> AllowedCorsOrigins { get; set; }
        public List<string> AllowedGrantTypes { get; set; }
        public List<string> AllowedScopes { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        public string BackChannelLogoutUri { get; set; }
        public List<ClientClaim> Claims { get; set; }
        public string ClientClaimsPrefix { get; set; } = "client_";
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public List<ClientSecret> ClientSecrets { get; set; }
        public string ClientUri { get; set; }
        public int? ConsentLifetime { get; set; } = null;
        public bool Enabled { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public string FrontChannelLogoutUri { get; set; }
        public int Id { get; set; }
        public List<string> IdentityProviderRestrictions { get; set; }
        public int IdentityTokenLifetime { get; set; } = 300;
        public bool IncludeJwtId { get; set; }
        public string LogoUri { get; set; }
        public string PairWiseSubjectSalt { get; set; }
        public List<string> PostLogoutRedirectUris { get; set; }
        public List<ClientProperty> Properties { get; set; }
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;
        public List<string> RedirectUris { get; set; }
        public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;
        public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;
        public bool RequireClientSecret { get; set; } = true;
        public bool RequireConsent { get; set; } = true;
        public bool RequirePkce { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
    }
}