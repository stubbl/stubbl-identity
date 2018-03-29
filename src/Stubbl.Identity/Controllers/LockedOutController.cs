using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class LockedOutController : Controller
    {
        [HttpPost("/locked-out", Name = "LockedOut")]
        public IActionResult LockedOut()
        {
            return View();
        }
    }
}
