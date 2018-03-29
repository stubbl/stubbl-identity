namespace Stubbl.Identity.Models.ResetPasswordConfirmation
{
    public class ResetPasswordConfirmationViewModel
    {
        public ResetPasswordConfirmationViewModel(string emailAddress, string returnUrl)
        {
            EmailAddress = emailAddress;
            ReturnUrl = returnUrl;
        }

        public string EmailAddress { get; }
        public string ReturnUrl { get; }
    }
}