using System.Collections.Generic;
using System.Linq;

namespace System.Security.Claims
{
    public static class ClaimIdentityExtensions
    {
        public static void AddOrReplaceClaim(this ClaimsIdentity extended, Claim claim)
        {
            if (extended == null)
            {
                throw new ArgumentNullException(nameof(extended));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var existingClaim = extended.Claims.FirstOrDefault(x => x.Type == claim.Type);

            if (existingClaim != null)
            {
                extended.RemoveClaim(existingClaim);
            }

            extended.AddClaim(claim);
        }

        public static void AddOrReplaceClaims(this ClaimsIdentity extended, IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
            {
                extended.AddOrReplaceClaim(claim);
            }
        }
    }
}