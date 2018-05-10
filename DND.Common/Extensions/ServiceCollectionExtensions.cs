using DND.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
