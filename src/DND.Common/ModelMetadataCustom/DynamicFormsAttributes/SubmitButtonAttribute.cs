﻿using DND.Common.ModelMetadataCustom.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Common.ModelMetadataCustom.DynamicFormsAttributes
{
    public class SubmitButtonAttribute : Attribute, IDisplayMetadataAttribute
    {
        public string @Class { get; set; } = "btn btn-primary btn-sm";

        public SubmitButtonAttribute()
        {

        }

        public SubmitButtonAttribute(string @class)
        {
            @Class = @class;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "SubmitButton";
            modelMetadata.DisplayName = () => "";
            modelMetadata.AdditionalValues["SubmitButtonClass"] = Class;
        }
    }
}