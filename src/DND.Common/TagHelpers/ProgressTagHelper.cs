using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.TagHelpers
{
    [HtmlTargetElement("progress")]
    public class ProgressTagHelper : TagHelper
    {
        public string Percent { get; set; }

        public ProgressTagHelper()
        {

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.SetHtmlContent(
                $@"<div class=""progress"">
                        <div class=""progress-bar bg-info"" style=""width: { Percent }%""> { Percent }%</div>
                    </div>");
        }

    }
}
