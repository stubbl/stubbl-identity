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

        [HttpGet("/link-external-login-callback", Name = "LinkExternalLoginCallback")]
        public async Task<ActionResult> LinkExternalLoginCallback()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));

            if (externalLoginInfo == null)
            {
                return View("Error");
            }

            var result = await _userManager.AddLoginAsync(user, externalLoginInfo);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            return RedirectToRoute("ManageExternalLogins");
        }
    }
}