namespace Stubbl.Identity.Models.ResetPassword
{
    public class ResetPasswordViewModel : ResetPasswordInputModel
    {
        public ResetPasswordViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}