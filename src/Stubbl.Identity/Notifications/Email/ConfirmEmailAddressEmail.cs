using MimeKit;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Notifications.Email
{
    public class ConfirmEmailAddressEmail : IEmail
    {
        public ConfirmEmailAddressEmail(string to, string callbackUrl)
        {
            From = new InternetAddressList { new MailboxAddress("Stubbl", "noreply@stubbl.it") };
            To = new InternetAddressList { new MailboxAddress(to) };
            Subject = "Stubbl: Please confirm your email address";
            Body =
                $"Please confirm your email address by clicking the following link: <a href=\"{callbackUrl}\">{callbackUrl}</a>.";
        }

        public string Body { get; }
        public InternetAddressList From { get; }
        public string Subject { get; }
        public InternetAddressList To { get; }
    }
}
