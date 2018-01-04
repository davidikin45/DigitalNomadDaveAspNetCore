using Autofac;
using DND.Domain;
using DND.Domain.Models;
using DND.EFPersistance.Identity;
using DND.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Solution.Base.Controllers.Admin;
using Solution.Base.DependencyInjection.Autofac.Modules;
using Solution.Base.Extensions;
using Solution.Base.Filters;
using Solution.Base.Implementation.Persistance;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Services;
using Solution.Base.Middleware;
using Solution.Base.Routing;
using Solution.Base.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DND.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            Configuration.PopulateStaticConnectionStrings();

            HostingEnvironment = hostingEnvironment;
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddDbContext<ApplicationIdentityDbContext>(ConnectionStrings.GetConnectionString("DefaultConnectionString"));

            services.AddIdentity<ApplicationIdentityDbContext, User, IdentityRole>();

            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewLocator());
            });

            services.AddResponseCaching(options =>
            {
                options.SizeLimit = 300 * 1024 * 1024; //100Mb
                options.MaximumBodySize = 64 * 1024 * 1024 ; //64Mb
            });

            //https://stackoverflow.com/questions/46492736/asp-net-core-2-0-http-response-caching-middleware-nothing-cached
            //Client Side Cache Time
            services.AddHttpCacheHeaders(opt => opt.MaxAge = 600);

            services.AddResponseCompression();

            // Add framework services.
            services.AddMvc(options => {
                options.UseCustomModelBinding();
                options.Filters.Add<OperationCancelledExceptionFilter>();

                //Cache-control: no-cache = store response on client browser but recheck with server each request 
                //Cache-control: no-store = dont store response on client
                options.CacheProfiles.Add("Cache24HourNoParams", new CacheProfile()
                {
                    VaryByHeader = "Accept,Accept-Language,Accept-Encoding",
                    //VaryByQueryKeys = "", Only used for server side caching
                    Duration = 60 * 60 * 24, // 24 hour,
                    Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                });

                options.CacheProfiles.Add("Cache24HourParams", new CacheProfile()
                {
                    VaryByHeader = "Accept,Accept-Language,Accept-Encoding",
                    VaryByQueryKeys = new string[] { "*" }, //Only used for server side caching
                    Duration = 60 * 60 * 24, // 24 hour,
                    Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                });

                //Dashed Routing Convention
                //options.Conventions.Add(new DashedRoutingConvention(defaultControllerName: "Home", defaultActionName: "Index"));
            }).
            AddApplicationPart(typeof(AdminFileManagerController).GetTypeInfo().Assembly).AddControllersAsServices();

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = Configuration.GetValue<string>("Settings:AssemblyPrefix") + " Api", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(location);
                var xmlPath = Path.Combine(directory, "Api.xml");

                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
            });

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add services using your custom container here.
            string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string pluginsPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins\\");
            if (!Directory.Exists(pluginsPath)) Directory.CreateDirectory(pluginsPath);

            var assemblyPrefix = "DND";
            Func<Assembly, Boolean> filterFunc = (a => a.FullName.Contains(assemblyPrefix) || a.FullName.Contains("Solution"));
            Func<string, Boolean> stringFunc = (s => (new FileInfo(s)).Name.Contains(assemblyPrefix) || (new FileInfo(s)).Name.Contains("Solution"));

            builder.RegisterModule(new AutofacConventionsDependencyInjectionModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacTasksModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacConventionsMetadataModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacAutomapperModule() { Filter = filterFunc });

            builder.RegisterType<TaskRunner>().AsSelf().PropertiesAutowired();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, TaskRunner taskRunner)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            
            app.UseRequestTasks();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Configuration.GetValue<string>("Settings:AssemblyPrefix") + " API V1");
            });

            //http://www.talkingdotnet.com/how-to-enable-gzip-compression-in-asp-net-core/
            // General
            //"text/plain",
            // Static files
            //"text/css",
            //"application/javascript",
            // MVC
            //"text/html",
            //"application/xml",
            //"text/xml",
            //"application/json",
            //"text/json",
            app.UseResponseCompression();

            //This is Etags
            app.UseHttpCacheHeaders();

            //In memory cache
            //https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core
            //Unfortunately, the built-in response caching middleware makes this very difficult. 
            //Firstly, the same cache duration is used for both client and server caches. Secondly, currently there is no easy way to invalidate cache entries.
            //app.UseResponseCaching();
            app.UseResponseCachingCustom(); //Allows Invalidation

            app.MapWhen(
               context => context.Request.Path.ToString().StartsWith("/uploads"),
               appBranch => {
                   // ... optionally add more middleware to this branch
                   char[] seperator = { ',' };
                   List<string> publicUploadFolders = Configuration.GetValue<string>("Settings:PublicUploadFolders").Split(seperator).ToList();
                   appBranch.UseContentHandler(publicUploadFolders);
               });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            StaticProperties.ModelMetadataProvider = serviceProvider.GetService<IModelMetadataProvider>();
            StaticProperties.HostingEnvironment = HostingEnvironment;
            StaticProperties.Configuration = Configuration;
            StaticProperties.HttpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

            taskRunner.RunTasksAtStartup();
        }
    }
}
