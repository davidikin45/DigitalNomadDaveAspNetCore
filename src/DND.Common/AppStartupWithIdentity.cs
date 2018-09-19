using DND.Common.Extensions;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DND.Common
{
    public class AppStartupWithIdentity<TIdentiyDbContext, TUser> : AppStartup
        where TIdentiyDbContext : DbContext
        where TUser : class
    {
        public AppStartupWithIdentity(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            :base(configuration, hostingEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var passwordSettingsSection = Configuration.GetSection("PasswordSettings");
            var passwordSettings = passwordSettingsSection.Get<PasswordSettings>();

            var userSettingsSection = Configuration.GetSection("UserSettings");
            var userSettings = userSettingsSection.Get<UserSettings>();

            var loginSettingsSection = Configuration.GetSection("LoginSettings");
            var loginSettings = loginSettingsSection.Get<LoginSettings>();

            var connectionString = Configuration.GetConnectionString("DefaultConnectionString");

            services.AddDbContextSqlServer<TIdentiyDbContext>(connectionString);
            
            if (loginSettings.Application.Enable || loginSettings.JwtToken.Enable)
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
    }
}
