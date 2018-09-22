using DND.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DND.Web.FlightSearch
{
    public class Startup : AppStartup
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(loggerFactory, configuration, hostingEnvironment)
        {

        }

        public override void AddDatabases(IServiceCollection services, string defaultConnectionString)
        {
          
        }
    }
}
