namespace Stubbl.Identity.MvcClient.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        [HttpGet("", Name = "Home")]
        public IActionResult Home()
        {
            return View();
        }
    }
}
