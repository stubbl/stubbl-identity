using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.ChangePassword
{
    public class ChangePasswordInputModel
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [RegularExpression(".{8,}", ErrorMessage = "The password must be at least 8 characters long")]
        public string NewPassword { get; set; }
    }
}
