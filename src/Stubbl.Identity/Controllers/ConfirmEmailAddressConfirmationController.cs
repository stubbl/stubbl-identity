using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ConfirmEmailAddressConfirmation;

namespace Stubbl.Identity.Controllers
{
    public class ConfirmEmailAddressConfirmationController : Controller
    {
        [HttpGet("/confirm-email-address-confirmation", Name = "ConfirmEmailAddressConfirmation")]
        public IActionResult ConfirmEmailAddressConfirmation([FromQuery] string emailAddress, [FromQuery] string returnUrl)
        {
            var viewModel = new ConfirmEmailAddressConfirmationViewModel
            (
                emailAddress,
                returnUrl
            );

            return View(viewModel);
        }
    }
}