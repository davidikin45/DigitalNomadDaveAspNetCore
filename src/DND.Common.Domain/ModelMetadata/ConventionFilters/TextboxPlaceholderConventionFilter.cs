using DND.Common.Infrastructure.Interfaces.Domain.ModelMetadata;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;

namespace DND.Common.Domain.ModelMetadata
{
    public class TextboxPlaceholderConventionFilter : IDisplayMetadataFilter
    {
        private readonly DisplayConventionsDisableSettings _displayConventionsDisableSettings;
        public TextboxPlaceholderConventionFilter(IOptions<DisplayConventionsDisableSettings> displayConventionsDisableSettings)
        {
            _displayConventionsDisableSettings = displayConventionsDisableSettings.Value;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if (!_displayConventionsDisableSettings.TextboxPlaceholder)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;
                var displayName = "";
                if (modelMetadata.DisplayName != null)
                {
                    displayName = modelMetadata.DisplayName.Invoke();
                }
                var placeholder = "";
                if (modelMetadata.Placeholder != null)
                {
                    placeholder = modelMetadata.Placeholder.Invoke();
                }

                if (!string.IsNullOrEmpty(displayName) &&
                      string.IsNullOrEmpty(placeholder))
                {
                    context.DisplayMetadata.Placeholder = () => displayName + "...";
                }
            }
        }
    }
}