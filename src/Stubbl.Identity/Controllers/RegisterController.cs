namespace Stubbl.Identity.Controllers
{
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Stubbl.Identity.Models.Register;
    using Stubbl.Identity.Services.EmailSender;
    using System.Threading.Tasks;

    public class RegisterController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterController(IEmailSender emailSender, IIdentityServerInteractionService interactionService,
            SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _interactionService = interactionService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public RegisterViewModel BuildRegisterViewModel(string returnUrl, string emailAddress = null, RegisterInputModel inputModel = null)
        {
            return new RegisterViewModel
            {
                EmailAddress = emailAddress ?? inputModel?.EmailAddress,
                Password = inputModel?.Password,
                ReturnUrl = returnUrl,
            };
        }

        [HttpGet("/register", Name = "Register")]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl, string emailAddress)
        {
            ModelState.Clear();

            var viewModel = BuildRegisterViewModel(returnUrl, emailAddress);

            return View(viewModel);
        }

        [HttpPost("/register", Name = "Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInputModel model, string returnUrl)
        {
            RegisterViewModel viewModel;

            if (!ModelState.IsValid)
            {
                viewModel = BuildRegisterViewModel(returnUrl);

                return View(viewModel);
            }

            if (await _userManager.FindByEmailAsync(model.EmailAddress) != null)
            {
                ModelState.AddModelError(nameof(model.EmailAddress), $"This email address has already been registered");

                viewModel = BuildRegisterViewModel(returnUrl);

                return View(viewModel);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Username = model.EmailAddress
            };

            var identityResult = await _userManager.CreateAsync(user, model.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                viewModel = BuildRegisterViewModel(returnUrl);

                return View(viewModel);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.RouteUrl("ConfirmEmailAddress", new { userId = user.Id, token, returnUrl }, Request.Scheme);

            var subject = "Stubbl: Please confirm your email address";
            var message = $"Please confirm your email address by clicking the following link: <a href=\"{callbackUrl}\">{callbackUrl}</a>.";

            await _emailSender.SendEmailAsync(user.EmailAddress, subject, message);

            if (_signInManager.Options.SignIn.RequireConfirmedEmail)
            {
                return RedirectToRoute("RegisterConfirmation", new { userId = user.Id, returnUrl });
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            if (_interactionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("ViewAccount");
        }
    }
}
