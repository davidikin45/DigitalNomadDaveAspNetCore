using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.Interfaces
{
    public interface IMetadataAttribute
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
