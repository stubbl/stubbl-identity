using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ForgotPasswordConfirmation;

namespace Stubbl.Identity.Controllers
{
    public class ForgotPasswordConfirmationController : Controller
    {
        [HttpGet("/forgot-password-confirmation", Name = "ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation([FromQuery] string returnUrl)
        {
            var viewModel = new ForgotPasswordConfirmationViewModel
            (
                returnUrl
            );

            return View(viewModel);
        }
    }
}