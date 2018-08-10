using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.Interfaces
{
    public interface IDisplayMetadataFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
