using DND.Common.Extensions;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DND.Common
{
    public abstract class AppStartupWithIdentity<TIdentiyDbContext, TUser> : AppStartup
        where TIdentiyDbContext : DbContext
        where TUser : class
    {
        public AppStartupWithIdentity(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            :base(loggerFactory, configuration, hostingEnvironment)
        {

        }

        public override void ConfigureIdentityServices(IServiceCollection services)
        {
            base.ConfigureIdentityServices(services);

            var passwordSettings = GetSettings<PasswordSettings>("PasswordSettings");
            var userSettings = GetSettings<UserSettings>("UserSettings");
            var authenticationSettings = GetSettings<AuthenticationSettings>("AuthenticationSettings");

            if (authenticationSettings.Application.Enable || authenticationSettings.JwtToken.Enable)
            {
                services.AddIdentity<TIdentiyDbContext, TUser, IdentityRole>(
                passwordSettings.MaxFailedAccessAttemptsBeforeLockout,
                passwordSettings.LockoutMinutes,
                passwordSettings.RequireDigit,
                passwordSettings.RequiredLength,
                passwordSettings.RequiredUniqueChars,
                passwordSettings.RequireLowercase,
                passwordSettings.RequireNonAlphanumeric,
                passwordSettings.RequireUppercase,
                userSettings.RequireConfirmedEmail,
                userSettings.RequireUniqueEmail,
                userSettings.RegistrationEmailConfirmationExprireDays,
                userSettings.ForgotPasswordEmailConfirmationExpireHours,
                userSettings.UserDetailsChangeLogoutMinutes);
            }
        }

        public override void AddDatabases(IServiceCollection services, string defaultConnectionString)
        {
            services.AddDbContextSqlServer<TIdentiyDbContext>(defaultConnectionString);
        }
    }
}
