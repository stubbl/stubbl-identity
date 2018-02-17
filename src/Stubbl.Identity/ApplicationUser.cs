namespace Stubbl.Identity
{
    using CodeContrib.AspNetCore.Identity.MongoDB;

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
