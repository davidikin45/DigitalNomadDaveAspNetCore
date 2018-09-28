using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Helpers;
using DND.Common.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DND.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
           string bearerTokenKey,
           string bearerTokenPublicSigningKeyPath,
           string bearerTokenPublicSigningCertificatePath,
           string bearerTokenExternalIssuers,
           string bearerTokenLocalIssuer,
           string bearerTokenAudiences)
        {
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

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
            if (!string.IsNullOrEmpty(bearerTokenExternalIssuers))
            {
                foreach (var externalIssuer in bearerTokenExternalIssuers.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(externalIssuer))
                    {
                        validIssuers.Add(externalIssuer);
                    }
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
            {
                cfg.SaveToken = true;
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
                };
            }
             );

            return services;
        }

        public static void AddCookiePolicy(this IServiceCollection services, string cookieConsentName)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.ConsentCookie.Name = cookieConsentName;
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        public static void AddDbContextInMemory<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
                    options.UseInMemoryDatabase(""));
        }

        public static void AddDbContextSqlServer<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString));
        }

        public static void AddDbContextSqlite<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
                    options.UseSqlite(connectionString));
        }

        public static void AddIdentity<TContext, TUser, TRole>(this IServiceCollection services,
        int maxFailedAccessAttemptsBeforeLockout,
        int lockoutMinutes,
        bool requireDigit,
        int requiredLength,
        int requiredUniqueChars,
        bool requireLowercase,
        bool requireNonAlphanumeric,
        bool requireUppercase,

        //user
        bool requireConfirmedEmail,
        bool requireUniqueEmail,
        int registrationEmailConfirmationExprireDays,
        int forgotPasswordEmailConfirmationExpireHours,
        int userDetailsChangeLogoutMinutes) 
            where TContext : DbContext
            where TUser : class
            where TRole : class
        {       
            services.AddIdentity<TUser, TRole>(options => 
            {
                options.Password.RequireDigit = requireDigit;
                options.Password.RequiredLength = requiredLength;
                options.Password.RequiredUniqueChars = requiredUniqueChars;
                options.Password.RequireLowercase = requireLowercase;
                options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;
                options.Password.RequireUppercase = requireUppercase;
                options.User.RequireUniqueEmail = requireUniqueEmail;
                options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
                options.Tokens.EmailConfirmationTokenProvider = "emailconf";

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = maxFailedAccessAttemptsBeforeLockout;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutMinutes);
            })
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<TUser>>("emailconf")
                .AddPasswordValidator<DoesNotContainPasswordValidator<TUser>>();

            //registration email confirmation days
            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
           options.TokenLifespan = TimeSpan.FromDays(registrationEmailConfirmationExprireDays));

            //forgot password hours
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(forgotPasswordEmailConfirmationExpireHours));

            //Security stamp validator validates every x minutes and will log out user if account is changed. e.g password change
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(userDetailsChangeLogoutMinutes);
            });
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, string apiName, string description, string contactName, string contactWebsite, string version, string xmlDocumentationPath)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new Info { Title = apiName, Description = description, Contact = new Contact() { Name = contactName, Email = null, Url = contactWebsite }, Version = version });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.OperationFilter<SwaggerAssignSecurityRequirements>();
                c.SchemaFilter<SwaggerModelExamples>();

                c.IncludeXmlComments(xmlDocumentationPath);
                c.DescribeAllEnumsAsStrings();

                c.DescribeAllParametersInCamelCase();
            });

            return services;
        }

        public static void ConfigureCorsAllowAnyOrigin(this IServiceCollection services, string name)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name,
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureCorsAllowSpecificOrigin(this IServiceCollection services, string name, params string[] domains)
        {
            services.AddCors(options =>
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.1
                options.AddPolicy(name,
                  builder => builder
                  .WithOrigins(domains)
                  .AllowAnyMethod()
                  .AllowAnyHeader());
            });
        }
    }
}
