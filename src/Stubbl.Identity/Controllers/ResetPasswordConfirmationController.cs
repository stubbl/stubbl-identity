using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ResetPasswordConfirmation;

namespace Stubbl.Identity.Controllers
{
    public class ResetPasswordConfirmationController : Controller
    {
        [HttpGet("/reset-password-confirmation", Name = "ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation([FromQuery] string emailAddress, [FromQuery] string returnUrl)
        {
            var viewModel = new ResetPasswordConfirmationViewModel
            (
                emailAddress,
                returnUrl
            );

            return View(viewModel);
        }
    }
}