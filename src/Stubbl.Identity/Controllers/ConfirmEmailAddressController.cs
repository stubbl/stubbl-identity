﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.Controllers
{
    public class ConfirmEmailAddressController : Controller
    {
        private readonly UserManager<StubblUser> _userManager;

        public ConfirmEmailAddressController(UserManager<StubblUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("/confirm-email-address/{userId}", Name = "ConfirmEmailAddress")]
        public async Task<IActionResult> ConfirmEmailAddress([FromRoute] string userId, [FromQuery] string token, [FromQuery] string returnUrl)
        {
            if (userId == null || token == null)
            {
                return RedirectToRoute("Home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (!result.Succeeded)
                {
                    return View("Error");
                }
            }
            else if (user.NewEmailAddress != null)
            {
                user.EmailAddress = user.NewEmailAddress;
                user.NewEmailAddress = null;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return View("Error");
                }
            }

            return RedirectToRoute("ConfirmEmailAddressConfirmation", new { user.EmailAddress, returnUrl });
        }
    }
}