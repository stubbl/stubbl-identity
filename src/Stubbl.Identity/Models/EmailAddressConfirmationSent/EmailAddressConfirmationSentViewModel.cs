namespace Stubbl.Identity.Models.EmailAddressConfirmationSent
{
    public class EmailAddressConfirmationSentViewModel
    {
        public EmailAddressConfirmationSentViewModel(string emailAddress, string userId, bool allowResend,
            string returnUrl)
        {
            EmailAddress = emailAddress;
            UserId = userId;
            AllowResend = allowResend;
            ReturnUrl = returnUrl;
        }

        public bool AllowResend { get; }
        public string EmailAddress { get; }
        public string ReturnUrl { get; }
        public string UserId { get; }
    }
}