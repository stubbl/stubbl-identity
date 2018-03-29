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

        [HttpPost("/link-external-login", Name = "LinkLogin")]
        [ValidateAntiForgeryToken]
        public IActionResult LinkExternalLogin([FromQuery] string provider)
        {
            var redirectUrl = Url.RouteUrl("LinkLoginCallback");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));

            return Challenge(properties, provider);
        }
    }
}