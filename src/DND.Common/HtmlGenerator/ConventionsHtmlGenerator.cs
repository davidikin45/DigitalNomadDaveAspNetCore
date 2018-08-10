using DND.Common.AppSettings;
using DND.Common.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DND.Common.HtmlGenerator
{
    public class ConventionsHtmlGenerator : DefaultHtmlGenerator, IHtmlGenerator
    {
        private readonly DisplayConventionsDisableOptions _displayConventionsDisableOptions;

        public ConventionsHtmlGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor, IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder, ValidationHtmlAttributeProvider validationAttributeProvider, IOptions<DisplayConventionsDisableOptions> displayConventionsDisableOptions)
            :base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
        {
            _displayConventionsDisableOptions = displayConventionsDisableOptions.Value;
        }

        public override TagBuilder GenerateLabel(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string labelText, object htmlAttributes)
        {
            var builder = base.GenerateLabel(viewContext, modelExplorer, expression, labelText, htmlAttributes);

            var editOrCreateMode = ((viewContext.ViewData.ContainsKey("EditMode") && (Boolean)viewContext.ViewData["EditMode"]) || (viewContext.ViewData.ContainsKey("CreateMode") && (Boolean)viewContext.ViewData["CreateMode"])) && !(viewContext.ViewData.ContainsKey("DetailsMode") && (Boolean)viewContext.ViewData["DetailsMode"]);

            if (editOrCreateMode && !_displayConventionsDisableOptions.LabelRequiredAsterix && modelExplorer.Metadata.IsRequired)
            {
                var newLabelText = builder.InnerHtml.Render() + " *";
                builder.InnerHtml.SetContent(newLabelText);
            }

            return builder;
        }
    }
}
