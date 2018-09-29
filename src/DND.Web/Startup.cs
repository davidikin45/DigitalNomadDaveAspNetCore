
using DND.Common;
using DND.Data.Identity;
using DND.Domain.Identity.Users;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.FlightSearch.ApplicationServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DND.Web
{
    public class Startup : AppStartupWithIdentity<IdentityContext,User>
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(loggerFactory, configuration, hostingEnvironment)
        {

            var types = new Type[] {
                typeof(IBlogPostApplicationService),
                typeof(ICarouselItemApplicationService),
                typeof(IFormApplicationService),
                typeof(IFlightSearchApplicationService)};
        }

        public override void ConfigureHttpClients(IServiceCollection services)
        {
            
        }
    }
}
