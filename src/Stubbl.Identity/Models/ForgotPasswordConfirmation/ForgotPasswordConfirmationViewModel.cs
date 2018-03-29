namespace Stubbl.Identity.Models.ForgotPasswordConfirmation
{
    public class ForgotPasswordConfirmationViewModel
    {
        public ForgotPasswordConfirmationViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}