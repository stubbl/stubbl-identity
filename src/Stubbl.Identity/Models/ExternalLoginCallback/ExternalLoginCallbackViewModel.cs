namespace Stubbl.Identity.Models.ExternalLoginCallback
{
    public class ExternalLoginCallbackViewModel : ExternalLoginCallbackInputModel
    {
        public ExternalLoginCallbackViewModel(string loginProvider, string returnUrl)
        {
            LoginProvider = loginProvider;
            ReturnUrl = returnUrl;
        }

        public string LoginProvider { get; }
        public string ReturnUrl { get; }
    }
}