using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace Stubbl.Identity.Models.Login
{
    public class LoginViewModel : LoginInputModel
    {
        public LoginViewModel(IReadOnlyCollection<AuthenticationScheme> loginProviders, bool allowLocalLogin)
        {
            LoginProviders = loginProviders;
            AllowLocalLogin = allowLocalLogin;
        }

        public bool AllowLocalLogin { get; }
        public bool IsExternalLoginOnly => !AllowLocalLogin && LoginProviders?.Count() == 1;
        public IReadOnlyCollection<AuthenticationScheme> LoginProviders { get; }
    }
}