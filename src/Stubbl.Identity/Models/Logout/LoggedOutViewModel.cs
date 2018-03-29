namespace Stubbl.Identity.Models.Logout
{
    public class LoggedOutViewModel
    {
        public LoggedOutViewModel(string clientName, string logoutId, string logoutIframeUrl,
            bool automaticRedirectAfterLogout, string postLogoutRedirectUri, string externalLoginProvider)
        {
            ClientName = clientName;
            LogoutId = logoutId;
            LogoutIframeUrl = logoutIframeUrl;
            AutomaticRedirectAfterLogout = automaticRedirectAfterLogout;
            PostLogoutRedirectUri = postLogoutRedirectUri;
            ExternalLoginProvider = externalLoginProvider;
        }

        public bool AutomaticRedirectAfterLogout { get; }
        public string ClientName { get; }
        public string ExternalLoginProvider { get; }
        public string LogoutId { get; }
        public string LogoutIframeUrl { get; }
        public string PostLogoutRedirectUri { get; }
    }
}