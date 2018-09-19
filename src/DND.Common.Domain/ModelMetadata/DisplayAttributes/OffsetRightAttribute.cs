using DND.Common.Infrastructure.Interfaces.Domain.ModelMetadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Common.Domain.ModelMetadata
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