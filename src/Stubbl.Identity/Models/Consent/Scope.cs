namespace Stubbl.Identity.Models.Consent
{
    public class Scope
    {
        public bool Checked { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
    }
}