using System.ComponentModel.DataAnnotations;

namespace Stubbl.Identity.Models.AddPassword
{
    public class AddPasswordInputModel
    {
        [Required]
        [RegularExpression(".{8,}", ErrorMessage = "The password must be at least 8 characters long")]
        public string Password { get; set; }
    }
}
