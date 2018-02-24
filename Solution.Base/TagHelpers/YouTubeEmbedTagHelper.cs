using System;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Solution.Base.Scripts;

namespace Solution.Base.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("YouTubeEmbed")]
    public class YouTubeEmbedTagHelper : TagHelper
    {
        private readonly IScriptManagerSingleton _scriptManager;

        public YouTubeEmbedTagHelper(IScriptManagerSingleton scriptManager)
        {
            _scriptManager = scriptManager;
        }

        public string YouTubeId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
        
            output.Attributes.Add(new TagHelperAttribute("id", YouTubeId));
            output.Attributes.Add(new TagHelperAttribute("class", "youtubeLoad"));
   
            var scriptTextExecute = string.Format(@"
                 $(document).ready(function () {{
                        $('.youtubeLoad').nonSuckyYouTubeEmbed();
                }});
            ");
            _scriptManager.AddScriptText(scriptTextExecute);
        }
    }
}