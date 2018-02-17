namespace Stubbl.Identity.Services
{
    using IdentityModel;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class StubblProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public StubblProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var nameClaim = context.Subject.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (nameClaim == null)
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(nameClaim.Value);

            if (user == null)
            {
                return;
            }

            context.IssuedClaims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            context.IssuedClaims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.CompletedTask;
        }
    }
}
