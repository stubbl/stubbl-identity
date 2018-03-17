using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ResetPasswordConfirmation;

namespace Stubbl.Identity.Controllers
{
    public class ResetPasswordConfirmationController : Controller
    {
        [HttpGet("/reset-password-confirmation", Name = "ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation(string emailAddress, string returnUrl)
        {
            var viewModel = new ResetPasswordConfirmationViewModel
            {
                EmailAddress = emailAddress,
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }
    }
}