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
using DND.Common.Swagger;
using DND.Common.Tasks;
using DND.Domain.Models;
using DND.EFPersistance.Identity;
using DND.Web.MVCImplementation.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.IO;
using System.IO.Compression;
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
            //Settings
            bool enableMVCModelValidation = Configuration.GetValue<bool>("Settings:Switches:EnableMVCModelValidation");
            bool useSQLite = bool.Parse(ConnectionStrings.GetConnectionString("UseSQLite"));
            string cookieAuthName = Configuration.GetValue<string>("Settings:CookieAuthName");
            string cookieTempDataName = Configuration.GetValue<string>("Settings:CookieTempDataName");
            string mvcImplementationFolder = Configuration.GetValue<string>("Settings:MVCImplementationFolder");
            string assemblyPrefix = Configuration.GetValue<string>("Settings:AssemblyPrefix");
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string xmlDocumentationFileName = assemblyName + ".xml";
            int responseCacheSizeMB = Configuration.GetValue<int>("Settings:ResponseCacheSizeMB");

            string bearerTokenIssuer = Configuration["Tokens:Issuer"];
            string bearerTokenAudience = Configuration["Tokens:Audience"];
            string bearerTokenKey = Configuration["Tokens:Key"];

            string SQLiteConnectionString = ConnectionStrings.GetConnectionString("SQLite");
            string SQLServerConnectionString = ConnectionStrings.GetConnectionString("DefaultConnectionString");

            //password
            bool requireDigit = Configuration.GetValue<bool>("Settings:Password:RequireDigit");
            int requiredLength = Configuration.GetValue<int>("Settings:Password:RequiredLength");
            int requiredUniqueChars = Configuration.GetValue<int>("Settings:Password:RequiredUniqueChars");
            bool requireLowercase = Configuration.GetValue<bool>("Settings:Password:RequireLowercase");
            bool requireNonAlphanumeric = Configuration.GetValue<bool>("Settings:Password:RequireNonAlphanumeric");
            bool requireUppercase = Configuration.GetValue<bool>("Settings:Password:RequireUppercase");

            //user
            bool requireConfirmedEmail = Configuration.GetValue<bool>("Settings:User:RequireConfirmedEmail");
            int registrationEmailConfirmationExprireDays = Configuration.GetValue<int>("Settings:User:RegistrationEmailConfirmationExprireDays");
            int forgotPasswordEmailConfirmationExpireHours = Configuration.GetValue<int>("Settings:User:ForgotPasswordEmailConfirmationExpireHours");
            int userDetailsChangeLogoutMinutes = Configuration.GetValue<int>("Settings:User:UserDetailsChangeLogoutMinutes");

            //External Logins
            bool enableGoogleLogin = Configuration.GetValue<bool>("Settings:Login:Google:Enable");
            string googleClientId = Configuration.GetValue<string>("Settings:Login:Google:ClientId");
            string googleClientSecret = Configuration.GetValue<string>("Settings:Login:Google:ClientSecret");

            bool enableFacebookLogin = Configuration.GetValue<bool>("Settings:Login:Facebook:Enable");
            string facebookClientId = Configuration.GetValue<string>("Settings:Login:Facebook:ClientId");
            string facebookClientSecret = Configuration.GetValue<string>("Settings:Login:Facebook:ClientSecret");

            var bin = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);


            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            if (useSQLite)
            {
                services.AddDbContextSqlite<ApplicationIdentityDbContext>(SQLiteConnectionString);
            }
            else
            {
                services.AddDbContextSqlServer<ApplicationIdentityDbContext>(SQLServerConnectionString);
            }

            if (useSQLite)
            {
                services.AddHangfireSqlite(SQLiteConnectionString);
            }
            else
            {
                services.AddHangfireSqlServer(SQLServerConnectionString);
            }

            //Adds "Identity.Application"/IdentityConstants.ApplicationScheme cookie authentication scheme 
            services.AddIdentity<ApplicationIdentityDbContext, User, IdentityRole>(requireDigit, requiredLength, requiredUniqueChars, requireLowercase, requireNonAlphanumeric,
                requireUppercase, requireConfirmedEmail, registrationEmailConfirmationExprireDays, forgotPasswordEmailConfirmationExpireHours, userDetailsChangeLogoutMinutes);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.Cookie.Name = cookieAuthName;
            });

            var authenticationBuilder = services.AddAuthentication()
                 //.AddCookie(options => {
                 //    options.CookieName = cookieAuthName;
                 //    options.LoginPath = "/account/Login";
                 //})
                 .AddJwtBearer(cfg =>
                     cfg.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidIssuer = bearerTokenIssuer,
                         ValidAudience = bearerTokenAudience,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearerTokenKey))
                     }
                 );

            if (enableGoogleLogin)
            {
                authenticationBuilder.AddGoogle("Google", options =>
                {
                    options.ClientId = "clientId";
                    options.ClientSecret = "clientSecret";
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }

            if (enableFacebookLogin)
            {
                authenticationBuilder.AddFacebook("Facebook", options =>
                {
                    options.ClientId = "clientId";
                    options.ClientSecret = "clientSecret";
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }

            //Add this to controller or action using Authorize(Policy = "UserMustBeAdmin")
            //Can create custom requirements by implementing IAuthorizationRequirement and AuthorizationHandler (Needs to be added to services as scoped)
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserMustBeAdmin", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    //policyBuilder.AddRequirements();
                });
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewLocator(mvcImplementationFolder));
            });

            services.AddResponseCaching(options =>
            {
                options.SizeLimit = responseCacheSizeMB * 1024 * 1024; //100Mb
                options.MaximumBodySize = 64 * 1024 * 1024; //64Mb
            });

            //https://stackoverflow.com/questions/46492736/asp-net-core-2-0-http-response-caching-middleware-nothing-cached
            //Client Side Cache Time
            services.AddHttpCacheHeaders(opt => opt.MaxAge = 600);

            //Compression
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { ""});
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            // Add framework services.
            services.AddMvc(options =>
            {
                options.UseCustomModelBinding();

                //DbGeography causes infinite validation loop
                //https://github.com/aspnet/Home/issues/2024
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(DbGeography)));

                options.Filters.Add<ExceptionHandlingFilter>();
                options.Filters.Add<OperationCancelledExceptionFilter>();


                //Accept = Response MIME type client is able to understand.
                //Accept-Language = Response Language client is able to understand.
                //Accept-Encoding = Response Compression client is able to understand.

                //Cache-control: no-cache = store response on client browser but recheck with server each request 
                //Cache-control: no-store = dont store response on client
                options.CacheProfiles.Add("Cache24HourNoParams", new CacheProfile()
                {
                    VaryByHeader = "Accept,Accept-Language,X-Requested-With",
                    //VaryByQueryKeys = "", Only used for server side caching
                    Duration = 60 * 60 * 24, // 24 hour,
                    Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                    NoStore = false
                });

                options.CacheProfiles.Add("Cache24HourParams", new CacheProfile()
                {
                    //IIS DynamicCompressionModule and StaticCompressionModule add the Accept-Encoding Vary header.
                    VaryByHeader = "Accept,Accept-Language,X-Requested-With",
                    VaryByQueryKeys = new string[] { "*" }, //Only used for server side caching
                    Duration = 60 * 60 * 24, // 24 hour,
                    Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                    NoStore = false
                });

                //Prevents returning object representation in default format when request format isn't available
                options.ReturnHttpNotAcceptable = true;


                //Variable resource representations
                //Use RequestHeaderMatchesMediaTypeAttribute to route for Accept header. Version media types not URI!
                var jsonOutputFormatter = options.OutputFormatters
                   .OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    // jsonOutputFormatter.SupportedMediaTypes
                    //.Add("application/vnd.dnd.tour.v1+json");
                    // jsonOutputFormatter.SupportedMediaTypes
                    //.Add("application/vnd.dnd.tourwithestimatedprofits.v1+json");
                }

                var jsonInputFormatter = options.InputFormatters
                   .OfType<JsonInputFormatter>().FirstOrDefault();
                if (jsonInputFormatter != null)
                {
                    // jsonInputFormatter.SupportedMediaTypes
                    //.Add("application/vnd.dnd.tourforcreation.v1+json");
                    // jsonInputFormatter.SupportedMediaTypes
                    //.Add("application/vnd.dnd.tourwithmanagerforcreation.v1+json");
                }

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
            .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            .AddCookieTempDataProvider(options =>
            {
                // new API
                options.Cookie.Name = cookieTempDataName;
            });

            //Disable IObjectValidatable and Validation Attributes from being evaluated and populating modelstate
            //https://stackoverflow.com/questions/46374994/correct-way-to-disable-model-validation-in-asp-net-core-2-mvc
            if (!enableMVCModelValidation)
            {
                var validator = services.FirstOrDefault(s => s.ServiceType == typeof(IObjectModelValidator));
                if (validator != null)
                {
                    services.Remove(validator);
                    services.Add(new ServiceDescriptor(typeof(IObjectModelValidator), _ => new NonValidatingValidator(), ServiceLifetime.Singleton));
                }
            }

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
            services.Configure<IpRateLimitOptions>((options) =>
            {
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
            //services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = assemblyPrefix + " API", Version = "v1" });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });


                //CookieAuthenticationDefaults.AuthenticationScheme
                c.AddSecurityDefinition(IdentityConstants.ApplicationScheme, new ApiKeyScheme
                {
                    Description = "Cookie Authorization scheme. Example: \"Set-Cookie: Key=Value\"",
                    Name = cookieAuthName,
                    In = "cookie",
                    Type = "apiKey"
                });

                //For the time being will assume ALL methods are authorized by Bearer tokens
                //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    { "Bearer", new string[] { } }
                //});

                //TODO - UpdatePartial causes error
                c.OperationFilter<SwaggerAssignSecurityRequirements>();
                c.SchemaFilter<SwaggerModelExamples>();

                // Set the comments path for the Swagger JSON and UI.
                //var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                //var directory = System.IO.Path.GetDirectoryName(location);

                var xmlPath = Path.Combine(bin, xmlDocumentationFileName);

                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();

                c.DescribeAllParametersInCamelCase();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Settings
            var assemblyPrefix = Configuration.GetValue<string>("Settings:AssemblyPrefix");
            string pluginsFolder = @"plugins\";
            string commonAssembly = "DND.Common";

            // Add services using your custom container here.
            string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string pluginsPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), pluginsFolder);
            if (!Directory.Exists(pluginsPath)) Directory.CreateDirectory(pluginsPath);

            Func<Assembly, Boolean> filterFunc = (a => a.FullName.Contains(assemblyPrefix) || a.FullName.Contains(commonAssembly));
            Func<string, Boolean> stringFunc = (s => (new FileInfo(s)).Name.Contains(assemblyPrefix) || (new FileInfo(s)).Name.Contains(commonAssembly));

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
            //settings
            bool enableRedirectNonWwwToWww = Configuration.GetValue<bool>("Settings:Switches:EnableRedirectNonWwwToWww");
            bool enableHelloWord = Configuration.GetValue<bool>("Settings:Switches:EnableHelloWorld");
            bool enableSwagger = Configuration.GetValue<bool>("Settings:Switches:EnableSwagger");
            bool enableResponseCompression = Configuration.GetValue<bool>("Settings:Switches:EnableResponseCompression");
            bool enableIpRateLimiting = Configuration.GetValue<bool>("Settings:Switches:EnableIpRateLimiting");
            bool enableCors = Configuration.GetValue<bool>("Settings:Switches:EnableCors");
            bool enableResponseCaching = Configuration.GetValue<bool>("Settings:Switches:EnableResponseCaching");
            bool enableETags = Configuration.GetValue<bool>("Settings:Switches:EnableETags");
            bool enableHangfire = Configuration.GetValue<bool>("Settings:Switches:EnableHangfire");

            string publicUploadFoldersString = Configuration.GetValue<string>("Settings:PublicUploadFolders");
            string assemblyPrefix = Configuration.GetValue<string>("Settings:AssemblyPrefix");
            string mvcImplementationFolder = Configuration.GetValue<string>("Settings:MVCImplementationFolder");

            string uploadsFolder = "/uploads";
            string commonAssembly = "DND.Common";

            //cache
            int uploadFilesDays = Configuration.GetValue<int>("Settings:Cache:UploadFilesDays");
            int versionedStaticFilesDays = Configuration.GetValue<int>("Settings:Cache:VersionedStaticFilesDays");
            int nonVersionedStaticFilesDays = Configuration.GetValue<int>("Settings:Cache:NonVersionedStaticFilesDays");


            if(enableRedirectNonWwwToWww)
            {
                var options = new RewriteOptions();
                options.AddRedirectToWww();
                app.UseRewriter(options);
            }

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

            if (enableHelloWord)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }

            app.UseRequestTasks();

            if (enableSwagger)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {

                    c.SwaggerEndpoint("/swagger/v1/swagger.json", assemblyPrefix + " API V1");
                    c.DocExpansion(DocExpansion.None);
                });
            }

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
            if (enableResponseCompression)
            {
                //https://www.softfluent.com/blog/dev/Enabling-gzip-compression-with-ASP-NET-Core
                //Concerning performance, the middleware is about 28% slower than the IIS compression (source). Additionally, IIS or nginx has a threshold for compression to avoid compressing very small files.
                app.UseResponseCompression();
            }

            //API rate limiting
            if (enableIpRateLimiting)
            {
                app.UseIpRateLimiting();
            }

            if (enableCors)
            {
                app.UseCors("AllowAnyOrigin");
            }

            //Cache-Control:max-age=0
            //This is equivalent to clicking Refresh, which means, give me the latest copy unless I already have the latest copy.
            //Cache-Control:no-cache 
            //This is holding Shift while clicking Refresh, which means, just redo everything no matter what.

            //Should only be used for server side HTML cachcing or Read Only API. Doesn't really make sense to use Response Caching for CRUD API. 

            //Will only attempt serve AND store caching if:
            //1. Controller or Action has ResponseCache attribute with Location = ResponseCacheLocation.Any
            //2. Request method is GET OR HEAD
            //3. AND Authorization header is not included

            //Will only attempt to serve from cache if:
            //1. Request header DOES NOT contain Cache-Control: no-cache (HTTP/1.1) AND Pragma: no-cache (HTTP/1.0)
            //2. AND Request header DOES NOT contain Cache-Control: max-age=0. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
            //3. AND Request header If-None-Match != Cached ETag
            //4. AND Request header If-Modified-Since < Cached Last Modified (Time it was stored in cache)

            //Will only attempt to store in cache if:
            //1. Request header DOES NOT contain Cache-Control: no-store
            //2. AND Response header DOES NOT contain Cache-Control: no-store
            //3. AND Response header does not contain Set-Cookie
            //4. AND Response Status is 200

            //When storing
            //1. Stores all headers except Age
            //2. Stores Body
            //3. Stores Length

            //In memory cache
            //https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core
            //Unfortunately, the built-in response caching middleware makes this very difficult. 
            //Firstly, the same cache duration is used for both client and server caches. Secondly, currently there is no easy way to invalidate cache entries.
            //app.UseResponseCaching();
            //Request Header Cache-Control: max-age=0 or no-cache will bypass Response Caching. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
            if (enableResponseCaching)
            {
                app.UseResponseCachingCustom(); //Allows Invalidation
            }

            //Works for: GET, HEAD (efficiency, and saves bandwidth)
            //Works for: PUT, PATCH (Concurrency)s
            //This is Etags
            //Generating ETags is expensive. Putting this after response caching makes sense.
            if (enableETags)
            {
                app.UseHttpCacheHeaders(true, true, true, true);
            }

            app.MapWhen(
               context => context.Request.Path.ToString().StartsWith(uploadsFolder),
               appBranch =>
               {
                   // ... optionally add more middleware to this branch
                   char[] seperator = { ',' };
                   List<string> publicUploadFolders = publicUploadFoldersString.Split(seperator).ToList();
                   appBranch.UseContentHandler(Configuration, publicUploadFolders, uploadFilesDays);
               });

            app.UseDefaultFiles();

            //versioned files can have large cache expiry time
            app.UseVersionedStaticFiles(versionedStaticFilesDays);

            //non versioned files
            app.UseNonVersionedStaticFiles(nonVersionedStaticFilesDays);

            app.UseAuthentication();

            if (enableHangfire)
            {
                app.UseHangfire();
            }

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
            NavigationMenuHelper.MVCImplementationFolder = mvcImplementationFolder;

            Func<Assembly, Boolean> filterFunc = (a => a.FullName.Contains(assemblyPrefix) || a.FullName.Contains(commonAssembly));
            DomainEvents.Init(filterFunc);

            taskRunner.RunTasksAtStartup();
        }
    }
}
