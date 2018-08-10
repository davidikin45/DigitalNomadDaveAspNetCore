using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class OffsetRightAttribute : Attribute, IDisplayMetadataAttribute
    {
        public int OffSetColumns { get; set; } = 0;

        public OffsetRightAttribute(int offSetColumns)
        {
            OffSetColumns = offSetColumns;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.AdditionalValues["OffsetRight"] = OffSetColumns;
        }
    }
}