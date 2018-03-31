using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ResetPassword;

namespace Stubbl.Identity.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly UserManager<StubblUser> _userManager;

        public ResetPasswordController(UserManager<StubblUser> userManager)
        {
            _userManager = userManager;
        }

        private static ResetPasswordViewModel BuildResetPasswordViewModel(string code, string returnUrl)
        {
            return new ResetPasswordViewModel
            (
                returnUrl
            )
            {
                Code = code
            };
        }

        [HttpGet("/reset-password", Name = "ResetPassword")]
        public IActionResult ResetPassword([FromQuery] string code, [FromQuery] string returnUrl)
        {
            if (code == null)
            {
                return View("Error");
            }

            var viewModel = BuildResetPasswordViewModel(code, returnUrl);

            return View(viewModel);
        }

        [HttpPost("/reset-password", Name = "ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordInputModel inputModel, [FromQuery] string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = BuildResetPasswordViewModel(inputModel.Code, returnUrl);

                return View(viewModel);
            }

            var user = await _userManager.FindByEmailAsync(inputModel.EmailAddress);

            if (user == null)
            {
                ModelState.AddModelError(nameof(inputModel.EmailAddress), "Please check the email address");

                var viewModel = BuildResetPasswordViewModel(inputModel.Code, returnUrl);

                return View(viewModel);
            }

            var result = await _userManager.ResetPasswordAsync(user, inputModel.Code, inputModel.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(inputModel.EmailAddress), "Please check the email address");

                var viewModel = BuildResetPasswordViewModel(inputModel.Code, returnUrl);

                return View(viewModel);
            }

            return RedirectToRoute("ResetPasswordConfirmation", new {user.EmailAddress, returnUrl});
        }
    }
}