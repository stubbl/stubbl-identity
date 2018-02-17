namespace Stubbl.Identity.Services.EmailSender
{
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class LoggingEmailSender : IEmailSender
    {
        private readonly ILogger<LoggingEmailSender> _logger;

        public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string emailAddress, string subject, string message)
        {
            _logger.LogInformation("Sent email to {EmailAddress} with subject {Subject} and message {Message}", emailAddress, subject, message);

            return Task.CompletedTask;
        }
    }
}
