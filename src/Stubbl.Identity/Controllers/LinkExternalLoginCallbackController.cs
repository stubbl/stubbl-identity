using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class LinkExternalLoginCallbackController : Controller
    {
        private readonly ILogger<LinkExternalLoginCallbackController> _logger;
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public LinkExternalLoginCallbackController(ILogger<LinkExternalLoginCallbackController> logger,
            SignInManager<StubblUser> signInManager, UserManager<StubblUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/link-external-login-callback", Name = "LinkLoginCallback")]
        public async Task<ActionResult> LinkExternalSLoginCallback()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            var loginInfo = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));

            if (loginInfo == null)
            {
                _logger.LogWarning("Error loading external login information during callback");

                return RedirectToRoute("ManageLogins");
            }

            var result = await _userManager.AddLoginAsync(user, loginInfo);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            return RedirectToRoute("ManageLogins");
        }
    }
}