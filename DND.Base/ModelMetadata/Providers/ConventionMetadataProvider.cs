using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;

namespace DND.Base.ModelMetadata
{
    public class ConventionMetadataProvider : IDisplayMetadataProvider
    {
        public ConventionMetadataProvider() { }

        private readonly IMetadataFilter[] _metadataFilters;

        public ConventionMetadataProvider(
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
