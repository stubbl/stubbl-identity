namespace Stubbl.Identity.Models.Consent
{
    using System.Collections.Generic;

    public class ConsentViewModel : ConsentInputModel
    {
        public bool AllowRememberConsent { get; set; }
        public string ClientName { get; set; }
        public string ClientLogoUrl { get; set; }
        public string ClientUrl { get; set; }
        public IReadOnlyCollection<Scope> IdentityScopes { get; set; }
        public IReadOnlyCollection<Scope> ResourceScopes { get; set; }
    }
}
