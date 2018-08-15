using DND.Common.AppSettings;
using DND.Common.ModelMetadataCustom.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common.ModelMetadataCustom.ConventionFilters
{
    public class TextAreaByNameConventionFilter : IDisplayMetadataFilter
    {
        private readonly DisplayConventionsDisableOptions _displayConventionsDisableOptions;
        public TextAreaByNameConventionFilter(IOptions<DisplayConventionsDisableOptions> displayConventionsDisableOptions)
        {
            _displayConventionsDisableOptions = displayConventionsDisableOptions.Value;
        }

        private static readonly HashSet<string> TextAreaFieldNames =
				new HashSet<string>
						{
							"body",
                            "comments"
                        };

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if (!_displayConventionsDisableOptions.TextAreaByName)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;

                if (!string.IsNullOrEmpty(propertyName) &&
                    string.IsNullOrEmpty(modelMetadata.DataTypeName) &&
                    TextAreaFieldNames.Any(propertyName.ToLower().Contains))
                {
                    modelMetadata.DataTypeName = "MultilineText";
                    modelMetadata.AdditionalValues["MultilineTextRows"] = 7;
                    modelMetadata.AdditionalValues["MultilineTextHTML"] = false;
                }
            }
        }
    }
}