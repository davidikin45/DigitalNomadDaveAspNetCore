using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Solution.Base.ModelMetadataCustom.Interfaces;
using System;

namespace Solution.Base.ModelMetadataCustom.DisplayAttributes
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