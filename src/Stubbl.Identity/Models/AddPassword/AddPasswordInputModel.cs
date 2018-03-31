using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.AddPassword
{
    public class AddPasswordInputModel
    {
        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(".{8,}", ErrorMessage = "The password must be at least 8 characters long")]
        public string Password { get; set; }
    }
}
