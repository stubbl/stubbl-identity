namespace Stubbl.Identity.HtmlHelpers
{
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public static class HtmlHelperLoginProviderExtensions
    {
        private static readonly Dictionary<string, string> _loginProviderIcons = new Dictionary<string, string>()
        {
            { "GitHub", "fa-github" },
            { "Google", "fa-google" },
            { "Microsoft", "fa-windows" }
        };

        public static IHtmlContent LoginProviderIcon(this IHtmlHelper htmlHelper, string loginProvider)
        {
            if (!_loginProviderIcons.ContainsKey(loginProvider))
            {
                return null;
            }

            return new HtmlString($"<i class=\"fa {_loginProviderIcons[loginProvider]}\"></i>");
        }
    }
}
