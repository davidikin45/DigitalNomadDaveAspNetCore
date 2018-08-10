using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.DynamicFormsAttributes
{
    public class SubmitButtonAttribute : Attribute, IDisplayMetadataAttribute
    {
        public SubmitButtonAttribute()
        {
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "SubmitButton";
            modelMetadata.DisplayName = () => "";
        }
    }
}