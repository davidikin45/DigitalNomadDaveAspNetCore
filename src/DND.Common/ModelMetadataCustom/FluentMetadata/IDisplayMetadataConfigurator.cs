using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.FluentMetadata
{
    public interface IDisplayMetadataConfigurator
    {
        void Configure(DisplayMetadata metadata);
    }
}