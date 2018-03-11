namespace Stubbl.Identity.Controllers
{
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Stubbl.Identity.Models.Login;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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
                .Select(x => x.Name)
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
                        loginProviders = loginProviders.Where(lp => client.IdentityProviderRestrictions.Contains(lp)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                EnableLocalLogin = allowLocalLogin,
                EmailAddress = emailAddress ?? authorizationContext?.LoginHint,
                LoginProviders = loginProviders.ToArray(),
                ReturnUrl = returnUrl
            };
        }

        public async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var authorizationContext = await _interactionService.GetAuthorizationContextAsync(model.ReturnUrl);

            var viewModel = await BuildLoginViewModelAsync(authorizationContext, model.ReturnUrl);
            viewModel.EmailAddress = model.EmailAddress;
            viewModel.RememberMe = model.RememberMe;

            return viewModel;
        }

        [HttpGet]
        [Route("/login", Name = "Login")]
        public async Task<IActionResult> Login(string returnUrl, string emailAddress)
        {
            ModelState.Clear();

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var authorizationContext = await _interactionService.GetAuthorizationContextAsync(returnUrl);

            if (authorizationContext?.IdP != null
                && (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any(p => string.Equals(p.Name, authorizationContext.IdP, StringComparison.InvariantCultureIgnoreCase)))
            {
                var redirectUrl = Url.RouteUrl("ExternalLoginCallback", new { returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(authorizationContext.IdP, redirectUrl);

                return Challenge(properties, authorizationContext.IdP);
            }

            var viewModel = await BuildLoginViewModelAsync(authorizationContext, returnUrl, emailAddress);

            return View(viewModel);
        }

        [HttpPost("/login", Name = "Login")]
        public async Task<IActionResult> Login(LoginInputModel model, string returnUrl)
        {
            LoginViewModel viewModel;

            if (!ModelState.IsValid)
            {
                viewModel = await BuildLoginViewModelAsync(model);

                return View(viewModel);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    // TODO LockedOut
                    return RedirectToRoute("LockedOut");
                }

                if (signInResult.IsNotAllowed)
                {
                    if (_signInManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        var user = await _userManager.FindByEmailAsync(model.EmailAddress);

                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            return RedirectToRoute("RegisterConfirmation", new { userId = user.Id, returnUrl });
                        }
                    }
                }

                if (signInResult.RequiresTwoFactor)
                {
                    // TODO LoginTwoFactor
                    return RedirectToRoute("LoginTwoFactor", new { returnUrl, rememberMe = model.RememberMe });
                }

                ModelState.AddModelError("", "The email address and/or password is incorrect");

                viewModel = await BuildLoginViewModelAsync(model);

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