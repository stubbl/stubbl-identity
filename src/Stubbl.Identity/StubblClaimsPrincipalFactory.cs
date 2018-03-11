namespace Stubbl.Identity
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    public class StubblClaimsPrincipalFactory : UserClaimsPrincipalFactory<StubblUser, StubblRole>
    {
        public StubblClaimsPrincipalFactory(UserManager<StubblUser> userManager,
            RoleManager<StubblRole> roleManager, IOptions<IdentityOptions> identityOptions)
            : base(userManager, roleManager, identityOptions)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(StubblUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);

            var claims = new List<Claim>();

            // Profile
            claims.AddIfValueNotNull(JwtClaimTypes.Name, string.Join(" ", new[] { user.FirstName, user.LastName }));
            claims.AddIfValueNotNull(JwtClaimTypes.FamilyName, user.LastName);
            claims.AddIfValueNotNull(JwtClaimTypes.GivenName, user.FirstName);
            //claims.AddIfValueNotNull(JwtClaimTypes.MiddleName, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.NickName, ...);
            claims.AddIfValueNotNull(JwtClaimTypes.PreferredUserName, user.Username);
            //claims.AddIfValueNotNull(JwtClaimTypes.Profile, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.Picture, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.WebSite, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.Gender, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.BirthDate, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.ZoneInfo, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.Locale, ...);
            //claims.AddIfValueNotNull(JwtClaimTypes.UpdatedAt, ...);

            // Email
            claims.AddIfValueNotNull(JwtClaimTypes.Email, user.EmailAddress);
            claims.AddIfValueNotNull(JwtClaimTypes.EmailVerified, user.EmailAddressConfirmed.ToString().ToLower());

            // Address
            //claims.AddIfValueNotNull(JwtClaimTypes.Address, ...);

            // Phone
            claims.AddIfValueNotNull(JwtClaimTypes.PhoneNumber, user.PhoneNumber);
            claims.AddIfValueNotNull(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString().ToLower());

            // OpenId
            claims.AddIfValueNotNull(JwtClaimTypes.Subject, user.Id.ToString());

            claimsIdentity.AddOrReplaceClaims(claims);

            return claimsIdentity;
        }
    }
}
