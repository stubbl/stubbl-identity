using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stubbl.Identity.Models.Diagnostics;
using Stubbl.Identity.Options;

namespace Stubbl.Identity.Controllers
{
    [Authorize]
    public class DiagnosticsController : Controller
    {
        private readonly DiagnosticsOptions _diagnosticsOptions;

        public DiagnosticsController(IOptions<DiagnosticsOptions> diagnosticsOptions)
        {
            _diagnosticsOptions = diagnosticsOptions.Value;
        }

        [HttpGet("/diagnostics", Name = "Diagnostics")]
        public async Task<IActionResult> Diagnostics(string secret)
        {
            if (_diagnosticsOptions.Secret == null && secret != _diagnosticsOptions.Secret)
            {
                return NotFound();
            }

            var viewModel = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());

            return View(viewModel);
        }
    }
}