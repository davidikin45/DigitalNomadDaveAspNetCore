using Solution.Base.ModelMetadataCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Solution.Base.ModelMetadataCustom.Interfaces;

namespace Solution.Base.ModelMetadataCustom.ConventionFilters
{
	public class TextAreaByNameFilter : IMetadataFilter
	{
		private static readonly HashSet<string> TextAreaFieldNames =
				new HashSet<string>
						{
							"body",
                            "message",
                            "comments",
                            "text"
                        };

        public void TransformMetadata(DisplayMetadataProviderContext context)
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