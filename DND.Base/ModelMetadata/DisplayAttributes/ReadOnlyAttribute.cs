using DND.Base.ModelMetadata;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Base.ModelMetadata
{
	public class ReadOnlyAttribute : Attribute, IMetadataAttribute
    {
        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            if (string.IsNullOrEmpty(modelMetadata.DataTypeName))
            {
                modelMetadata.DataTypeName = "ReadOnly";
            }
        }
    }
}