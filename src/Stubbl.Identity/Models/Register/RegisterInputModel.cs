namespace Stubbl.Identity.Models.Register
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter a first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(".{8,}", ErrorMessage = "The password must be at least 8 characters long")]
        public string Password { get; set; }
    }
}
