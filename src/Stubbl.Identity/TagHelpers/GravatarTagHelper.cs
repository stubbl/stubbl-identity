using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stubbl.Identity.TagHelpers
{
    [HtmlTargetElement("img", Attributes = EmailAddressAttributeName)]
    public class GravatarTagHelper : TagHelper
    {
        private const string DefaultAttributeName = "asp-gravatar-default";
        private const string EmailAddressAttributeName = "asp-gravatar-emailaddress";
        private const string SizeAttributeName = "asp-gravatar-size";

        [HtmlAttributeName(DefaultAttributeName)]
        public string Default { get; set; }

        [HtmlAttributeName(EmailAddressAttributeName)]
        public string EmailAddress { get; set; }

        [HtmlAttributeName(SizeAttributeName)] public string Size { get; set; }

        [HtmlAttributeNotBound] [ViewContext] public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var md5Hash = GenerateMd5Hash(EmailAddress);
            var uri = new Uri($"https://gravatar.com/avatar/{md5Hash}?d={Default}&s={Size}");

            output.Attributes.SetAttribute("src",
                uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped));
        }

        private static string GenerateMd5Hash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value.Trim());
            var hash = md5.ComputeHash(inputBytes);
            var stringBuilder = new StringBuilder();

            foreach (var @byte in hash)
            {
                stringBuilder.Append(@byte.ToString("X2"));
            }

            return stringBuilder.ToString().ToLowerInvariant();
        }
    }
}