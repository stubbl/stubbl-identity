namespace Stubbl.Identity.Models.Register
{
    public class RegisterViewModel : RegisterInputModel
    {
        public RegisterViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; }
    }
}