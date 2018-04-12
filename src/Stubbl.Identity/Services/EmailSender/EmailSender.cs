using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Stubbl.Identity.Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailSender(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task SendEmailAsync(IEmail email, CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.From.AddRange(email.From);
            message.To.AddRange(email.To);
            message.Subject = email.Subject;
            message.Body = new TextPart("html")
            {
                Text = email.Body
            };

            if (_hostingEnvironment.IsDevelopment())
            {
                await message.WriteToAsync(Path.Combine(AppContext.BaseDirectory, $"{message.To}-{DateTime.UtcNow:yyyy-MM-ddThh-mm-ss}.eml"), cancellationToken);
            }
            else
            {
                using (var smtpClient = new SmtpClient())
                {
                    var configurationSection = _configuration.GetSection("Smtp");
                    var host = configurationSection.GetValue<string>("Host");
                    var port = configurationSection.GetValue<int>("Port");
                    var username = configurationSection.GetValue<string>("Username");
                    var password = configurationSection.GetValue<string>("Password");

                    await smtpClient.ConnectAsync(host, port, cancellationToken: cancellationToken);
                    smtpClient.Authenticate(username, password, cancellationToken);

                    await smtpClient.SendAsync(message, cancellationToken);
                    await smtpClient.DisconnectAsync(true, cancellationToken);
                }
            }
        }
    }
}