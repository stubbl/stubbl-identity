namespace Stubbl.Identity.MvcClient.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class SecureController : Controller
    {
        [Authorize]
        [HttpGet("/secure", Name = "Secure")]
        public IActionResult Secure()
        {
            ViewData["Message"] = "Secure page.";

            return View();
        }
    }
}
