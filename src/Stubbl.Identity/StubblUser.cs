using CodeContrib.AspNetCore.Identity.MongoDB;

namespace Stubbl.Identity
{
    public class StubblUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}