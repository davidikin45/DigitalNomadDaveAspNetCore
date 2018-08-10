using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.Interfaces
{
    public interface IDisplayMetadataAttribute
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
