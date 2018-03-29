using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.AddPassword;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class AddPasswordController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public AddPasswordController(SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/add-password", Name = "AddPassword")]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                return RedirectToRoute("Home");
            }

            return View();
        }

        [HttpPost("/add-password", Name = "AddPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPassword([FromForm] AddPasswordInputModel inputModel)
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

            if (await _userManager.HasPasswordAsync(user))
            {
                return RedirectToRoute("Home");
            }

            var result = await _userManager.AddPasswordAsync(user, inputModel.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error adding password");

                return View(inputModel);
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToRoute("Home", new { Message = "Your password has been added." });
        }
    }
}
