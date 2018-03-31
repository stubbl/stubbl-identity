using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.ChangeEmailAddress
{
    public class ChangeEmailAddressInputModel
    {
        [Required(ErrorMessage = "Please enter your new email address")]
        public string EmailAddress { get; set; }
    }
}
