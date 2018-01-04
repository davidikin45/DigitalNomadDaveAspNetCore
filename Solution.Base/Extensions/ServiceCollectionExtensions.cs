using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContext<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
                    options.UseSqlServer(connectionString));
        }

        public static void AddIdentity<TContext, TUser, TRole>(this IServiceCollection services) 
            where TContext : DbContext
            where TUser : class
            where TRole : class
        {
            services.AddIdentity<TUser, TRole>()
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();
        }
    }
}
