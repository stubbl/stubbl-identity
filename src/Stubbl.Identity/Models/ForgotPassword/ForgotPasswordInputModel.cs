namespace Stubbl.Identity.Models.ForgotPassword
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordInputModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }
    }
}
