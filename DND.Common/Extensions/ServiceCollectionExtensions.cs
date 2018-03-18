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

        public static void AddIdentity<TContext, TUser, TRole>(this IServiceCollection services) 
            where TContext : DbContext
            where TUser : class
            where TRole : class
        {
            services.AddIdentity<TUser, TRole>(cfg => cfg.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();
        }
    }
}
