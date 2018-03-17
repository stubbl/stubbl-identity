using System.Collections.Generic;

namespace Stubbl.Identity.Models.Consent
{
    public class ConsentInputModel
    {
        public string Button { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
    }
}