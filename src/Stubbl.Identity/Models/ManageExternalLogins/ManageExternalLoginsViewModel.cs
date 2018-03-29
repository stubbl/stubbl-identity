using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Stubbl.Identity.Models.ManageExternalLogins
{
    public class ManageExternalLoginsViewModel
    {
        public ManageExternalLoginsViewModel(IReadOnlyCollection<UserLoginInfo> currentLogins,
            IReadOnlyCollection<AuthenticationScheme> otherLogins, bool allowRemoval)
        {
            CurrentLogins = currentLogins;
            OtherLogins = otherLogins;
            AllowRemoval = allowRemoval;
        }

        public bool AllowRemoval { get; }
        public IReadOnlyCollection<UserLoginInfo> CurrentLogins { get; }
        public IReadOnlyCollection<AuthenticationScheme> OtherLogins { get; }
    }
}
