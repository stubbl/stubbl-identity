namespace Stubbl.Identity.Models.EmailAddressConfirmationSent
{
    public class EmailAddressConfirmationSentViewModel
    {
        public EmailAddressConfirmationSentViewModel(string emailAddress, string userId, bool allowResendConfirmation,
            string returnUrl)
        {
            EmailAddress = emailAddress;
            UserId = userId;
            AllowResendConfirmation = allowResendConfirmation;
            ReturnUrl = returnUrl;
        }

        public bool AllowResendConfirmation { get; }
        public string EmailAddress { get; }
        public string ReturnUrl { get; }
        public string UserId { get; }
    }
}