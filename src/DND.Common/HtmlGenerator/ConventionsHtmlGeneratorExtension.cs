using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.HtmlGenerator
{
    public static class ConventionsHtmlGeneratorExtension
    {
        public static IServiceCollection AddConventionsHtmlGenerator(this IServiceCollection services)
        {
            services.RemoveAll<IHtmlGenerator>();
            return services.AddSingleton<IHtmlGenerator, ConventionsHtmlGenerator>();
        }
    }
}
