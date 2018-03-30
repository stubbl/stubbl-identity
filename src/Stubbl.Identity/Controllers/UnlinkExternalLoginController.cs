using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.UnlinkExternalLogin;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class UnlinkExternalLoginController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public UnlinkExternalLoginController(SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("/unllink-external-login", Name = "UnlinkExternalLogin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkExternalLogin([FromForm] UnlinkExternalLoginInputModel inputModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.RemoveLoginAsync(user, inputModel.LoginProvider, inputModel.ProviderKey);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToRoute("ManageExternalLogins");
        }
    }
}