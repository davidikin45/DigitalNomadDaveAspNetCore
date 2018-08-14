﻿using AspNetCoreRateLimit;
using Autofac;
using DND.Common.Alerts;
using DND.Common.AppSettings;
using DND.Common.Constraints;
using DND.Common.Controllers.Admin;
using DND.Common.Controllers.Api;
using DND.Common.DependencyInjection.Autofac.Modules;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.Filters;
using DND.Common.Hangfire;
using DND.Common.Helpers;
using DND.Common.Helpers.Cryptography;
using DND.Common.HtmlGenerator;
using DND.Common.Infrastructure;
using DND.Common.Middleware;
using DND.Common.ModelMetadataCustom.Providers;
using DND.Common.Reflection;
using DND.Common.Routing;
using DND.Common.SignalRHubs;
using DND.Common.Swagger;
using DND.Common.Tasks;
using DND.Common.Validation;
using DND.Data.Identity;
using DND.Domain.Identity.Users;
using DND.Infrastructure;
using DND.Web.MVCImplementation.DynamicForms.ModelBinders;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

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
            bool EnableIFramesGlobal = Configuration.GetValue<bool>("AppSettings:Switches:EnableIFramesGlobal");
            bool enableMVCModelValidation = Configuration.GetValue<bool>("AppSettings:Switches:EnableMVCModelValidation");
            bool useSQLite = bool.Parse(DNDConnectionStrings.GetConnectionString("UseSQLite"));
            string cookieConsentName = Configuration.GetValue<string>("AppSettings:CookieConsentName");
            string cookieAuthName = Configuration.GetValue<string>("AppSettings:CookieAuthName"); //OpenID Connect
            string cookieApplicationAuthName = Configuration.GetValue<string>("AppSettings:CookieApplicationAuthName");
            string cookieExternalAuthName = Configuration.GetValue<string>("AppSettings:CookieExternalAuthName");
            string cookieTempDataName = Configuration.GetValue<string>("AppSettings:CookieTempDataName");
            string mvcImplementationFolder = Configuration.GetValue<string>("AppSettings:MVCImplementationFolder");
            string domain = Configuration.GetValue<string>("AppSettings:Domain");
            string assemblyPrefix = Configuration.GetValue<string>("AppSettings:AssemblyPrefix");
            string commonAssembly = "DND.Common";
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string xmlDocumentationFileName = assemblyName + ".xml";
            int responseCacheSizeMB = Configuration.GetValue<int>("AppSettings:ResponseCacheSizeMB");

            string SQLiteConnectionString = DNDConnectionStrings.GetConnectionString("SQLite");
            string SQLServerConnectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");

            //password
            bool requireDigit = Configuration.GetValue<bool>("AppSettings:Password:RequireDigit");
            int requiredLength = Configuration.GetValue<int>("AppSettings:Password:RequiredLength");
            int requiredUniqueChars = Configuration.GetValue<int>("AppSettings:Password:RequiredUniqueChars");
            bool requireLowercase = Configuration.GetValue<bool>("AppSettings:Password:RequireLowercase");
            bool requireNonAlphanumeric = Configuration.GetValue<bool>("AppSettings:Password:RequireNonAlphanumeric");
            bool requireUppercase = Configuration.GetValue<bool>("AppSettings:Password:RequireUppercase");

            //user
            bool requireConfirmedEmail = Configuration.GetValue<bool>("AppSettings:User:RequireConfirmedEmail");
            int registrationEmailConfirmationExprireDays = Configuration.GetValue<int>("AppSettings:User:RegistrationEmailConfirmationExprireDays");
            int forgotPasswordEmailConfirmationExpireHours = Configuration.GetValue<int>("AppSettings:User:ForgotPasswordEmailConfirmationExpireHours");
            int userDetailsChangeLogoutMinutes = Configuration.GetValue<int>("AppSettings:User:UserDetailsChangeLogoutMinutes");

            //External Logins
            bool enableApplicationLogin = Configuration.GetValue<bool>("AppSettings:Login:Application:Enable");
            bool enableJwtTokenLogin = Configuration.GetValue<bool>("AppSettings:Login:JwtToken:Enable");
            bool enableOpenIdConnectLogin = Configuration.GetValue<bool>("AppSettings:Login:OpenIdConnect:Enable");
            bool enableOpenIdConnectJwtTokenLogin = Configuration.GetValue<bool>("AppSettings:Login:OpenIdConnectJwtToken:Enable");

            bool enableGoogleLogin = Configuration.GetValue<bool>("AppSettings:Login:Google:Enable");
            string googleClientId = Configuration.GetValue<string>("AppSettings:Login:Google:ClientId");
            string googleClientSecret = Configuration.GetValue<string>("AppSettings:Login:Google:ClientSecret");

            bool enableFacebookLogin = Configuration.GetValue<bool>("AppSettings:Login:Facebook:Enable");
            string facebookClientId = Configuration.GetValue<string>("AppSettings:Login:Facebook:ClientId");
            string facebookClientSecret = Configuration.GetValue<string>("AppSettings:Login:Facebook:ClientSecret");

            var bin = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            Func<string, Boolean> assemblyFilter = (s => (new FileInfo(s)).Name.Contains(assemblyPrefix) || (new FileInfo(s)).Name.Contains(commonAssembly));
            string pluginsFolder = @"plugins\";
            string pluginsPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), pluginsFolder);
            if (!Directory.Exists(pluginsPath)) Directory.CreateDirectory(pluginsPath);

            //Email
            var fromDisplayName = Configuration.GetValue<string>("AppSettings:Email:FromDisplayName");
            var fromEmail = Configuration.GetValue<string>("AppSettings:Email:FromEmail");

            var toEmail = Configuration.GetValue<string>("AppSettings:Email:ToEmail");
            var toDisplayName = Configuration.GetValue<string>("AppSettings:Email:ToDisplayName");

            //Smtp
            bool sendEmailsViaSmtp = Configuration.GetValue<bool>("AppSettings:Email:SendEmailsViaSmtp");
            var username = Configuration.GetValue<string>("AppSettings:Email:Username");
            var password = Configuration.GetValue<string>("AppSettings:Email:Password");
            var host = Configuration.GetValue<string>("AppSettings:Email:Host");
            int port = Configuration.GetValue<int>("AppSettings:Email:Port");
            bool ssl = Configuration.GetValue<bool>("AppSettings:Email:Ssl");

            //Write to disk
            bool writeEmailsToFileSystem = Configuration.GetValue<bool>("AppSettings:Email:WriteEmailsToFileSystem");
            string fileSystemFolder = Configuration.GetValue<string>("AppSettings:Email:FileSystemFolder");

            //Token Signing Keys
            string bearerTokenLocalIssuer = Configuration["Tokens:LocalIssuer"];
            string bearerTokenExternalIssuers = Configuration["Tokens:ExternalIssuers"];
            string bearerTokenAudiences = Configuration["Tokens:Audiences"];

            string bearerTokenKey = Configuration["Tokens:Key"];
            string bearerTokenPrivateSigningKeyPath = HostingEnvironment.MapContentPath(Configuration.GetValue<string>("Tokens:PrivateKeyPath"));
            string bearerTokenPublicSigningKeyPath = HostingEnvironment.MapContentPath(Configuration.GetValue<string>("Tokens:PublicKeyPath"));
            string bearerTokenPrivateSigningCertificatePath = HostingEnvironment.MapContentPath(Configuration.GetValue<string>("Tokens:PrivateCertificatePath"));
            string bearerTokenPrivateSigningCertificatePassword = HostingEnvironment.MapContentPath(Configuration.GetValue<string>("Tokens:PrivateCertificatePassword"));
            string bearerTokenPublicSigningCertificatePath = HostingEnvironment.MapContentPath(Configuration.GetValue<string>("Tokens:PublicCertificatePath"));

            var emailsFileSystemPath = fileSystemFolder;
            if (!emailsFileSystemPath.Contains(@":\"))
            {
                emailsFileSystemPath = Path.Combine(bin, fileSystemFolder);
            }

            services.Configure<DisplayConventionsDisableOptions>(Configuration.GetSection("AppSettings:DisplayConventionsDisable"));

            //Load plugins into current AppDomain
            var pluginAssemblies = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly)
                               .Select(System.Reflection.Assembly.LoadFile);

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            if (useSQLite)
            {
                services.AddDbContextSqlite<IdentityDbContext>(SQLiteConnectionString);
            }
            else
            {
                services.AddDbContextSqlServer<IdentityDbContext>(SQLServerConnectionString);
            }

            if (useSQLite)
            {
                services.AddHangfireSqlite(SQLiteConnectionString);
            }
            else
            {
                services.AddHangfireSqlServer(SQLServerConnectionString);
            }

            if (enableApplicationLogin)
            {
                //Adds Scheme = "Identity.Application" (IdentityConstants.ApplicationScheme) cookie authentication scheme 
                services.AddIdentity<IdentityDbContext, User, IdentityRole>(requireDigit, requiredLength, requiredUniqueChars, requireLowercase, requireNonAlphanumeric,
                    requireUppercase, requireConfirmedEmail, registrationEmailConfirmationExprireDays, forgotPasswordEmailConfirmationExpireHours, userDetailsChangeLogoutMinutes);

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = cookieApplicationAuthName;
                });
            }


            services.ConfigureExternalCookie(options =>
              {
                  options.Cookie.Name = cookieExternalAuthName;
              });

            var authenticationBuilder = services.AddAuthentication();

            if (enableJwtTokenLogin)
            {
                //Adds Scheme = "Bearer" (JwtBearerDefaults.AuthenticationScheme) authentication scheme 

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // keep original claim types

                var signingKeys = new List<SecurityKey>();
                if (!String.IsNullOrWhiteSpace(bearerTokenKey))
                {
                    //Symmetric
                    signingKeys.Add(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearerTokenKey)));
                }

                if (!String.IsNullOrWhiteSpace(bearerTokenPublicSigningKeyPath))
                {
                    //Assymetric
                    signingKeys.Add(SigningKey.LoadPublicRsaSigningKey(bearerTokenPublicSigningKeyPath));
                }

                if (!String.IsNullOrWhiteSpace(bearerTokenPublicSigningCertificatePath))
                {
                    //Assymetric
                    signingKeys.Add(SigningKey.LoadPublicSigningCertificate(bearerTokenPublicSigningCertificatePath));
                }

                var validIssuers = new List<string>();
                foreach (var externalIssuer in bearerTokenExternalIssuers.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(externalIssuer))
                    {
                        validIssuers.Add(externalIssuer);
                    }
                }
                if (!string.IsNullOrWhiteSpace(bearerTokenLocalIssuer))
                {
                    validIssuers.Add(bearerTokenLocalIssuer);
                }

                var validAudiences = new List<string>();
                foreach (var audience in bearerTokenAudiences.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(audience))
                    {
                        validAudiences.Add(audience);
                    }
                }

                //https://developer.okta.com/blog/2018/03/23/token-authentication-aspnetcore-complete-guide
                authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, cfg =>
                         cfg.TokenValidationParameters = new TokenValidationParameters()
                         {
                             // Specify what in the JWT needs to be checked 
                             ValidateIssuer = true,
                             ValidateAudience = true,
                             ValidateLifetime = true,
                             ValidateIssuerSigningKey = true,
                             RequireSignedTokens = true,

                             ValidIssuers = validIssuers, //in the JWT this is the uri of the Identity Provider which issues the token.
                             ValidAudiences = validAudiences, //in the JWT this is aud. This is the resource the user is expected to have.

                             IssuerSigningKeys = signingKeys
                         }
                     );
            }

            if (EnableIFramesGlobal)
            {
                services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            }

            if (enableOpenIdConnectJwtTokenLogin)
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

            if (enableOpenIdConnectLogin)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // keep original claim types
                services.Configure<AuthenticationOptions>(options =>
                {
                    https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.1&tabs=aspnetcore2x
                    //overides "Identity.Application"/IdentityConstants.ApplicationScheme set by AddIdentity
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Challenge scheme is how user should login if they arent already logged in.
                });

                //authetication scheme
                authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
                {
                    options.Cookie.Name = cookieAuthName;
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

            services.Configure<EmailServiceOptions>(options =>
            {
                options.FromDisplayName = fromDisplayName;
                options.FromEmail = fromEmail;

                options.ToDisplayName = toDisplayName;
                options.ToEmail = toEmail;

                options.Username = username;
                options.Password = password;
                options.Host = host;
                options.Port = port;
                options.Ssl = ssl;

                options.PickupDirectoryLocation = emailsFileSystemPath;

                options.SendEmailsViaSmtp = sendEmailsViaSmtp;
                options.WriteEmailsToFileSystem = writeEmailsToFileSystem;
            });

            if (sendEmailsViaSmtp)
            {
                services.AddTransient<IEmailService, EmailServiceSmtp>();
            }
            else
            {
                services.AddTransient<IEmailService, EmailServiceFileSystem>();
            }

            services.Configure<AssemblyProviderOptions>(options =>
            {
                options.BinPath = bin;
                options.AssemblyFilter = assemblyFilter;
            });

            //services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewLocator(mvcImplementationFolder));

                //Add Embedded Views from other assemblies
                foreach (var pluginAssembly in pluginAssemblies)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(pluginAssembly));
                }
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("tokenCheck", typeof(TokenConstraint));
                options.ConstraintMap.Add("versionCheck", typeof(RouteVersionConstraint));
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

            services.AddCookieConsentNeeded(cookieConsentName);

            // Add framework services.
            var mvc = services.AddMvc(options =>
            {
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
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.Formatting = Formatting.Indented;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddCookieTempDataProvider(options =>
            {
                // new API
                options.Cookie.Name = cookieTempDataName;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Add Controllers from other assemblies
            mvc.AddApplicationPart(typeof(AdminFileManagerController).GetTypeInfo().Assembly).AddControllersAsServices();
            foreach (var pluginAssembly in pluginAssemblies)
            {
                mvc.AddApplicationPart(pluginAssembly).AddControllersAsServices();
            }

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

            //HSTS
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            //HTTPS
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });


            //CORS = Cross Origin Resource Sharing 
            //Is a client side security. Does not prevent 
            //https://docs.microsoft.com/en-us/aspnet/core/security/cors
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.1
                options.AddPolicy("AllowSpecificOrigin",
                  builder => builder
                  .WithOrigins("https://www." + domain + ".com", "http://www." + domain + ".com", "https://" + domain + ".com", "http://" + domain + ".com")
                  .AllowAnyMethod()
                  .AllowAnyHeader());
            });

            services.AddCustomModelMetadataProvider();
            services.AddCustomObjectValidator();
            services.AddInheritanceValidationAttributeAdapterProvider();
            services.AddConventionsHtmlGenerator();

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
                    Name = cookieApplicationAuthName,
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
            var assemblyPrefix = Configuration.GetValue<string>("AppSettings:AssemblyPrefix");
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
            builder.RegisterModule(new AutofacDbContextFactoryModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacConventionsMetadataModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacConventionsSignalRHubModule() { Paths = new string[] { binPath, pluginsPath }, Filter = stringFunc });
            builder.RegisterModule(new AutofacAutomapperModule() { Filter = filterFunc });

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, TaskRunner taskRunner, ISignalRHubMapper signalRHubMapper)
        {


            //settings
            bool enableCookieConsent = Configuration.GetValue<bool>("AppSettings:Switches:EnableCookieConsent");
            bool enableRedirectNonWwwToWww = Configuration.GetValue<bool>("AppSettings:Switches:EnableRedirectNonWwwToWww");
            bool enableRedirectHttpToHttps = Configuration.GetValue<bool>("AppSettings:Switches:EnableRedirectHttpToHttps");
            bool enableHsts = Configuration.GetValue<bool>("AppSettings:Switches:EnableHsts");
            bool enableHelloWord = Configuration.GetValue<bool>("AppSettings:Switches:EnableHelloWorld");
            bool enableSwagger = Configuration.GetValue<bool>("AppSettings:Switches:EnableSwagger");
            bool enableResponseCompression = Configuration.GetValue<bool>("AppSettings:Switches:EnableResponseCompression");
            bool enableIpRateLimiting = Configuration.GetValue<bool>("AppSettings:Switches:EnableIpRateLimiting");
            bool enableCors = Configuration.GetValue<bool>("AppSettings:Switches:EnableCors");
            bool enableResponseCaching = Configuration.GetValue<bool>("AppSettings:Switches:EnableResponseCaching");
            bool enableETags = Configuration.GetValue<bool>("AppSettings:Switches:EnableETags");
            bool enableHangfire = Configuration.GetValue<bool>("AppSettings:Switches:EnableHangfire");
            bool enableSignalR = Configuration.GetValue<bool>("AppSettings:Switches:EnableSignalR");

            string cookieConsentName = Configuration.GetValue<string>("AppSettings:CookieConsentName");
            string publicUploadFoldersString = Configuration.GetValue<string>("AppSettings:PublicUploadFolders");
            string assemblyPrefix = Configuration.GetValue<string>("AppSettings:AssemblyPrefix");
            string mvcImplementationFolder = Configuration.GetValue<string>("AppSettings:MVCImplementationFolder");
            string defaultCulture = Configuration.GetValue<string>("AppSettings:DefaultCulture");

            string uploadsFolder = "/uploads";
            string commonAssembly = "DND.Common";

            //cache
            int uploadFilesDays = Configuration.GetValue<int>("AppSettings:Cache:UploadFilesDays");
            int versionedStaticFilesDays = Configuration.GetValue<int>("AppSettings:Cache:VersionedStaticFilesDays");
            int nonVersionedStaticFilesDays = Configuration.GetValue<int>("AppSettings:Cache:NonVersionedStaticFilesDays");

            //signalr
            string signalRUrlPrefix = Configuration.GetValue<string>("AppSettings:SignalRUrlPrefix");

            foreach (var publicUploadFolder in publicUploadFoldersString.Split(','))
            {
                var path = env.WebRootPath + publicUploadFolder;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            //Pipeline
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            app.UseWhen(context => context.Request.Path.ToString().StartsWith("/ping"),
               appBranch =>
               {
                   appBranch.Run(async (context) =>
                   {
                       context.Response.ContentType = "text/html";
                       await context.Response
                           .WriteAsync("<!DOCTYPE html><html lang=\"en\"><head>" +
                               "<title></title></head><body><p>Hosted by Kestrel</p>");

                       if (serverAddressesFeature != null)
                       {
                           await context.Response
                               .WriteAsync("<p>Listening on the following addresses: " +
                                   string.Join(", ", serverAddressesFeature.Addresses) +
                                   "</p>");
                       }

                       await context.Response.WriteAsync("<p>Request URL: " +
                           $"{context.Request.GetDisplayUrl()}<p>");
                   });
               }
            );

            if (env.IsDevelopment() || env.EnvironmentName == "Integration")
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
                                //Whenever exceptions are thrown from api services.
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync(Messages.UnknownError);
                            });
                        });
                    }
               );

                if (enableHsts)
                {
                    //Only ever use HSTS in production!!!!!
                    //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                    app.UseHsts();
                }
            }

            if (enableRedirectHttpToHttps)
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                //picks up port automatically
                app.UseHttpsRedirection();
            }

            if (enableRedirectNonWwwToWww)
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
                if (HostingEnvironment.IsProduction())
                {
                    app.UseCors("AllowSpecificOrigin");
                }
                else
                {
                    app.UseCors("AllowAnyOrigin");
                }
            }

            if (enableSignalR)
            {
                app.UseSignalR(routes =>
                {
                    routes.MapHub<NotificationHub>(signalRUrlPrefix + "/signalr/notifications");
                    signalRHubMapper.MapHubs(routes, signalRUrlPrefix);
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
            if (enableResponseCaching)
            {
                if (enableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, cookieConsentName) && !IsStreamRequest(context),
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
            if (enableETags)
            {
                if (enableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, cookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseHttpCacheHeaders(true, true, true, true);
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                     appBranch =>
                     {
                         appBranch.UseHttpCacheHeaders(true, true, true, true);
                     }
                   );
                }
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

            //Default culture should be set to where the majority of traffic comes from.
            //If the client sends through "en" and the default culture is "en-AU". Instead of falling back to "en" it will fall back to "en-AU".
            var defaultLanguage = defaultCulture.Split('-')[0];

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
                new CultureInfo(defaultCulture)
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: defaultCulture, uiCulture: defaultCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedFormatCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedUICultures
            });

            app.UseDefaultFiles();

            //versioned files can have large cache expiry time
            app.UseVersionedStaticFiles(versionedStaticFilesDays);

            //non versioned files
            app.UseNonVersionedStaticFiles(nonVersionedStaticFilesDays);

            app.UseAuthentication();

            if (enableHangfire)
            {
                // Configure hangfire to use the new JobActivator.
                GlobalConfiguration.Configuration.UseActivator(new HangfireDependencyInjectionActivator(serviceProvider));
                app.UseHangfire();
            }

            if (enableCookieConsent)
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
            NavigationMenuHelperExtension.NavigationMenuHelper.MVCImplementationFolder = mvcImplementationFolder;

            taskRunner.RunTasksAfterApplicationConfiguration();
        }
    }
}
