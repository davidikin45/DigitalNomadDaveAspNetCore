using DND.Common.Infrastructure.Interfaces.Domain.ModelMetadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Common.Domain.ModelMetadata
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