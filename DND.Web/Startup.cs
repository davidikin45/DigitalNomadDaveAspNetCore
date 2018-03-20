using AspNetCoreRateLimit;
using Autofac;
using DND.Common.Alerts;
using DND.Common.Controllers.Admin;
using DND.Common.DependencyInjection.Autofac.Modules;
using DND.Common.DomainEvents;
using DND.Common.Extensions;
using DND.Common.Filters;
using DND.Common.Implementation.Persistance;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Services;
using DND.Common.Middleware;
using DND.Common.Tasks;
using DND.Domain.Models;
using DND.EFPersistance.Identity;
using DND.Web.Implementation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static DND.Common.Helpers.NavigationMenuHelperExtension;

namespace DND.Web
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
            bool useSQLite = bool.Parse(ConnectionStrings.GetConnectionString("UseSQLite"));

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            if(useSQLite)
            {
                services.AddDbContextSqlite<ApplicationIdentityDbContext>(ConnectionStrings.GetConnectionString("SQLite"));
            }
            else
            {
                services.AddDbContextSqlServer<ApplicationIdentityDbContext>(ConnectionStrings.GetConnectionString("DefaultConnectionString"));
            }

            services.AddIdentity<ApplicationIdentityDbContext, User, IdentityRole>();

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    }
                );

            services.AddTransient<IEmailSender, EmailSender>();

            string implementationFolder = Configuration.GetValue<string>("Settings:ImplementationFolder");
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewLocator(implementationFolder));
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

            if (useSQLite)
            {
                services.AddHangfireSqlite(ConnectionStrings.GetConnectionString("SQLite"));
            }
            else
            {
                services.AddHangfireSqlServer(ConnectionStrings.GetConnectionString("DefaultConnectionString"));
            }

            // Add framework services.
            services.AddMvc(options => {
                options.UseCustomModelBinding();

                //DbGeography causes infinite validation loop
                //https://github.com/aspnet/Home/issues/2024
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(DbGeography)));

                options.Filters.Add<ExceptionHandlingFilter>();
                options.Filters.Add<OperationCancelledExceptionFilter>();

                //Cache-control: no-cache = store response on client browser but recheck with server each request 
                //Cache-control: no-store = dont store response on client
                options.CacheProfiles.Add("Cache24HourNoParams", new CacheProfile()
                {
                    VaryByHeader = "Accept,Accept-Language,Accept-Encoding",
                    //VaryByQueryKeys = "", Only used for server side caching
                    Duration = 60 * 60 * 24, // 24 hour,
                    Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                    NoStore = false
                });

                options.CacheProfiles.Add("Cache24HourParams", new CacheProfile()
                {
                    VaryByHeader = "Accept,Accept-Language,Accept-Encoding",
                    VaryByQueryKeys = new string[] { "*" }, //Only used for server side caching
                    Duration = 60 * 60 * 24, // 24 hour,
                    Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                    NoStore = false
                });

                //Prevents returning object representation in default format when request format isn't available
                options.ReturnHttpNotAcceptable = true;

                options.FormatterMappings.SetMediaTypeMappingForFormat(
                                           "xml", "application/xml");
                //Allow null 200 responses
                //options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();

                //XML
                //options.InputFormatters.Add(new XmlSerializerInputFormatter());
                //options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                //Dashed Routing Convention
                //options.Conventions.Add(new DashedRoutingConvention(defaultControllerName: "Home", defaultActionName: "Index"));
            })
            .AddXmlSerializerFormatters() //XML Opt out. Contract Serializer is Opt in
            .AddApplicationPart(typeof(AdminFileManagerController).GetTypeInfo().Assembly).AddControllersAsServices()
            .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                //Header then QueryString is consistent with how Accept header/FormatFilter works.
                option.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"), new QueryStringApiVersionReader("api-version"));
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;
            });

            //API rate limiting
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>((options)=> {
                options.GeneralRules = new List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 3,
                        Period = "5m"
                    },
                     new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 2,
                        Period = "10s"
                    }
                };
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            //Url Helper for creating API resource links
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            //CORS
            //https://docs.microsoft.com/en-us/aspnet/core/security/cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            //Used for returning only certain fields in API
            services.AddTransient<ITypeHelperService, ITypeHelperService>();

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = Configuration.GetValue<string>("Settings:AssemblyPrefix") + " API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                // Set the comments path for the Swagger JSON and UI.
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(location);
                var xmlPath = Path.Combine(directory, "Api.xml");
              
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();

                c.DescribeAllParametersInCamelCase();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add services using your custom container here.
            string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string pluginsPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins\\");
            if (!Directory.Exists(pluginsPath)) Directory.CreateDirectory(pluginsPath);

            var assemblyPrefix = "DND";
            Func<Assembly, Boolean> filterFunc = (a => a.FullName.Contains(assemblyPrefix) || a.FullName.Contains("DND.Common"));
            Func<string, Boolean> stringFunc = (s => (new FileInfo(s)).Name.Contains(assemblyPrefix) || (new FileInfo(s)).Name.Contains("DND.Common"));

            builder.RegisterModule(new AutofacConventionsDependencyInjectionModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacTasksModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacDomainEventHandlerModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
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
                // Non Api
                app.UseWhen(context => !context.Request.Path.ToString().StartsWith("/api"),
                     appBranch =>
                     {
                         appBranch.UseExceptionHandler("/Error");
                     }
                );


                // Web Api
                app.UseWhen(context => context.Request.Path.ToString().StartsWith("/api"),
                    appBranch =>
                    {
                        appBranch.UseExceptionHandler(appBuilder =>
                        {
                            appBuilder.Run(async context =>
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync(Messages.UnknownError);
                            });
                        });
                    }
               );
            }
            
            app.UseRequestTasks();
     
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Configuration.GetValue<string>("Settings:AssemblyPrefix") + " API V1");
                c.DocExpansion(DocExpansion.None);
            });


            //Use Response Compression Middleware when you're:
            //Unable to use the following server-based compression technologies:
            //IIS Dynamic Compression module
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
            //app.UseResponseCompression();

            //API rate limiting
            //app.UseIpRateLimiting();

            app.UseCors("AllowAnyOrigin");


            //Unsure about ETag and Response caching order. The pluralsight video restful api building has them the other way around.
            //This is Etags *
            app.UseHttpCacheHeaders();

            //In memory cache
            //https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core
            //Unfortunately, the built-in response caching middleware makes this very difficult. 
            //Firstly, the same cache duration is used for both client and server caches. Secondly, currently there is no easy way to invalidate cache entries.
            //app.UseResponseCaching();
            //Request Header Cache-Control: max-age:0 or no-cache will bypass Response Caching. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
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

            app.UseHangfire();

            app.UseMvc(routes =>
            {
                //Angular templates
                //First matches url pattern and then substitutes in missing values. Always need Controller and Action
                routes.MapRoute(
                    name: "Templates",
                    template: "{feature}/Template/{name}",
                    defaults: new { controller = "Template", action = "Render" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            StaticProperties.ModelMetadataProvider = serviceProvider.GetService<IModelMetadataProvider>();
            StaticProperties.HostingEnvironment = HostingEnvironment;
            StaticProperties.Configuration = Configuration;
            StaticProperties.HttpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            string implementationFolder = Configuration.GetValue<string>("Settings:ImplementationFolder");
            NavigationMenuHelper.ImplementationFolder = implementationFolder;

            DomainEvents.Init();

            taskRunner.RunTasksAtStartup();
        }
    }
}
