namespace Stubbl.Identity.Models.Logout
{
    public class LogoutViewModel : LogoutInputModel
    {
        public LogoutViewModel(bool showLogoutPrompt)
        {
            ShowLogoutPrompt = showLogoutPrompt;
        }

        public bool ShowLogoutPrompt { get; }
    }
}