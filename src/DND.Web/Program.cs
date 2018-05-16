using DND.Domain.Models;
using DND.EFPersistance.Identity;
using DND.EFPersistance.Identity.Initializers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using DND.Common.DependencyInjection.Autofac;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace DND.Web
{
    public class Program
    {
        //http://mygeekjourney.com/asp-net-core/integrating-serilog-asp-net-core/
        //https://www.carlrippon.com/asp-net-core-logging-with-serilog-and-sql-server/
        //Logging
        //Trace = 0
        //Debug = 1 -- Developement Standard
        //Information = 2
        //Warning = 3 -- Production Standard
        //Error = 4
        //Critical = 5

        //Default Environment
        private static readonly Dictionary<string, string> defaults = new Dictionary<string, string> { { WebHostDefaults.EnvironmentKey, "Development" } };

        public static IConfiguration Configuration;

        public static IConfiguration BuildWebHostConfiguration(string contentRoot)
        {
            return BuildWebHostConfiguration(null, contentRoot);
        }

        public static IConfiguration BuildWebHostConfiguration(string[] args, string contentRoot)
        {
            //Environment Order ASC
            //1. Command Line (environment=Development)
            //2. Environment Variable (ASPNETCORE_ENVIRONMENT=Development). In Azure ASPNETCORE_ENVIRONMENT can be set in Settings --> Application settings
            //3. Default from Dictionary

            var configEnvironmentBuilder = new ConfigurationBuilder()
                   .AddInMemoryCollection(defaults)
                   .AddEnvironmentVariables("ASPNETCORE_");

            if (args != null)
            {
                configEnvironmentBuilder.AddCommandLine(args);
            }

            var configEnvironment = configEnvironmentBuilder.Build();

            var config = new ConfigurationBuilder()
           .AddInMemoryCollection(defaults)
          .SetBasePath(contentRoot)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{configEnvironment[WebHostDefaults.EnvironmentKey] ?? "Production"}.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables("ASPNETCORE_");

            if (args != null)
            {
                config.AddCommandLine(args);
            }

            return config.Build();
        }

        public static int Main(string[] args)
        {
            Configuration = BuildWebHostConfiguration(args, Directory.GetCurrentDirectory());

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(Configuration)
              .Enrich.FromLogContext()
              .CreateLogger();

            //SQL Logging
            //    {
            //        "Name": "MSSqlServer",
            //"Args": {
            //            "connectionString": "Connection String",
            //  "tableName": "Log",
            //  "autoCreateSqlTable": true
            //}
            //    }

            //Serilog.Debugging.SelfLog.Enable(msg =>
            //{
            //    Debug.Print(msg);
            //    Debugger.Break();
            //});

            try
            {
                Log.Information("Getting the motors running...");

                var host = BuildWebHost(args);

                //Db initialization
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<ApplicationIdentityDbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    var initializer = new ApplicationIdentityDbContextInitializer(context, userManager, roleManager);
                    initializer.Initialize();
                }

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                .UseSetting("detailedErrors", "true")
                .CaptureStartupErrors(true)
                .UseKestrel(c => c.AddServerHeader = false)
                .UseAutofac()
                .UseConfiguration(Configuration)
                .UseSerilog()
                .UseStartup<Startup>()
                .Build();
    }
}
