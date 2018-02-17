namespace Stubbl.Identity.Controllers
{
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Stubbl.Identity.Models.ExternalLoginCallback;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ExternalLoginCallbackController : Controller
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly ILogger<ExternalLoginCallbackController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalLoginCallbackController(ILogger<ExternalLoginCallbackController> logger,
            IIdentityServerInteractionService interactionService, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _interactionService = interactionService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public ExternalLoginCallbackViewModel BuildExternalLoginCallbackViewModel(ExternalLoginCallbackInputModel model, string loginProvider, string returnUrl)
        {
            return new ExternalLoginCallbackViewModel
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                LoginProvider = loginProvider,
                ReturnUrl = returnUrl
            };
        }

        [HttpGet("/external-login-callback", Name = "ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError)
        {
            if (remoteError != null)
            {
                _logger.LogWarning("Login provider returned with message {remoteError}", remoteError);

                return RedirectToRoute("Login");
            }

            var authenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                _logger.LogWarning("Error loading external login information during callback");

                return RedirectToRoute("Login");
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                if (_interactionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToRoute("ViewAccount");
            }

            if (signInResult.IsLockedOut)
            {
                // TODO LockedOut
                return RedirectToRoute("LockedOut");
            }

            var emailAddress = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var name = loginInfo.Principal.FindFirstValue(ClaimTypes.Name)?.Split(" ");

            var model = new ExternalLoginCallbackInputModel
            {
                FirstName = name.First(),
                LastName = name.Length > 1 ? name.Last() : null,
                EmailAddress = emailAddress
            };

            ModelState.SetModelValue(nameof(model.FirstName), model.FirstName, model.FirstName);
            ModelState.SetModelValue(nameof(model.LastName), model.LastName, model.LastName);
            ModelState.SetModelValue(nameof(model.EmailAddress), model.EmailAddress, model.EmailAddress);

            TryValidateModel(model);

            return await ExternalLoginCallback(model, returnUrl);
        }

        [HttpPost("/external-login-callback", Name = "ExternalLoginCallback")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginCallback(ExternalLoginCallbackInputModel model, string returnUrl)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                // TODO Custom exception.
                throw new ApplicationException("Error loading external login information during callback confirmation");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = BuildExternalLoginCallbackViewModel(model, loginInfo.LoginProvider, returnUrl);

                return View(viewModel);
            }

            if (await _userManager.FindByEmailAsync(model.EmailAddress) != null)
            {
                ModelState.AddModelError(nameof(model.EmailAddress), "This email address has already been registered");

                var viewModel = BuildExternalLoginCallbackViewModel(model, loginInfo.LoginProvider, returnUrl);

                return View(viewModel);
            }

            var user = new ApplicationUser
            {
                EmailAddress = model.EmailAddress,
                Username = model.EmailAddress
            };

            var identityResult = await _userManager.CreateAsync(user);

            if (!identityResult.Succeeded)
            {
                _logger.LogError("Error creating user. @{IdentityResult}", identityResult);

                ModelState.AddModelError("", "Error creating user");

                var viewModel = BuildExternalLoginCallbackViewModel(model, loginInfo.LoginProvider, returnUrl);

                return View(viewModel);
            }

            identityResult = await _userManager.AddLoginAsync(user, loginInfo);

            if (!identityResult.Succeeded)
            {
                _logger.LogError("Error adding login to user. @{IdentityResult}", identityResult);

                await _userManager.DeleteAsync(user);

                ModelState.AddModelError("", "Error adding login to user");

                var viewModel = BuildExternalLoginCallbackViewModel(model, loginInfo.LoginProvider, returnUrl);

                return View(viewModel);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            if (_interactionService.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("ViewAccount");
        }
    }
}
