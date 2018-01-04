using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Solution.Base.ModelMetadataCustom.Interfaces;

namespace Solution.Base.ModelMetadataCustom.ConventionFilters
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