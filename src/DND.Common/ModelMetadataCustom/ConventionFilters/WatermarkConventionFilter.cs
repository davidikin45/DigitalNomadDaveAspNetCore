using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.ConventionFilters
{
	public class WatermarkConventionFilter : IMetadataFilter
	{

        public void TransformMetadata(DisplayMetadataProviderContext context)
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