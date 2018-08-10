using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.ModelMetadataCustom.Providers
{
    public static class CustomModelMetadataProviderExtension
    {
        public static IServiceCollection AddCustomModelMetadataProvider(this IServiceCollection services)
        {
            services.RemoveAll<IModelMetadataProvider>();
            return services.AddSingleton<IModelMetadataProvider, CustomModelMetadataProviderSingleton>();
        }
    }
}
