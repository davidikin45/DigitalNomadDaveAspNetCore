using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.Base.ModelMetadata
{
    public interface IMetadataFilter
    {
        void TransformMetadata(DisplayMetadataProviderContext context);
    }
}
