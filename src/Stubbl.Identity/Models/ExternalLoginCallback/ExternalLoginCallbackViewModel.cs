namespace Stubbl.Identity.Models.ExternalLoginCallback
{
    public class ExternalLoginCallbackViewModel : ExternalLoginCallbackInputModel
    {
        public string LoginProvider { get; set; }
        public string ReturnUrl { get; set; }
    }
}