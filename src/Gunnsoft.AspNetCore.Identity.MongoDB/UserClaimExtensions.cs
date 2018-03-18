using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    internal static class UserClaimExtensions
    {
        internal static IReadOnlyCollection<IdentityUserClaim> ToUserClaims(this IEnumerable<Claim> extended)
        {
            return extended.Select(c => new IdentityUserClaim
                {
                    Type = c.Type,
                    Value = c.Value
                })
                .ToList();
        }
    }
}