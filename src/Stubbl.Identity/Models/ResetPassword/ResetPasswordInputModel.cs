namespace Stubbl.Identity.Models.ResetPassword
{
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordInputModel
    {
        public string Code { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [Required]
        [RegularExpression(".{8,}", ErrorMessage = "The password must be at least 8 characters long")]
        public string Password { get; set; }
    }
}
