using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("", Name = "Home")]
        public IActionResult Home()
        {
            return View();
        }
    }
}