using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ResetPassword;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<StubblUser> _userManager;

        public ResetPasswordController(IEmailSender emailSender, UserManager<StubblUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        private ResetPasswordViewModel BuildResetPasswordViewModel(string code, string returnUrl)
        {
            return new ResetPasswordViewModel
            {
                Code = code,
                ReturnUrl = returnUrl
            };
        }

        [HttpGet("/reset-password", Name = "ResetPassword")]
        public IActionResult ResetPassword(string code, string returnUrl)
        {
            if (code == null)
            {
                // TODO Custom exception.
                throw new ApplicationException("A code must be supplied to reset your password.");
            }

            var viewModel = BuildResetPasswordViewModel(code, returnUrl);

            return View(viewModel);
        }

        [HttpPost("/reset-password", Name = "ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordInputModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = BuildResetPasswordViewModel(model.Code, returnUrl);

                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.EmailAddress), "Please check the email address");

                var viewModel = BuildResetPasswordViewModel(model.Code, returnUrl);

                return View(viewModel);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(model.EmailAddress), "Please check the email address");

                var viewModel = BuildResetPasswordViewModel(model.Code, returnUrl);

                return View(viewModel);
            }

            return RedirectToRoute("ResetPasswordConfirmation", new {user.EmailAddress, returnUrl});
        }
    }
}