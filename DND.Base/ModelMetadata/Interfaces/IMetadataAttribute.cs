using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.ModelMetadata
{
    public interface IMetadataAttribute
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
