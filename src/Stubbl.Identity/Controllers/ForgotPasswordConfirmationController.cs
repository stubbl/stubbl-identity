namespace Stubbl.Identity.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Stubbl.Identity.Models.ForgotPasswordConfirmation;
    using Stubbl.Identity.Services.EmailSender;

    public class ForgotPasswordConfirmationController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordConfirmationController(IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpGet("/forgot-password-confirmation", Name = "ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation(string returnUrl)
        {
            var viewModel = new ForgotPasswordConfirmationViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }
    }
}
