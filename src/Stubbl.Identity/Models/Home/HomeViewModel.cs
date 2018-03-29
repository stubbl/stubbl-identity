namespace Stubbl.Identity.Models.Home
{
    public class HomeViewModel
    {
        public HomeViewModel(string firstName, string lastName, string emailAddress, bool hasPassword,
            int loginsCount)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            HasPassword = hasPassword;
            LoginsCount = loginsCount;
        }

        public string EmailAddress { get; }
        public string FirstName { get; }
        public bool HasPassword { get; }
        public string LastName { get; }
        public int LoginsCount { get; }
    }
}
