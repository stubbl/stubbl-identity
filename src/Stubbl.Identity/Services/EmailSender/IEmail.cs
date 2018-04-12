using MimeKit;

namespace Stubbl.Identity.Services.EmailSender
{
    public interface IEmail
    {
        string Body { get; }
        InternetAddressList From { get; }
        string Subject { get; }
        InternetAddressList To { get; }
    }
}