using DND.Base.ModelMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Base.ModelMetadata
{
	public class HtmlByNameFilter : IMetadataFilter
    {
		private static readonly HashSet<string> TextAreaFieldNames =
				new HashSet<string>
						{
							"html"
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
                modelMetadata.DataTypeName = "Html";
            }
        }
    }
}