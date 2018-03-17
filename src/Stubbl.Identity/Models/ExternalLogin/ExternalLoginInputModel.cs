using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.ExternalLogin
{
    public class ExternalLoginInputModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }
    }
}