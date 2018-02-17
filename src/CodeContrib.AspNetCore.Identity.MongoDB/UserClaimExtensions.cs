namespace CodeContrib.AspNetCore.Identity.MongoDB
{
    using Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    internal static class UserClaimExtensions
    {
        internal static IReadOnlyCollection<IdentityUserClaim> ToUserClaims(this IEnumerable<Claim> claims)
        {
            return claims.Select(c => new IdentityUserClaim
                {
                    Type = c.Type,
                    Value = c.Value
                })
                .ToList();
        }
    }
}
