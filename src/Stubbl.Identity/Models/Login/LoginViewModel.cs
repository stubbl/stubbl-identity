﻿namespace Stubbl.Identity.Models.Login
{
    using System.Collections.Generic;
    using System.Linq;

    public class LoginViewModel : LoginInputModel
    {
        public bool EnableLocalLogin { get; set; }
        public IReadOnlyCollection<string> LoginProviders { get; set; }
        public bool IsExternalLoginOnly => !EnableLocalLogin && LoginProviders?.Count() == 1;
    }
}
