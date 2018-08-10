using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.ModelMetadataCustom.Providers
{
    public interface ICustomModelMetadataProviderSingleton : IModelMetadataProvider
    {
        IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType, ICustomTypeDescriptor model);
    }
}
