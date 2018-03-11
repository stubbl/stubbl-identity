namespace Stubbl.Identity.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class ConfirmEmailAddressController : Controller
    {
        private readonly UserManager<StubblUser> _userManager;

        public ConfirmEmailAddressController(UserManager<StubblUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("/confirm-email-address", Name = "ConfirmEmailAddress")]
        public async Task<IActionResult> ConfirmEmailAddress(string userId, string token, string returnUrl)
        {
            if (userId == null || token == null)
            {
                return RedirectToRoute("Home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // TODO Custom exception.
                throw new ApplicationException($"Unable to load user with ID \"{userId}\".");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                var identityResult = await _userManager.ConfirmEmailAsync(user, token);

                if (!identityResult.Succeeded)
                {
                    // TODO Custom exception.
                    throw new ApplicationException($"Unable to confirm email address for user with ID \"{userId}\".");
                }
            }

            return RedirectToRoute("ConfirmEmailAddressConfirmation", new { user.EmailAddress, returnUrl });
        }
    }
}
