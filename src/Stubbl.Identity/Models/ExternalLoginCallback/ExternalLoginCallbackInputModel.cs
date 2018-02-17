namespace Stubbl.Identity.Models.ExternalLoginCallback
{
    using System.ComponentModel.DataAnnotations;

    public class ExternalLoginCallbackInputModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter a first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name")]
        public string LastName { get; set; }
    }
}
