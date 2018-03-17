using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ConfirmEmailAddressConfirmation;

namespace Stubbl.Identity.Controllers
{
    public class ConfirmEmailAddressConfirmationController : Controller
    {
        [HttpGet("/confirm-email-address-confirmation", Name = "ConfirmEmailAddressConfirmation")]
        public IActionResult ConfirmEmailAddressConfirmation(string emailAddress, string returnUrl)
        {
            var viewModel = new ConfirmEmailAddressConfirmationViewModel
            {
                EmailAddress = emailAddress,
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }
    }
}