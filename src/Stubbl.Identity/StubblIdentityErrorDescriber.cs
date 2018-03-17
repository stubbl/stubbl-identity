using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace Stubbl.Identity
{
    public class StubblIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = string.Format(CultureInfo.CurrentCulture, "The email address '{0}' is already taken.",
                    userName)
            };
        }
    }
}