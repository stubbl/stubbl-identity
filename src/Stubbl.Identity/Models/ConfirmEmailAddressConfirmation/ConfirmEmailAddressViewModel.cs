namespace Stubbl.Identity.Models.ConfirmEmailAddressConfirmation
{
    public class ConfirmEmailAddressConfirmationViewModel
    {
        public ConfirmEmailAddressConfirmationViewModel(string emailAddress, string returnUrl)
        {
            EmailAddress = emailAddress;
            ReturnUrl = returnUrl;
        }

        public string EmailAddress { get; }
        public string ReturnUrl { get; }
    }
}