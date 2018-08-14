﻿using DND.Common.ModelMetadataCustom.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Common.ModelMetadataCustom.DisplayAttributes
{
    public class BooleanYesButtonAttribute : BooleanTextButtonAttribute, IDisplayMetadataAttribute
    {
        public BooleanYesButtonAttribute()
            :base("Yes")
        {

        }
    }

    public class BooleanTextButtonAttribute : Attribute, IDisplayMetadataAttribute
    {
        public string Text { get; set; }

        public BooleanTextButtonAttribute(string text)
        {
            Text = text;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            modelMetadata.DataTypeName = "BooleanTextButton";
            modelMetadata.AdditionalValues["BooleanText"] = Text;
        }
    }
}