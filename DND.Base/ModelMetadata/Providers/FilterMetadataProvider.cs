using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.ModelMetadata
{
    public class FilterMetadataProvider : IDisplayMetadataProvider
    {
        public FilterMetadataProvider() { }

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
