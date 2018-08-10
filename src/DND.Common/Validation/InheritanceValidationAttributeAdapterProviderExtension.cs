using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Validation
{
    public static class InheritanceValidationAttributeAdapterProviderExtension
    {
        public static IServiceCollection AddInheritanceValidationAttributeAdapterProvider(this IServiceCollection services)
        {
            services.RemoveAll<IValidationAttributeAdapterProvider>();
            services.AddSingleton<IValidationAttributeAdapterProvider, InheritanceValidationAttributeAdapterProvider>();

            return services;
        }
    }
}
