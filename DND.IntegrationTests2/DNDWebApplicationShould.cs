using System;
using System.Threading.Tasks;
using DND.Common.DependencyInjection.Autofac;
using DND.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using DND.Common.Extensions;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace DND.IntegrationTests2
{
    [DeploymentItem("Microsoft.VisualStudio.TestPlatform.ObjectModel.dll")]
    public class DNDWebApplicationShould
    {
        private static string ContentPath
        {
            get
            {
                var path = PlatformServices.Default.Application.ApplicationBasePath;
                var contentPath = Path.GetFullPath(Path.Combine(path, $@"..\..\..\DND.Web"));
                return contentPath;
            }
        }

        [Fact]
        public async Task RenderHomePage()
        {

            //Todo: This is not working

            var builder = new WebHostBuilder();
            
            builder.UseContentRoot(ContentPath)
           .UseEnvironment("Development")
           .ConfigureLogging(factory =>
           {
               factory.AddConsole();
           })
           .UseAutofac()
           .UseConfiguration(Program.BuildWebHostConfiguration(null, builder.GetSetting(WebHostDefaults.ContentRootKey)))
           .UseStartup<Startup>()
           .ConfigureServices(services =>
           {
               //Test Server Fix
               //https://github.com/aspnet/Hosting/issues/954
               var assembly = typeof(Startup).GetTypeInfo().Assembly;
               services.ConfigureRazorViewEngineForTestServer(assembly);

               //https://github.com/Microsoft/vstest/issues/428
           });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
