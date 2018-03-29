namespace Stubbl.Identity.Models.RegisterConfirmation
{
    public class RegisterConfirmationViewModel
    {
        public RegisterConfirmationViewModel(string emailAddress, string userId, bool allowResendConfirmation,
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