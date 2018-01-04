using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Solution.Base.ModelMetadataCustom.Interfaces
{
    public interface IMetadataFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
