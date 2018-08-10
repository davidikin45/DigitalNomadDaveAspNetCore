using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.ConventionFilters
{
    using DND.Common.AppSettings;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Linq;
    public class HtmlByNameConventionFilter : IDisplayMetadataFilter
    {
        private readonly DisplayConventionsDisableOptions _displayConventionsDisableOptions;
        public HtmlByNameConventionFilter(IOptions<DisplayConventionsDisableOptions> displayConventionsDisableOptions)
        {
            _displayConventionsDisableOptions = displayConventionsDisableOptions.Value;
        }

        private static readonly HashSet<string> TextAreaFieldNames =
                new HashSet<string>
                        {
                            "html"
                        };

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if(!_displayConventionsDisableOptions.HtmlByName)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;

                if (!string.IsNullOrEmpty(propertyName) &&
                      string.IsNullOrEmpty(modelMetadata.DataTypeName) &&
                      TextAreaFieldNames.Any(propertyName.ToLower().Contains))
                {
                    modelMetadata.DataTypeName = "Html";
                }
            }
        }
    }
}