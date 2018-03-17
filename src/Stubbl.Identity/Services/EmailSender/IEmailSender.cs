using System.Threading.Tasks;

namespace Stubbl.Identity.Services.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string message);
    }
}