using DND.Common.ModelMetadataCustom.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class BooleanTrueFalseButtonsAttribute : Attribute, IDisplayMetadataAttribute
    {
        public BooleanTrueFalseButtonsAttribute()
        {

        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "BooleanTrueFalseButtons";
        }
    }
}