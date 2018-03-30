using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class LinkExternalLoginController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public LinkExternalLoginController(SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("/link-external-login", Name = "LinkExternalLogin")]
        [ValidateAntiForgeryToken]
        public IActionResult LinkExternalLogin([FromForm] string loginProvider)
        {
            var redirectUrl = Url.RouteUrl("LinkExternalLoginCallback");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(loginProvider, redirectUrl, _userManager.GetUserId(User));

            return Challenge(properties, loginProvider);
        }
    }
}