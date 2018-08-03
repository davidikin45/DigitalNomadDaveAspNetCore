using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.TagHelpers
{
    [HtmlTargetElement("button", Attributes = "active-url")]
    [HtmlTargetElement("a", Attributes = "active-url")]
    public class ActiveTagHelper : TagHelper
    {
        public string ActiveUrl { get; set; }

        private IHttpContextAccessor httpService;

        public ActiveTagHelper(IHttpContextAccessor httpService)
        {
            this.httpService = httpService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (httpService.HttpContext.Request.Path.ToString().Contains(ActiveUrl))
            {
                var exsitingAttrs = output.Attributes["class"]?.Value;
                output.Attributes.SetAttribute("class",
                    "active " + exsitingAttrs.ToString());
            }
        }
    }
}
