using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class AlignRightAttribute : Attribute, IDisplayMetadataAttribute
    {
        public bool AlignRight { get; set; } = true;

        public AlignRightAttribute()
        {
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["AlignRight"] = AlignRight;
        }
    }
}