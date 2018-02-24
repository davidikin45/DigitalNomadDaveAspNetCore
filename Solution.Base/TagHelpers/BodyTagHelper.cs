using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Solution.Base.Scripts;

namespace Solution.Base.TagHelpers
{
    [HtmlTargetElement("Body")]
    public class BodyTagHelper : TagHelper
    {
        private readonly IScriptManagerSingleton _scriptManager;
      
        public BodyTagHelper(IScriptManagerSingleton scriptManager)
        {
            _scriptManager = scriptManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            (await output.GetChildContentAsync()).GetContent();

            var sb = new StringBuilder();
            if (_scriptManager.ScriptTexts.Count > 0)
            {
                sb.AppendLine("<script type='text/javascript'>");
                foreach (var scriptText in _scriptManager.ScriptTexts)
                    sb.AppendLine(scriptText);
                sb.AppendLine("</script>");
            }
            output.PostContent.AppendHtml(sb.ToString());
        }
    }
}