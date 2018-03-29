using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.Home;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public HomeController(SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/", Name = "Home")]
        public async Task<IActionResult> Home()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            var viewModel = new HomeViewModel
            (
                user.FirstName,
                user.LastName,
                await _userManager.GetEmailAsync(user),
                await _userManager.HasPasswordAsync(user),
                (await _userManager.GetLoginsAsync(user)).Count
            );

            return View(viewModel);
        }
    }
}