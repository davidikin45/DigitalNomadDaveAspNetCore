using DND.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
        bool requireDigit,
        int requiredLength,
        int requiredUniqueChars,
        bool requireLowercase,
        bool requireNonAlphanumeric,
        bool requireUppercase,

        //user
        bool requireConfirmedEmail,
        int registrationEmailConfirmationExprireDays,
        int forgotPasswordEmailConfirmationExpireHours,
        int userDetailsChangeLogoutMinutes) 
            where TContext : DbContext
            where TUser : class
            where TRole : class
        {       
            services.AddIdentity<TUser, TRole>(options => 
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = requireDigit;
                options.Password.RequiredLength = requiredLength;
                options.Password.RequiredUniqueChars = requiredUniqueChars;
                options.Password.RequireLowercase = requireLowercase;
                options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;
                options.Password.RequireUppercase = requireUppercase;

                options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
                options.Tokens.EmailConfirmationTokenProvider = "emailconf";

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
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

        public static IServiceCollection ConfigureRazorViewEngineForTestServer(this IServiceCollection services, Assembly assembly)
        {
            //https://github.com/aspnet/Hosting/issues/954
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                var previous = options.CompilationCallback;
                options.CompilationCallback = (context) =>
                {
                    previous?.Invoke(context);

                    var assemblies = assembly.GetReferencedAssemblies().Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
                    .ToList();
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading.Tasks")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));

                    context.Compilation = context.Compilation.AddReferences(assemblies);
                };
            });
        }

    }
}
