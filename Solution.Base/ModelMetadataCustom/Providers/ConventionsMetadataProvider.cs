using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Solution.Base.ModelMetadataCustom.Interfaces;
using System;

namespace Solution.Base.ModelMetadataCustom.Providers
{
    public class ConventionsMetadataProvider : IDisplayMetadataProvider
    {
        public ConventionsMetadataProvider() { }

        private readonly IMetadataFilter[] _metadataFilters;

        public ConventionsMetadataProvider(
            IMetadataFilter[] metadataFilters)
        {
            _metadataFilters = metadataFilters;
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            Array.ForEach(_metadataFilters, m => m.TransformMetadata(context));
        }
    }
}
