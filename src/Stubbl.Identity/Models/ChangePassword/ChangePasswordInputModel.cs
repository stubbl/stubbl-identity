using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.ChangePassword
{
    public class ChangePasswordInputModel
    {
        [Required(ErrorMessage = "Please enter your current password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please enter your new password")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string NewPassword { get; set; }
    }
}
