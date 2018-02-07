using DND.Domain.Models;
using DND.EFPersistance.Identity;
using DND.EFPersistance.Identity.Initializers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Solution.Base.DependencyInjection.Autofac;
using System;

namespace DND.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            //Db initialization
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationIdentityDbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    var initializer = new ApplicationIdentityDbContextInitializer(context, userManager, roleManager);
                    initializer.Initialize();
                }
                catch (Exception ex)
                {
                    string test = "";
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting("detailedErrors", "true")
                .CaptureStartupErrors(true)
                .UseKestrel(c => c.AddServerHeader = false)
                .UseAutofac()
                .UseStartup<Startup>()
                .Build();
    }
}
