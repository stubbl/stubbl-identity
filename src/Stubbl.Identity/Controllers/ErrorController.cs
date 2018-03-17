using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/error", Name = "Error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}