﻿using DND.Common.DependencyInjection.Autofac;
using DND.Common.Infrastructure.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace DND.Common
{
    public abstract class ProgramBase<TStartup> where TStartup : class
    {
        public static IConfiguration Configuration;

        public static IConfiguration BuildWebHostConfiguration(string environment, string contentRoot)
        {
            return Config.Build(new string[] { "environment=" + environment }, contentRoot);
        }

        public static int RunApp(string[] args)
        {
            Configuration = Config.Build(args, Directory.GetCurrentDirectory());

            Logging.Init(Configuration);

            try
            {
                Log.Information("Getting the motors running...");

                var host = CreateWebHostBuilder(args).Build();

                //Db initialization
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    if(services.GetService<TaskRunner>() != null)
                    {
                        MigrateDatabases(services);
                    }
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

        private static void MigrateDatabases(IServiceProvider services)
        {
            Log.Information("Migrating Databases");

            var taskRunner = services.GetRequiredService<TaskRunner>();
            taskRunner.RunTasksOnWebHostStartup();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                // These two settings allow an error page to be shown rather than throwing exception on startup
                // Need to be careful putting code after IWebHostBuilder.Build()
                .CaptureStartupErrors(true)
                //.UseSetting("detailedErrors", "true") // Better to put this in appsettings
                .UseKestrel((context, options) =>
                {
                    options.AddServerHeader = false;
                }
                )
                .UseAutofac()
                .UseConfiguration(Configuration) ////IWebHostBuilder configuration is added to the app's configuration, but the converse isn't true—ConfigureAppConfiguration doesn't affect the IWebHostBuilder configuration
                .UseSerilog()
                .UseStartup<TStartup>();

        //WebHostBuilder - https://github.com/aspnet/Hosting/blob/3483a3250535da6f291326f3f5f1e3f66ca09901/src/Microsoft.AspNetCore.Hosting/WebHostBuilder.cs
        //WebHost.CreateDefaultBuilder(args) - https://github.com/aspnet/MetaPackages/blob/release/2.1/src/Microsoft.AspNetCore/WebHost.cs
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-2.1

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