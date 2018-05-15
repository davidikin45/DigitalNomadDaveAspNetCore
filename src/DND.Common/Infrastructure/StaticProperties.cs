using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure
{
    public static class StaticProperties
    {
        public static IModelMetadataProvider ModelMetadataProvider { get; set; }
        public static IHostingEnvironment HostingEnvironment { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static IHttpContextAccessor HttpContextAccessor { get; set; }
    }
}
