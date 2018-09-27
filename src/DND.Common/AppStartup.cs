using AspNetCoreRateLimit;
using Autofac;
using DND.Common.ActionResults;
using DND.Common.Alerts;
using DND.Common.Authorization;
using DND.Common.Constraints;
using DND.Common.DependencyInjection.Autofac.Modules;
using DND.Common.DynamicForms;
using DND.Common.Extensions;
using DND.Common.Filters;
using DND.Common.Hangfire;
using DND.Common.Helpers;
using DND.Common.HtmlGenerator;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Infrastructure.Tasks;
using DND.Common.Middleware;
using DND.Common.ModelMetadataCustom.Providers;
using DND.Common.Reflection;
using DND.Common.Routing;
using DND.Common.SignalRHubs;
using DND.Common.Validation;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
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
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace DND.Common
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

    public abstract class AppStartup
    {
        public AppStartup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            DontDelete();

           Logger = loggerFactory.CreateLogger("Startup");

            Configuration = configuration;
            Configuration.PopulateStaticConnectionStrings();

            HostingEnvironment = hostingEnvironment; 

            BinPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            Console.WriteLine($"Bin Folder: {BinPath}");
            PluginsPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), PluginsFolder);
            Console.WriteLine($"Plugins Folder: {PluginsPath}");
            if (!Directory.Exists(PluginsPath)) Directory.CreateDirectory(PluginsPath);
            AssemblyName = this.GetType().Assembly.GetName().Name;
            AppAssemblyPrefix = configuration.GetValue<string>("AppSettings:AssemblyPrefix");

            AssemblyBoolFilter = (a => a.FullName.Contains(AppAssemblyPrefix) || a.FullName.Contains(CommonAssemblyPrefix));
            AssemblyStringFilter = (s => (new FileInfo(s)).Name.Contains(AppAssemblyPrefix) || (new FileInfo(s)).Name.Contains(CommonAssemblyPrefix));

            //Load Assemblies into current AppDomain
            var binAssemblies = Directory.EnumerateFiles(BinPath,"*.*", SearchOption.TopDirectoryOnly)
              .Where(file => new[] { ".dll", ".exe" }.Any(file.ToLower().EndsWith))
              .Where(AssemblyStringFilter)
              .Select(Assembly.LoadFrom).ToList();

            //Load plugins into current AppDomain
            var pluginAssemblies = Directory.EnumerateFiles(PluginsPath, "*.*", SearchOption.TopDirectoryOnly)
                               .Where(file => new[] { ".dll", ".exe" }.Any(file.ToLower().EndsWith))
                               .Select(Assembly.LoadFrom).ToList();

            ApplicationParts = binAssemblies.Concat(pluginAssemblies).Where(AssemblyBoolFilter).ToList();
        }

        public void DontDelete()
        {
            var types = new Type[] {
                typeof(Serilog.ILogger),
                typeof(Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme),
                typeof(Serilog.Sinks.RollingFile.RollingFileSink),
                };

            Action<LoggerSinkConfiguration> action = (LoggerSinkConfiguration) =>
            {
                LoggerSinkConfiguration.Debug();
            };
        }

        public Microsoft.Extensions.Logging.ILogger Logger { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public string BinPath { get; }
        public string PluginsPath { get; }
        public string AssemblyName { get; }
        public string CommonAssemblyPrefix { get; } = "DND.Common";
        public string PluginsFolder { get; } = @"plugins\";
        public string AppAssemblyPrefix { get; }
        public Func<Assembly, Boolean> AssemblyBoolFilter { get; }
        public Func<string, Boolean> AssemblyStringFilter { get; }
        public List<Assembly> ApplicationParts { get; }
        public string UploadsFolder { get; } = "/uploads";

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            var tokenSettingsSection = Configuration.GetSection("TokenSettings");
            services.Configure<TokenSettings>(tokenSettingsSection);
            var tokenSettings = tokenSettingsSection.Get<TokenSettings>();

            var displayConventionsDisableSettingsSection = Configuration.GetSection("DisplayConventionsDisableSettings");
            services.Configure<DisplayConventionsDisableSettings>(displayConventionsDisableSettingsSection);
            var displayConventionsDisableSettings = displayConventionsDisableSettingsSection.Get<DisplayConventionsDisableSettings>();

            var CORSSettingsSection = Configuration.GetSection("CORSSettings");
            services.Configure<CORSSettings>(CORSSettingsSection);
            var corsSettings = CORSSettingsSection.Get<CORSSettings>();

            var passwordSettingsSection = Configuration.GetSection("PasswordSettings");
            services.Configure<PasswordSettings>(passwordSettingsSection);
            var passwordSettings = passwordSettingsSection.Get<PasswordSettings>();

            var userSettingsSection = Configuration.GetSection("UserSettings");
            services.Configure<UserSettings>(userSettingsSection);
            var userSettings = userSettingsSection.Get<UserSettings>();

            var authenticationSettingsSection = Configuration.GetSection("AuthenticationSettings");
            services.Configure<AuthenticationSettings>(authenticationSettingsSection);
            var authenticationSettings = authenticationSettingsSection.Get<AuthenticationSettings>();

            var authorizationSettingsSection = Configuration.GetSection("AuthorizationSettings");
            services.Configure<AuthorizationSettings>(authorizationSettingsSection);
            var authorizationSettings = authorizationSettingsSection.Get<AuthorizationSettings>();

            var cacheSettingsSection = Configuration.GetSection("CacheSettings");
            services.Configure<CacheSettings>(cacheSettingsSection);
            var cacheSettings = cacheSettingsSection.Get<CacheSettings>();

            var emailSettingsSection = Configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettingsSection);
            var emailSettings = emailSettingsSection.Get<EmailSettings>();

            services.Configure<EmailSettings>(options =>
            {
                if (!options.FileSystemFolder.Contains(@":\"))
                {
                    options.FileSystemFolder = Path.Combine(BinPath, options.FileSystemFolder);
                }
            });

            var emailTemplatesSection = Configuration.GetSection("EmailTemplates");
            services.Configure<EmailTemplates>(emailTemplatesSection);
            var emailTemplates = emailTemplatesSection.Get<EmailTemplates>();
            services.Configure<EmailTemplates>(options =>
            {
                if (!string.IsNullOrEmpty(options.Welcome))
                {
                    options.Welcome = HostingEnvironment.MapContentPath(options.Welcome);
                }

                if (!string.IsNullOrEmpty(options.ResetPassword))
                {
                    options.ResetPassword = HostingEnvironment.MapContentPath(options.ResetPassword);
                }
            });

            var switchSettingsSection = Configuration.GetSection("SwitchSettings");
            services.Configure<SwitchSettings>(switchSettingsSection);
            var switchSettings = switchSettingsSection.Get<SwitchSettings>();

            services.Configure<TokenSettings>(options =>
            {
                if (!string.IsNullOrEmpty(options.PrivateKeyPath))
                {
                    options.PrivateKeyPath = HostingEnvironment.MapContentPath(options.PrivateKeyPath);
                }

                if (!string.IsNullOrEmpty(options.PublicKeyPath))
                {
                    options.PublicKeyPath = HostingEnvironment.MapContentPath(options.PublicKeyPath);
                }

                if (!string.IsNullOrEmpty(options.PrivateCertificatePath))
                {
                    options.PrivateCertificatePath = HostingEnvironment.MapContentPath(options.PrivateCertificatePath);
                }

                if (!string.IsNullOrEmpty(options.PublicCertificatePath))
                {
                    options.PublicCertificatePath = HostingEnvironment.MapContentPath(options.PublicCertificatePath);
                }
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            AddDatabases(services, connectionString);

            services.AddHangfireSqlServer(connectionString);

            if (authenticationSettings.Application.Enable)
            {
                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = appSettings.CookieAuthName;
                });
            }

            services.ConfigureExternalCookie(options =>
              {
                  options.Cookie.Name = appSettings.CookieExternalAuthName;
              });

            var authenticationBuilder = services.AddAuthentication();

            if (authenticationSettings.JwtToken.Enable)
            {
                services.AddJwtAuthentication(
               tokenSettings.Key,
               !string.IsNullOrEmpty(tokenSettings.PublicKeyPath) ? HostingEnvironment.MapContentPath(tokenSettings.PublicKeyPath) : tokenSettings.PublicKeyPath,
               !string.IsNullOrEmpty(tokenSettings.PublicCertificatePath) ? HostingEnvironment.MapContentPath(tokenSettings.PublicCertificatePath) : tokenSettings.PublicCertificatePath,
               tokenSettings.ExternalIssuers,
               tokenSettings.LocalIssuer,
               tokenSettings.Audiences);
            }

            if (switchSettings.EnableIFramesGlobal)
            {
                services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            }

            if (authenticationSettings.OpenIdConnectJwtToken.Enable)
            {
                //scheme
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44318/";
                    options.ApiName = "api";
                    options.ApiSecret = "apisecret"; //Only need this if AccessTokenType = AccessTokenType.Reference
                });
            }

            if (authenticationSettings.OpenIdConnect.Enable)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // keep original claim types
                services.Configure<AuthenticationOptions>(options =>
                {
                    //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.1&tabs=aspnetcore2x
                    //overides "Identity.Application"/IdentityConstants.ApplicationScheme set by AddIdentity
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Challenge scheme is how user should login if they arent already logged in.
                });

                //authetication scheme
                authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
                {
                    options.Cookie.Name = appSettings.CookieAuthName;
                    options.AccessDeniedPath = "Authorization/AccessDenied";
                });
                authenticationBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:44318";

                    //options.ResponseType = "code"; //Authorization
                    //options.ResponseType = "id_token"; //Implicit
                    //options.ResponseType = "id_token token"; //Implicit
                    options.ResponseType = "code id_token"; //Hybrid
                    //options.ResponseType = "code token"; //Hybrid
                    //options.ResponseType = "code id_token token"; //Hybrid

                    //options.CallbackPath = new PathString("...")
                    //options.SignedOutCallbackPath = new PathString("...")
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");
                    options.Scope.Add("api");
                    options.Scope.Add("subscriptionlevel");
                    options.Scope.Add("country");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true;

                    options.ClientId = "mvc_client";
                    options.ClientSecret = "secret";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("idp");

                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                    options.ClaimActions.MapUniqueJsonKey("country", "country");

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = "given_name",
                        RoleClaimType = "role"
                    };
                });
            }

            if (authenticationSettings.Google.Enable)
            {
                authenticationBuilder.AddGoogle("Google", options =>
                {
                    options.ClientId = authenticationSettings.Google.ClientId;
                    options.ClientSecret = authenticationSettings.Google.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }

            if (authenticationSettings.Facebook.Enable)
            {
                authenticationBuilder.AddFacebook("Facebook", options =>
                {
                    options.ClientId = authenticationSettings.Facebook.ClientId;
                    options.ClientSecret = authenticationSettings.Facebook.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }

            //Add this to controller or action using Authorize(Policy = "UserMustBeAdmin")
            //Can create custom requirements by implementing IAuthorizationRequirement and AuthorizationHandler (Needs to be added to services as scoped)
            services.AddAuthorization(options =>
            {
                if(authorizationSettings.UserMustBeAuthorizedByDefault)
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                }

                options.AddPolicy("UserMustBeAdmin", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    //policyBuilder.AddRequirements();
                });

                //http://docs.identityserver.io/en/release/topics/apis.html
                options.AddPolicy(ApiScopes.Full, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full);
                });

                options.AddPolicy(ApiScopes.Write, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full, ApiScopes.Write, ApiScopes.Create, ApiScopes.Update);
                });

                options.AddPolicy(ApiScopes.Create, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full, ApiScopes.Write, ApiScopes.Create);
                });

                options.AddPolicy(ApiScopes.Read, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full, ApiScopes.Read);
                });

                options.AddPolicy(ApiScopes.Update, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full, ApiScopes.Write, ApiScopes.Update);
                });

                options.AddPolicy(ApiScopes.Delete, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full, ApiScopes.Write, ApiScopes.Delete);
                });

                options.AddPolicy(ApiScopes.Notifications, policyBuilder =>
                {
                    policyBuilder.RequireScope(ApiScopes.Full, ApiScopes.Notifications);
                });
            });

            services.AddSingleton<IAuthorizationHandler, AnonymousHandler>();
            //Scope name as policy
            //https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

            if (emailSettings.SendEmailsViaSmtp)
            {
                services.AddTransient<IEmailService, EmailServiceSmtp>();
            }
            else
            {
                services.AddTransient<IEmailService, EmailServiceFileSystem>();
            }

            services.Configure<AssemblyProviderOptions>(options =>
            {
                options.BinPath = BinPath;
                options.AssemblyFilter = AssemblyStringFilter;
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("tokenCheck", typeof(TokenConstraint));
                options.ConstraintMap.Add("versionCheck", typeof(RouteVersionConstraint));
            });

            services.AddResponseCaching(options =>
            {
                options.SizeLimit = appSettings.ResponseCacheSizeMB * 1024 * 1024; //100Mb
                options.MaximumBodySize = 64 * 1024 * 1024; //64Mb
            });

            //https://stackoverflow.com/questions/46492736/asp-net-core-2-0-http-response-caching-middleware-nothing-cached
            //Client Side Cache Time
            services.AddHttpCacheHeaders(opt => opt.MaxAge = 600, opt => opt.MustRevalidate = true);

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

            services.AddCookiePolicy(appSettings.CookieConsentName);

            // Add framework services.
            var mvc = services.AddMvc(options =>
            {
                //https://github.com/aspnet/Security/issues/1764
                options.AllowCombiningAuthorizeFilters = false;

                options.UseDbGeographyModelBinding();
                options.UseDynamicFormsModelBinding();

                //DbGeography causes infinite validation loop
                //https://github.com/aspnet/Home/issues/2024
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(DbGeography)));

                options.Filters.Add<ExceptionHandlingFilter>();
                options.Filters.Add<OperationCancelledExceptionFilter>();

                //options.Filters.Add(typeof(ModelValidationFilter));

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

                }

                var jsonInputFormatter = options.InputFormatters
                   .OfType<JsonInputFormatter>().FirstOrDefault();
                if (jsonInputFormatter != null)
                {

                }

                options.FormatterMappings.SetMediaTypeMappingForFormat(
                                           "xml", "application/xml");

            })
            .AddXmlSerializerFormatters() //XML Opt out. Contract Serializer is Opt in
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.Formatting = Formatting.Indented;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddCookieTempDataProvider(options =>
            {
                // new API
                options.Cookie.Name = appSettings.CookieTempDataName;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Automatic Api Validation Response when ApiController attribute is applied.
            //https://blogs.msdn.microsoft.com/webdev/2018/02/27/asp-net-core-2-1-web-apis/
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new UnprocessableEntityAngularObjectResult(Messages.RequestInvalid, context.ModelState);
                };
            });

            //Add Controllers from other assemblies
            foreach (var assembly in ApplicationParts)
            {
                mvc.AddApplicationPart(assembly).AddControllersAsServices();
            }

            //Add Embedded views from other assemblies
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewExpander(appSettings.MvcImplementationFolder));

                //Add Embedded Views from other assemblies
                //Edit and Continue wont work with these views.
                foreach (var assembly in ApplicationParts)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(assembly));
                }
            });

            //Disable IObjectValidatable and Validation Attributes from being evaluated and populating modelstate
            //https://stackoverflow.com/questions/46374994/correct-way-to-disable-model-validation-in-asp-net-core-2-mvc
            if (!switchSettings.EnableMVCModelValidation)
            {
                var validator = services.FirstOrDefault(s => s.ServiceType == typeof(IObjectModelValidator));
                if (validator != null)
                {
                    services.Remove(validator);
                    services.Add(new ServiceDescriptor(typeof(IObjectModelValidator), _ => new NonValidatingValidator(), ServiceLifetime.Singleton));
                }
            }

            services.AddSignalR();

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

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.ConfigureCorsAllowAnyOrigin("AllowAnyOrigin");
            services.ConfigureCorsAllowSpecificOrigin("AllowSpecificOrigin", corsSettings.Domains);

            services.AddCustomModelMetadataProvider();
            services.AddCustomObjectValidator();
            services.AddInheritanceValidationAttributeAdapterProvider();
            services.AddConventionsHtmlGenerator();

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);

            string xmlDocumentationFileName = AssemblyName + ".xml";
            var xmlDocumentationPath = Path.Combine(BinPath, xmlDocumentationFileName);
            services.AddSwagger(appSettings.AssemblyPrefix + " API", "", "", "", "v1", xmlDocumentationPath);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacConventionsDependencyInjectionModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacTasksModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacDomainEventHandlerModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacDbContextFactoryModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacConventionsMetadataModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacConventionsSignalRHubModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacAutomapperModule() { Filter = AssemblyBoolFilter });

            builder.RegisterType<TaskRunner>().AsSelf().PropertiesAutowired();
        }

        private static bool IsStreamRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var stream = false;

            var filename = Path.GetFileName(context.Request.Path.ToString());
            stream = !string.IsNullOrEmpty(filename) && context.Request.Query.Keys.Count() == 0 && filename.IsVideo();

            return stream;
        }

        private static bool AreCookiesConsentedCallback(Microsoft.AspNetCore.Http.HttpContext context, string cookieConsentName)
        {
            return context.Request.Path.ToString().StartsWith("/api") || (context.Request.Cookies.Keys.Contains(cookieConsentName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, IOptions<CacheSettings> cacheSettings,
            IOptions<SwitchSettings> switchSettings, TaskRunner taskRunner, ISignalRHubMapper signalRHubMapper, ILoggerFactory loggerFactory)
        {

            foreach (var publicUploadFolder in appSettings.Value.PublicUploadFolders.Split(','))
            {
                var path = env.WebRootPath + publicUploadFolder;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            if (env.IsDevelopment() || env.EnvironmentName == "Integration")
            {
                app.UsePing("/ping");

                // Non Api
                app.UseWhen(context => !context.Request.Path.ToString().StartsWith("/api"),
                    appBranch =>
                    {
                        app.UseDeveloperExceptionPage();
                    }
               );

                // Web Api
                app.UseWhen(context => context.Request.Path.ToString().StartsWith("/api"),
                    appBranch =>
                    {
                        appBranch.UseWebApiExceptionHandler(true);
                    }
               );

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
                        appBranch.UseWebApiExceptionHandler(false);
                    }
               );

                if (switchSettings.Value.EnableHsts)
                {
                    //Only ever use HSTS in production!!!!!
                    //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                    app.UseHsts();
                }
            }

            if (switchSettings.Value.EnableRedirectHttpToHttps)
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                //picks up port automatically
                app.UseHttpsRedirection();
            }

            if (switchSettings.Value.EnableRedirectNonWwwToWww)
            {
                var options = new RewriteOptions();
                options.AddRedirectToWww();
                //options.AddRedirectToHttps(StatusCodes.Status307TemporaryRedirect); // Does not pick up port automatically
                app.UseRewriter(options);
            }

            if (env.IsDevelopment())
            {
                //1. download profiler from https://stackify.com/prefix/
                //2. enable .NET profiler in windows tray
                //3. access results at http://localhost:2012
                //app.UseStackifyPrefix();
            }

            if (switchSettings.Value.EnableHelloWorld)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }

            app.UseRequestTasks();

            if (switchSettings.Value.EnableSwagger)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {

                    c.SwaggerEndpoint("/swagger/v1/swagger.json", appSettings.Value.AssemblyPrefix + " API V1");
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
            if (switchSettings.Value.EnableResponseCompression)
            {
                //https://www.softfluent.com/blog/dev/Enabling-gzip-compression-with-ASP-NET-Core
                //Concerning performance, the middleware is about 28% slower than the IIS compression (source). Additionally, IIS or nginx has a threshold for compression to avoid compressing very small files.
                app.UseResponseCompression();
            }

            //API rate limiting
            if (switchSettings.Value.EnableIpRateLimiting)
            {
                app.UseIpRateLimiting();
            }

            if (switchSettings.Value.EnableCors)
            {
                if (HostingEnvironment.IsProduction())
                {
                    app.UseCors("AllowSpecificOrigin");
                }
                else
                {
                    app.UseCors("AllowAnyOrigin");
                }
            }

            if (switchSettings.Value.EnableSignalR)
            {
                app.UseSignalR(routes =>
                {
                    routes.MapHub<NotificationHub>(appSettings.Value.SignalRUrlPrefix + "/signalr/notifications");
                    signalRHubMapper.MapHubs(routes, appSettings.Value.SignalRUrlPrefix);
                });
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
            if (switchSettings.Value.EnableResponseCaching)
            {
                if (switchSettings.Value.EnableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, appSettings.Value.CookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseResponseCachingCustom(); //Allows Invalidation
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                       appBranch =>
                       {
                           appBranch.UseResponseCachingCustom(); //Allows Invalidation
                       }
                     );
                }
            }

            //Works for: GET, HEAD (efficiency, and saves bandwidth)
            //Works for: PUT, PATCH (Concurrency)s
            //This is Etags
            //Generating ETags is expensive. Putting this after response caching makes sense.
            if (switchSettings.Value.EnableETags)
            {
                if (switchSettings.Value.EnableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, appSettings.Value.CookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseHttpCacheHeaders();
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                     appBranch =>
                     {
                         appBranch.UseHttpCacheHeaders();
                     }
                   );
                }
            }

            app.MapWhen(
               context => context.Request.Path.ToString().StartsWith(UploadsFolder),
               appBranch =>
               {
                   // ... optionally add more middleware to this branch
                   char[] seperator = { ',' };
                   List<string> publicUploadFolders = appSettings.Value.PublicUploadFolders.Split(seperator).ToList();
                   appBranch.UseContentHandler(Configuration, publicUploadFolders, cacheSettings.Value.UploadFilesDays);
               });

            //Default culture should be set to where the majority of traffic comes from.
            //If the client sends through "en" and the default culture is "en-AU". Instead of falling back to "en" it will fall back to "en-AU".
            var defaultLanguage = appSettings.Value.DefaultCulture.Split('-')[0];

            //Support all formats for numbers, dates, etc.
            var formatCulturesList = new List<string>();

            //Countries
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (!formatCulturesList.Contains(ci.Name))
                {
                    formatCulturesList.Add(ci.Name);
                }
            }

            //Languages
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if (!formatCulturesList.Contains(ci.TwoLetterISOLanguageName) && ci.TwoLetterISOLanguageName != defaultLanguage)
                {
                    formatCulturesList.Add(ci.TwoLetterISOLanguageName);
                }
            }

            var supportedFormatCultures = formatCulturesList.Select(x => new CultureInfo(x)).ToArray();

            var supportedUICultures = new CultureInfo[] {
                new CultureInfo(appSettings.Value.DefaultCulture)
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: appSettings.Value.DefaultCulture, uiCulture: appSettings.Value.DefaultCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedFormatCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedUICultures
            });

            app.UseDefaultFiles();

            //versioned files can have large cache expiry time
            app.UseVersionedStaticFiles(cacheSettings.Value.VersionedStaticFilesDays);

            //non versioned files
            app.UseNonVersionedStaticFiles(cacheSettings.Value.NonVersionedStaticFilesDays);

            app.UseAuthentication();

            if (switchSettings.Value.EnableHangfire)
            {
                // Configure hangfire to use the new JobActivator.
                GlobalConfiguration.Configuration.UseActivator(new HangfireDependencyInjectionActivator(serviceProvider));
                app.UseHangfire();
            }

            if (switchSettings.Value.EnableCookieConsent)
            {
                app.UseCookiePolicy();
            }

            app.UseMvc(routes =>
            {
                routes.AllRoutes("/all-routes");

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
            NavigationMenuHelperExtension.NavigationMenuHelper.MvcImplementationFolder = appSettings.Value.MvcImplementationFolder;

            taskRunner.RunTasksAfterApplicationConfiguration();
        }

        public abstract void AddDatabases(IServiceCollection services, string defaultConnectionString);

    }
}
