namespace Stubbl.Identity.Models.Consent
{
    public class Scope
    {
        public Scope(string name, string displayName, string description, bool @checked, bool emphasize,
            bool required)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
            Checked = @checked;
            Emphasize = emphasize;
            Required = required;
        }

        public bool Checked { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
    }
}