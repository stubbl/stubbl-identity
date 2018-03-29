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

        public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            var user = _httpContextAccessor.HttpContext.User;
            string externalLoginProvider = null;

            if (user?.Identity.IsAuthenticated == true)
            {
                var identityProvider = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                if (identityProvider != null && identityProvider != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout =
                        await _httpContextAccessor.HttpContext.GetSchemeSupportsSignOutAsync(identityProvider);

                    if (providerSupportsSignout)
                    {
                        if (logoutId == null)
                        {
                            logoutId = await _interactionService.CreateLogoutContextAsync();
                        }

                        externalLoginProvider = identityProvider;
                    }
                }
            }

            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            return new LoggedOutViewModel
            (
                logoutRequest?.ClientName,
                logoutId,
                logoutRequest?.SignOutIFrameUrl,
                AccountConfig.AutomaticRedirectAfterLogout,
                logoutRequest?.PostLogoutRedirectUri,
                externalLoginProvider
            );
        }

        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var user = _httpContextAccessor.HttpContext.User;

            if (user?.Identity.IsAuthenticated != true
                || (await _interactionService.GetLogoutContextAsync(logoutId))?.ShowSignoutPrompt == false)
            {
                return new LogoutViewModel
                (
                    false
                )
                {
                    LogoutId = logoutId,
                };
            }

            return new LogoutViewModel
            (
                AccountConfig.ShowLogoutPrompt
            )
            {
                LogoutId = logoutId,
            };
        }

        [HttpGet("/logout", Name = "Logout")]
        public async Task<IActionResult> Logout([FromQuery] string logoutId)
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
        public async Task<IActionResult> Logout([FromForm] LogoutInputModel model)
        {
            var viewModel = await BuildLoggedOutViewModelAsync(model.LogoutId);

            await _signInManager.SignOutAsync();

            if (viewModel.ExternalLoginProvider != null)
            {
                var url = Url.RouteUrl("Logout", new {logoutId = viewModel.LogoutId});

                return SignOut(new AuthenticationProperties {RedirectUri = url}, viewModel.ExternalLoginProvider);
            }

            return View("LoggedOut", viewModel);
        }
    }
}