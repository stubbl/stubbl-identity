using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.Controllers
{
    public class ExternalLoginController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;

        public ExternalLoginController(SignInManager<StubblUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost("/external-login", Name = "ExternalLogin")]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin([FromForm] string loginProvider, [FromQuery] string returnUrl)
        {
            var redirectUrl = Url.RouteUrl("ExternalLoginCallback", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(loginProvider, redirectUrl);

            return Challenge(properties, loginProvider);
        }
    }
}