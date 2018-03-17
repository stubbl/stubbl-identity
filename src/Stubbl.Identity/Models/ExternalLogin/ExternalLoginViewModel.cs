namespace Stubbl.Identity.Models.ExternalLogin
{
    public class ExternalLoginViewModel : ExternalLoginInputModel
    {
        public string LoginProvider { get; set; }
        public string ReturnUrl { get; set; }
    }
}