using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.Interfaces
{
    public interface IMetadataFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
