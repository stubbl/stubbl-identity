using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet("/", Name = "Home")]
        public IActionResult Home()
        {
            return View();
        }
    }
}