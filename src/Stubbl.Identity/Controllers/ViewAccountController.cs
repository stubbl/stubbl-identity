namespace Stubbl.Identity.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ViewAccountController : Controller
    {
        [HttpGet("/", Name = "ViewAccount")]
        public IActionResult ViewAccount()
        {
            return View();
        }
    }
}