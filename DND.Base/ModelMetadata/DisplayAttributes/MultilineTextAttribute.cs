using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Base.ModelMetadata
{
    public class MultilineTextAttribute : Attribute, IMetadataAttribute
    {
        public int Rows { get; set; }
        public Boolean HTML { get; set; }

        public MultilineTextAttribute()
        {
            Rows = 7;
            HTML = false;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "MultilineText";
            modelMetadata.AdditionalValues["MultilineTextRows"] = Rows;
            modelMetadata.AdditionalValues["MultilineTextHTML"] = HTML;
        }
    }
}