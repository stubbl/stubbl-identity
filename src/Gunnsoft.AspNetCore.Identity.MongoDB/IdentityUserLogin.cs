namespace CodeContrib.AspNetCore.Identity.MongoDB.Data
{
    public class IdentityUserLogin
    {
        public string LoginProvider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderKey { get; set; }
    }
}