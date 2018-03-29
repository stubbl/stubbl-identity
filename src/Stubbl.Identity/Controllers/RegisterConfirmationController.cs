using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.RegisterConfirmation;

namespace Stubbl.Identity.Controllers
{
    public class RegisterConfirmationController : Controller
    {
        private readonly UserManager<StubblUser> _userManager;

        public RegisterConfirmationController(UserManager<StubblUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("/register-confirmation/{userId}", Name = "RegisterConfirmation")]
        public async Task<IActionResult> RegisterConfirmation([FromRoute] string userId, [FromQuery] bool confirmationSent, [FromQuery] string returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToRoute("Home");
            }

            var viewModel = new RegisterConfirmationViewModel
            (
                user.EmailAddress,
                userId,
                !confirmationSent,
                returnUrl
            );

            return View(viewModel);
        }
    }
}