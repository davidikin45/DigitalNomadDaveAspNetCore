using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using DND.Common.ModelMetadataCustom.Interfaces;

namespace DND.Common.ModelMetadataCustom.Providers
{
    public class AttributeMetadataProvider : IDisplayMetadataProvider
    {
        public AttributeMetadataProvider() { }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.PropertyAttributes != null)
            {
                foreach (object propAttr in context.PropertyAttributes)
                {
                    if(propAttr is IMetadataAttribute)
                    {
                        ((IMetadataAttribute)propAttr).TransformMetadata(context);
                    }
                }
            }
        }
    }
}
