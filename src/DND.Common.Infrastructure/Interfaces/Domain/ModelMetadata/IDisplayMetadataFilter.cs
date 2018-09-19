using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.Infrastructure.Interfaces.Domain.ModelMetadata
{
    public interface IDisplayMetadataFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
