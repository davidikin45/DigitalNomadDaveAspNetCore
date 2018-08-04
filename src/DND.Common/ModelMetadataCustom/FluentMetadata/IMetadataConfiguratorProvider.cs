using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace DND.Common.ModelMetadataCustom.FluentMetadata
{
    public interface IMetadataConfiguratorProviderSingleton
    {
        IEnumerable<IMetadataConfigurator> GetMetadataConfigurators(ModelMetadataIdentity identity);
    }
}