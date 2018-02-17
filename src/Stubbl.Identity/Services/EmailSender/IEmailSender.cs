namespace Stubbl.Identity.Services.EmailSender
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string message);
    }
}
