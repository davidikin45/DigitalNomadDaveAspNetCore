using DND.Common.DependencyInjection.Autofac;
using DND.Common.Extensions;
using DND.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DND.IntegrationTests2
{
    public class DNDWebApplicationShould
    {

        private const string AntiForgeryFieldName = "__AFTField";
        private const string AntiForgeryCookieName = "AFTCookie";

        private static string ContentPath
        {
            get
            {
                var path = PlatformServices.Default.Application.ApplicationBasePath;
                var contentPath = Path.GetFullPath(Path.Combine(path, $@"..\..\..\..\src\DND.Web"));
                return contentPath;
            }
        }

        [Theory]
        [InlineData("")]
        //[InlineData("/blog")]
        //[InlineData("/gallery")]
        //[InlineData("/videos")]
        //[InlineData("/bucket-list")]
        //[InlineData("/travel-map")]
        //[InlineData("/about")]
        //[InlineData("/work-with-me")]
        //[InlineData("/contact")]
        public async Task RenderHomePage(string path)
        {
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

               services.AddAntiforgery(options => {
                   options.CookieName = AntiForgeryCookieName;
                   options.FormFieldName = AntiForgeryCookieName;
               });

               //Test Server Fix
               //https://github.com/aspnet/Hosting/issues/954
               //https://github.com/Microsoft/vstest/issues/428
               var assembly = typeof(Startup).GetTypeInfo().Assembly;
               services.ConfigureRazorViewEngineForTestServer(assembly, "v4.7.2");
           });

            var testServer = new TestServer(builder);

            var client = testServer.CreateClient();

            var response = await client.GetAsync(path);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            testServer.Dispose();

            Assert.True(true);
        }
    }
}
