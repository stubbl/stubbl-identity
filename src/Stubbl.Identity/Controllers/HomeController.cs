namespace Stubbl.Identity.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

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