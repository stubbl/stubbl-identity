using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.Login;

namespace Stubbl.Identity.Controllers
{
    public class LoginController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public LoginController(IClientStore clientStore, IIdentityServerInteractionService interactionService,
            IAuthenticationSchemeProvider schemeProvider, SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _clientStore = clientStore;
            _interactionService = interactionService;
            _schemeProvider = schemeProvider;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<LoginViewModel> BuildLoginViewModelAsync(AuthorizationRequest authorizationContext,
            string returnUrl, string emailAddress = null)
        {
            var loginProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .ToList();

            var allowLocalLogin = true;

            if (authorizationContext?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(authorizationContext.ClientId);

                if (client != null)
                {
                    allowLocalLogin = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        loginProviders = loginProviders.Where(lp => client.IdentityProviderRestrictions.Contains(lp.Name))
                            .ToList();
                    }
                }
            }

            return new LoginViewModel
            (
                loginProviders.ToList(),
                allowLocalLogin
            )
            {
                EmailAddress = emailAddress ?? authorizationContext?.LoginHint,
                ReturnUrl = returnUrl
            };
        }

        public async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel inputModel)
        {
            var authorizationContext = await _interactionService.GetAuthorizationContextAsync(inputModel.ReturnUrl);

            var viewModel = await BuildLoginViewModelAsync(authorizationContext, inputModel.ReturnUrl);
            viewModel.EmailAddress = inputModel.EmailAddress;
            viewModel.RememberMe = inputModel.RememberMe;

            return viewModel;
        }

        [HttpGet]
        [Route("/login", Name = "Login")]
        public async Task<IActionResult> Login([FromQuery] string emailAddress, [FromQuery] string returnUrl)
        {
            if (User.IsAuthenticated())
            {
                return RedirectToRoute("Home");
            }

            ModelState.Clear();

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var authorizationContext = await _interactionService.GetAuthorizationContextAsync(returnUrl);

            if (authorizationContext?.IdP != null
                && (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any(p =>
                    string.Equals(p.Name, authorizationContext.IdP, StringComparison.InvariantCultureIgnoreCase)))
            {
                var redirectUrl = Url.RouteUrl("ExternalLoginCallback", new { returnUrl });
                var properties =
                    _signInManager.ConfigureExternalAuthenticationProperties(authorizationContext.IdP, redirectUrl);

                return Challenge(properties, authorizationContext.IdP);
            }

            var viewModel = await BuildLoginViewModelAsync(authorizationContext, returnUrl, emailAddress);

            return View(viewModel);
        }

        [HttpPost("/login", Name = "Login")]
        public async Task<IActionResult> Login([FromForm] LoginInputModel inputModel, [FromQuery] string returnUrl)
        {
            LoginViewModel viewModel;

            if (!ModelState.IsValid)
            {
                viewModel = await BuildLoginViewModelAsync(inputModel);

                return View(viewModel);
            }

            var signInResult =
                await _signInManager.PasswordSignInAsync(inputModel.EmailAddress, inputModel.Password, inputModel.RememberMe, true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return RedirectToRoute("LockedOut");
                }

                if (signInResult.IsNotAllowed)
                {
                    var user = await _userManager.FindByEmailAsync(inputModel.EmailAddress);

                    if (_signInManager.Options.SignIn.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                        return RedirectToRoute("EmailAddressConfirmationSent", new { userId = user.Id, returnUrl });
                    }

                    return View("Error");
                }

                if (signInResult.RequiresTwoFactor)
                {
                    // TODO LoginTwoFactor
                    return RedirectToRoute("LoginTwoFactor", new { returnUrl, rememberMe = inputModel.RememberMe });
                }

                ModelState.AddModelError("", "The email address and/or password is incorrect");

                viewModel = await BuildLoginViewModelAsync(inputModel);

                return View(viewModel);
            }

            if (!_interactionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoute("Home");
        }
    }
}