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
        public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
        {
            var redirectUrl = Url.RouteUrl("ExternalLoginCallback", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }
    }
}