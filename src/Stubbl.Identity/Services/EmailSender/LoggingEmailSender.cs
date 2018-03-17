using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stubbl.Identity.Services.EmailSender
{
    public class LoggingEmailSender : IEmailSender
    {
        private readonly ILogger<LoggingEmailSender> _logger;

        public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string emailAddress, string subject, string message)
        {
            _logger.LogInformation("Sent email to {EmailAddress} with subject {Subject} and message {Message}",
                emailAddress, subject, message);

            return Task.CompletedTask;
        }
    }
}