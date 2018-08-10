using DND.Common.ModelMetadataCustom.FluentMetadata;
using DND.Common.ModelMetadataCustom.Interfaces;
using DND.Common.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DND.Web
{
    //https://andrewlock.net/accessing-services-when-configuring-mvcoptions-in-asp-net-core/
    public class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly IDisplayMetadataFilter[] _metadataFilters;
        private readonly IMetadataConfiguratorProviderSingleton _provider;

        public ConfigureMvcOptions(IDisplayMetadataFilter[] metadataFilters, IMetadataConfiguratorProviderSingleton provider)
        {
            _metadataFilters = metadataFilters;
            _provider = provider;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new FluentMetadataProvider(_provider));
            options.ModelMetadataDetailsProviders.Add(new AttributeMetadataProvider());
            options.ModelMetadataDetailsProviders.Add(new ConventionsMetadataProvider(_metadataFilters));
        }
    }
}
