using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.FluentMetadata
{
    public interface IValidationMetadataConfigurator
    {
        void Configure(ValidationMetadata metadata);
    }
}