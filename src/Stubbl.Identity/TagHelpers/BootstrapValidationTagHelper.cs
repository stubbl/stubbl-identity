using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stubbl.Identity.TagHelpers
{
    [HtmlTargetElement("input", Attributes = ForAttributeName)]
    [HtmlTargetElement("select", Attributes = ForAttributeName)]
    [HtmlTargetElement("textarea", Attributes = ForAttributeName)]
    public class BootstrapValidationTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)] public ModelExpression For { get; set; }

        [HtmlAttributeNotBound] [ViewContext] public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            ViewContext.ViewData.ModelState.TryGetValue(For.Name, out var modelState);

            if (modelState == null)
            {
                return;
            }

            var tagBuilder = new TagBuilder("div");

            if (modelState.Errors.Any())
            {
                tagBuilder.AddCssClass("is-invalid");
            }
            else if (!string.Equals(
                context.AllAttributes
                    .SingleOrDefault(a => string.Equals(a.Name, "type", StringComparison.InvariantCultureIgnoreCase))
                    ?.Value?.ToString(), "password", StringComparison.InvariantCulture))
            {
                tagBuilder.AddCssClass("is-valid");
            }

            output.MergeAttributes(tagBuilder);
        }
    }
}