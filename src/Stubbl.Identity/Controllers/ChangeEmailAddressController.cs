using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ChangeEmailAddress;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class ChangeEmailAddressController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public ChangeEmailAddressController(IEmailSender emailSender,
            SignInManager<StubblUser> signInManager, UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/change-email-address", Name = "ChangeEmailAddress")]
        public async Task<IActionResult> ChangeEmailAddress()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToRoute("EmailAddressConfirmationSent", new { userId = user.Id });
            }

            return View();
        }

        [HttpPost("/change-email-address", Name = "ChangeEmailAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmailAddress([FromForm] ChangeEmailAddressInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToRoute("EmailAddressConfirmationSent", new { userId = user.Id });
            }

            var emailAddress = Regex.Replace(inputModel.EmailAddress, @"\+[^@]+", "");

            if (string.Equals(emailAddress, user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToRoute("Home");
            }

            if (await _userManager.FindByEmailAsync(emailAddress) != null)
            {
                ModelState.AddModelError(nameof(inputModel.EmailAddress), "This email address has already been registered");

                return View(inputModel);
            }

            user.NewEmailAddress = emailAddress;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.RouteUrl("ConfirmEmailAddress", new { userId = user.Id, token },
                Request.Scheme);

            const string subject = "Stubbl: Please confirm your email address";
            var message =
                $"Please confirm your email address by clicking the following link: <a href=\"{callbackUrl}\">{callbackUrl}</a>.";

            await _emailSender.SendEmailAsync(user.NewEmailAddress, subject, message);

            if (_signInManager.Options.SignIn.RequireConfirmedEmail)
            {
                return RedirectToRoute("EmailAddressConfirmationSent", new { userId = user.Id });
            }

            return RedirectToRoute("Home");
        }
    }
}
