namespace Stubbl.Identity.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Stubbl.Identity.Models.RegisterConfirmation;
    using Stubbl.Identity.Services.EmailSender;
    using System.Threading.Tasks;

    public class RegisterConfirmationController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterConfirmationController(IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpGet("/register-confirmation/{userId}", Name = "RegisterConfirmation")]
        public async Task<IActionResult> RegisterConfirmation(string userId, string returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToRoute("ViewAccount");
            }

            var viewModel = new RegisterConfirmationViewModel
            {
                EmailAddress = user.EmailAddress,
                ReturnUrl = returnUrl,
                UserId = userId
            };

            return View(viewModel);
        }
    }
}
