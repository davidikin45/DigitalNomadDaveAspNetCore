using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Solution.Base.ModelMetadataCustom.Interfaces
{
    public interface IMetadataAttribute
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
