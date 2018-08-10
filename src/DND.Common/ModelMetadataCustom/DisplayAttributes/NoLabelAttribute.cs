using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class NoLabelAttribute : Attribute, IDisplayMetadataAttribute
    {
        public bool NoLabel { get; set; } = true;

        public NoLabelAttribute()
        {
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["NoLabel"] = NoLabel;
        }
    }
}