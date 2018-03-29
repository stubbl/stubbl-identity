namespace Stubbl.Identity.Models.ForgotPassword
{
    public class ForgotPasswordViewModel : ForgotPasswordInputModel
    {
        public ForgotPasswordViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}