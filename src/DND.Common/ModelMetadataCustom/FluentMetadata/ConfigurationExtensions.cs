using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DND.Common.ModelMetadataCustom.FluentMetadata
{
    public static class ConfigurationExtensions
    {
        public static IMvcBuilder UseFluentMetadata(this IMvcBuilder builder)
        {
            //builder.Services.AddSingleton<IMetadataConfiguratorProviderSingleton, MetadataConfiguratorProviderSingleton>();
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, MvcOptionsSetup>();
            return builder;
        }
    }
}
