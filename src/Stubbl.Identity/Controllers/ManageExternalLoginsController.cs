using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stubbl.Identity.Models.ManageExternalLogins;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class ManageExternalLoginsController : Controller
    {
        private readonly SignInManager<StubblUser> _signInManager;
        private readonly UserManager<StubblUser> _userManager;

        public ManageExternalLoginsController(SignInManager<StubblUser> signInManager,
            UserManager<StubblUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/manage-logins", Name = "ManageExternalLogins")]
        public async Task<IActionResult> ManageExternalLogins()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("Error");
            }

            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(@as => userLogins.All(ul => @as.Name != ul.LoginProvider))
                .ToList();

            var viewModel = new ManageExternalLoginsViewModel
            (
                userLogins.ToList(),
                otherLogins.ToList(),
                user.PasswordHash != null || userLogins.Count > 1
            );

            return View(viewModel);
        }
    }
}
