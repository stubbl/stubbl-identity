namespace Stubbl.Identity.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.IO;

    public class CspReportController : Controller
    {
        private readonly ILogger<CspReportController> _logger;

        public CspReportController(ILogger<CspReportController> logger)
        {
            _logger = logger;
        }

        // TODO Ensure this is only called when referred from itself
        [HttpPost("/csp-report", Name = "CspReport")]
        public IActionResult CspReport()
        {
            using (var bodyReader = new StreamReader(Request.Body))
            {
                var violation = bodyReader.ReadToEnd();

                _logger.LogWarning(violation);

                // TODO Store in Azure Table Storage
            }

            return new EmptyResult();
        }
    }
}
