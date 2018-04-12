using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ForgotPassword;
using Stubbl.Identity.Notifications.Email;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<StubblUser> _userManager;

        public ForgotPasswordController(IEmailSender emailSender, UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        private ForgotPasswordViewModel BuildForgotPasswordViewModel(string returnUrl, string emailAddress = null)
        {
            return new ForgotPasswordViewModel
            (
                returnUrl
            )
            {
                EmailAddress = emailAddress
            };
        }

        [HttpGet("/forgot-password", Name = "ForgotPassword")]
        public IActionResult ForgotPassword(string returnUrl, string emailAddress)
        {
            ModelState.Clear();

            var viewModel = BuildForgotPasswordViewModel(returnUrl, emailAddress);

            return View(viewModel);
        }

        [HttpPost("/forgot-password", Name = "ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordInputModel inputModel,
            [FromQuery] string returnUrl, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = BuildForgotPasswordViewModel(returnUrl);

                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(inputModel.EmailAddress);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToRoute("ForgotPasswordConfirmation", new {returnUrl});
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.RouteUrl("ResetPassword", new {code, returnUrl}, Request.Scheme);
            var email = new ResetPasswordEmail
            (
                user.NewEmailAddress,
                callbackUrl
            );

            await _emailSender.SendEmailAsync(email, cancellationToken);

            return RedirectToRoute("ForgotPasswordConfirmation", new {returnUrl});
        }
    }
}