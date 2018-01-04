using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Solution.Base.Infrastructure
{
    public static class ModelMetadataProvider
    {
        public static Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata GetMetadataForType(Type modelType)
        {
            return StaticProperties.ModelMetadataProvider.GetMetadataForType(modelType);
        }

        public static Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata GetMetadataForProperty(Type containerType, string propertyName)
        {
            return StaticProperties.ModelMetadataProvider.GetMetadataForProperty(containerType, propertyName);
        }
    }
}
