using DND.Common.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Validation
{
    public static class CustomObjectValidatorExtension
    {
        public static IServiceCollection AddCustomObjectValidator(this IServiceCollection services)
        {
            services.RemoveAll<IObjectModelValidator>();
            services.AddSingleton<IObjectModelValidator>(s =>
            {
                var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
                var metadataProvider = s.GetRequiredService<ICustomModelMetadataProviderSingleton>();
                return new CustomObjectValidator(metadataProvider, options.ModelValidatorProviders);
            });

            return services;
        }
    
    }
}
