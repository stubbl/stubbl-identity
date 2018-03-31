using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.ResetPassword
{
    public class ResetPasswordInputModel
    {
        public string Code { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(".{8,}", ErrorMessage = "The password must be at least 8 characters long")]
        public string Password { get; set; }
    }
}