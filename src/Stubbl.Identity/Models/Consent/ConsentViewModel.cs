using System.Collections.Generic;

namespace Stubbl.Identity.Models.Consent
{
    public class ConsentViewModel : ConsentInputModel
    {
        public ConsentViewModel(string clientName, string clientUrl, string clientLogoUrl, 
            IReadOnlyCollection<Scope> identityScopes, IReadOnlyCollection<Scope> resourceScopes,
            bool allowRememberConsent)
        {
            ClientName = clientName;
            ClientUrl = clientUrl;
            ClientLogoUrl = clientLogoUrl;
            IdentityScopes = identityScopes;
            ResourceScopes = resourceScopes;
            AllowRememberConsent = allowRememberConsent;
        }

        public bool AllowRememberConsent { get; }
        public string ClientName { get; }
        public string ClientLogoUrl { get; }
        public string ClientUrl { get; }
        public IReadOnlyCollection<Scope> IdentityScopes { get; }
        public IReadOnlyCollection<Scope> ResourceScopes { get; }
    }
}