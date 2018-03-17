namespace CodeContrib.AspNetCore.Identity.MongoDB
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ClaimsExtensions
    {
        public static void AddOrReplace(this IList<IdentityUserClaim> extended, IdentityUserClaim claim)
        {
            if (extended == null)
            {
                return;
            }

            var existingClaim = extended.FirstOrDefault(x => x.Type == claim.Type);

            if (existingClaim != null)
            {
                extended.Remove(existingClaim);
            }

            extended.Add(claim);
        }
    }
}
