using DND.Common.DependencyInjection.Autofac;
using DND.Common.Infrastructure.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace DND.Common
{
    public abstract class ProgramBase<TStartup> where TStartup : class
    {
        private static readonly Dictionary<string, string> defaults = new Dictionary<string, string> { {
                WebHostDefaults.EnvironmentKey, "Development"
            } };

        public static IConfiguration Configuration;

        public static IConfiguration BuildWebHostConfiguration(string environment, string contentRoot)
        {
            return BuildWebHostConfiguration(new string[] { "environment=" + environment }, contentRoot);
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

            var appSettingsFileName = "appsettings.json";
            var appSettingsEnvironmentFilename = "appsettings." + (configEnvironment[WebHostDefaults.EnvironmentKey] ?? "Production") + ".json";

            Console.WriteLine($"Settings:" + Environment.NewLine + 
                               $"{contentRoot}\\{appSettingsFileName}" + Environment.NewLine + 
                               $"{contentRoot}\\{appSettingsEnvironmentFilename}");

            var config = new ConfigurationBuilder()
           .AddInMemoryCollection(defaults)
           .SetBasePath(contentRoot)
           .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
           .AddJsonFile(appSettingsEnvironmentFilename, optional: true, reloadOnChange: true)
           .AddEnvironmentVariables("ASPNETCORE_");

            if (args != null)
            {
                config.AddCommandLine(args);
            }

            return config.Build();
        }

        public static int RunApp(string[] args)
        {
            Configuration = BuildWebHostConfiguration(args, Directory.GetCurrentDirectory());

            Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(Configuration)
              .Enrich.FromLogContext()
              .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");

                var host = CreateWebHostBuilder(args).Build();

                if(host != null)
                {
                    //Db initialization
                    using (var scope = host.Services.CreateScope())
                    {
                        var services = scope.ServiceProvider;
                        MigrateDatabases(services);
                    }

                    host.Run();

                    return 0;
                }
                else
                {
                    Log.Fatal("Host terminated unexpectedly");
                    return 1;
                }
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

        private static void MigrateDatabases(IServiceProvider services)
        {
            var taskRunner = services.GetRequiredService<TaskRunner>();
            taskRunner.RunTasksOnWebHostStartup();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                .UseSetting("detailedErrors", "true")
                .CaptureStartupErrors(true)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                }
                )
                .UseAutofac()
                .UseConfiguration(Configuration)
                .UseSerilog()
                .UseStartup<TStartup>();

        // Only used by EF Core Tooling if IDesignTimeDbContextFactory is not implemented
        // Generally its not good practice to DB in the MVC Project so best to use IDesignTimeDbContextFactory
        //https://wildermuth.com/2017/07/06/Program-cs-in-ASP-NET-Core-2-0
        // public static IWebHost BuildWebHost(string[] args)
        //{
        // Configuration = BuildWebHostConfiguration(args, Directory.GetCurrentDirectory());
        //return CreateWebHostBuilder(args).Build();
        //}
    }
}