namespace Stubbl.Identity.Models.Consent
{
    using System.Collections.Generic;

    public class ConsentInputModel
    {
        public string Button { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
    }
}
