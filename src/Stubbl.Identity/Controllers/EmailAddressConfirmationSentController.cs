using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.EmailAddressConfirmationSent;

namespace Stubbl.Identity.Controllers
{
    public class EmailAddressConfirmationSentController : Controller
    {
        private readonly UserManager<StubblUser> _userManager;

        public EmailAddressConfirmationSentController(UserManager<StubblUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("/email-address-confirmation-sent/{userId}", Name = "EmailAddressConfirmationSent")]
        public async Task<IActionResult> EmailAddressConfirmationSent([FromRoute] string userId, [FromQuery] bool resent, [FromQuery] string returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToRoute("Home");
            }

            var viewModel = new EmailAddressConfirmationSentViewModel
            (
                user.NewEmailAddress ?? user.EmailAddress,
                userId,
                !resent,
                returnUrl
            );

            return View(viewModel);
        }
    }
}