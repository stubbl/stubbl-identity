namespace Stubbl.Identity
{
    using Microsoft.AspNetCore.Identity;
    using System.Globalization;

    public class StubblIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = string.Format(CultureInfo.CurrentCulture, "The email address '{0}' is already taken.", userName)
            };
        }
    }
}
