using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.Domain.ModelMetadata
{
    using DND.Common.Infrastructure.Interfaces.Domain.ModelMetadata;
    using DND.Common.Infrastructure.Settings;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Linq;
    public class HtmlByNameConventionFilter : IDisplayMetadataFilter
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;
        public HtmlByNameConventionFilter(IOptions<DisplayConventionsDisableSettings> displayConventionsDisableSettings)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings.Value;
        }

        private static readonly HashSet<string> TextAreaFieldNames =
                new HashSet<string>
                        {
                            "html"
                        };

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if(!_displayConventionsDisableSettings.HtmlByName)
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