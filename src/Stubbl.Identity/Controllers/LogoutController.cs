using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.Logout;

namespace Stubbl.Identity.Controllers
{
    public class LogoutController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly SignInManager<StubblUser> _signInManager;

        public LogoutController(IHttpContextAccessor httpContextAccessor,
            IIdentityServerInteractionService interactionService,
            SignInManager<StubblUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _interactionService = interactionService;
            _signInManager = signInManager;
        }

        [HttpGet("/logout", Name = "Logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var viewModel = await BuildLogoutViewModelAsync(logoutId);

            if (!viewModel.ShowLogoutPrompt)
            {
                return await Logout(viewModel);
            }

            return View(viewModel);
        }

        [HttpPost("/logout", Name = "Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            var viewModel = await BuildLoggedOutViewModelAsync(model.LogoutId);

            await _signInManager.SignOutAsync();

            if (viewModel.TriggerExternalSignout)
            {
                var url = Url.RouteUrl("Logout", new {logoutId = viewModel.LogoutId});

                return SignOut(new AuthenticationProperties {RedirectUri = url}, viewModel.LoginProvider);
            }

            return View("LoggedOut", viewModel);
        }

        public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            var logout = await _interactionService.GetLogoutContextAsync(logoutId);

            var viewModel = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountConfig.AutomaticRedirectAfterSignOut,
                ClientName = logout?.ClientName,
                LogoutId = logoutId,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            var user = _httpContextAccessor.HttpContext.User;

            if (user?.Identity.IsAuthenticated == true)
            {
                var identityProvider = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                if (identityProvider != null && identityProvider != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout =
                        await _httpContextAccessor.HttpContext.GetSchemeSupportsSignOutAsync(identityProvider);

                    if (providerSupportsSignout)
                    {
                        if (viewModel.LogoutId == null)
                        {
                            viewModel.LogoutId = await _interactionService.CreateLogoutContextAsync();
                        }

                        viewModel.LoginProvider = identityProvider;
                    }
                }
            }

            return viewModel;
        }

        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var viewModel = new LogoutViewModel
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = AccountConfig.ShowLogoutPrompt
            };

            var user = _httpContextAccessor.HttpContext.User;

            if (user?.Identity.IsAuthenticated != true)
            {
                viewModel.ShowLogoutPrompt = false;

                return viewModel;
            }

            var context = await _interactionService.GetLogoutContextAsync(logoutId);

            if (context?.ShowSignoutPrompt == false)
            {
                viewModel.ShowLogoutPrompt = false;

                return viewModel;
            }

            return viewModel;
        }
    }
}