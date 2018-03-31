using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ChangePassword;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class ChangePasswordController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public ChangePasswordController(SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/change-password", Name = "ChangePassword")]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            if (!await _userManager.HasPasswordAsync(user))
            {
                return RedirectToRoute("AddPassword");
            }

            return View();
        }

        [HttpPost("/change-password", Name = "ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            if (!await _userManager.HasPasswordAsync(user))
            {
                return RedirectToRoute("AddPassword");
            }

            var result = await _userManager.ChangePasswordAsync(user, inputModel.CurrentPassword, inputModel.NewPassword);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToRoute("Home", new { Message = "Your password has been changeed." });
        }
    }
}
