namespace Stubbl.Identity.Models.ExternalLogin
{
    public class ExternalLoginViewModel : ExternalLoginInputModel
    {
        public ExternalLoginViewModel(string loginProvider, string returnUrl)
        {
            LoginProvider = loginProvider;
            ReturnUrl = returnUrl;
        }

        public string LoginProvider { get; }
        public string ReturnUrl { get; }
    }
}