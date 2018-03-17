using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ForgotPassword;
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
            {
                EmailAddress = emailAddress,
                ReturnUrl = returnUrl
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
        public async Task<IActionResult> ForgotPassword(ForgotPasswordInputModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = BuildForgotPasswordViewModel(returnUrl);

                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return RedirectToRoute("ForgotPasswordConfirmation", new {returnUrl});
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.RouteUrl("ResetPassword", new {code, returnUrl}, Request.Scheme);

            var subject = "Reset Password";
            var message =
                $"Reset your password by clicking the following link: <a href=\"{callbackUrl}\">{callbackUrl}</a>";

            await _emailSender.SendEmailAsync(model.EmailAddress, subject, message);

            return RedirectToRoute("ForgotPasswordConfirmation", new {returnUrl});
        }
    }
}