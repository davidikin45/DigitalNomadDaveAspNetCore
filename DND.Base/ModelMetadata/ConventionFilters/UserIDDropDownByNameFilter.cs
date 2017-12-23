using DND.Base.ModelMetadata;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Base.ModelMetadata
{
	public class UserIDDropDownByNameFilter : IMetadataFilter
	{
        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;

            if (!string.IsNullOrEmpty(propertyName) &&
             string.IsNullOrEmpty(modelMetadata.DataTypeName) &&
             propertyName.ToLower().Contains("assignedto"))
            {
                modelMetadata.DataTypeName = "UserID";
            }
        }
    }
}