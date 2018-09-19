using DND.Common.Infrastructure.Interfaces.Domain.ModelMetadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Common.ModelMetadataCustom.Providers
{
    public class ConventionsMetadataProvider : IDisplayMetadataProvider, IValidationMetadataProvider
    {
        public ConventionsMetadataProvider() { }

        private readonly IDisplayMetadataFilter[] _metadataFilters;

        public ConventionsMetadataProvider(
            IDisplayMetadataFilter[] metadataFilters)
        {
            _metadataFilters = metadataFilters;
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            Array.ForEach(_metadataFilters, m => m.TransformMetadata(context));
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
           
        }
    }
}
