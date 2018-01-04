using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Solution.Base.DependencyInjection.Autofac;

namespace DND.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseAutofac()
                .UseStartup<Startup>()
                .Build();
    }
}
