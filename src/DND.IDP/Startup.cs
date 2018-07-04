using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DND.IDP.Entities;
using DND.IDP.Services;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DND.IDP
{
    public class Startup
    {
        private bool InMemory = false;

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var builder = services.AddIdentityServer();

            if(HostingEnvironment.IsProduction())
            {
                builder.AddSigningCredential(LoadCertificateFromStore());
            }
            else
            {
                builder.AddDeveloperSigningCredential();
            }

            if(InMemory)
            {
                builder.AddTestUsers(Config.GetUsers());
                builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
                builder.AddInMemoryApiResources(Config.GetApiResources());
                builder.AddInMemoryClients(Config.GetClients());
                builder.AddInMemoryPersistedGrants();
            }
            else
            {
                var connectionString = Configuration["connectionStrings:IDPUserDBConnectionString"];
                services.AddDbContext<UserContext>(o => o.UseSqlServer(connectionString));

                services.AddScoped<IUserRepository, UserRepository>();

                var idpDataDBConnectionString = Configuration["connectionStrings:IDPDataDBConnectionString"];

                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                builder.AddUserStore()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbContextBuilder =>
                    {
                        dbContextBuilder.UseSqlServer(idpDataDBConnectionString, sqlServerOptions => sqlServerOptions.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbContextBuilder =>
                    {
                        dbContextBuilder.UseSqlServer(idpDataDBConnectionString, sqlServerOptions => sqlServerOptions.MigrationsAssembly(migrationsAssembly));
                    };
                });
            }

            services.AddAuthentication(options => {
                options.DefaultScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
            }
            ).AddCookie(IdentityServerConstants.DefaultCookieAuthenticationScheme+".2FA", options =>
            {

            });

            services.AddAuthentication().AddFacebook(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.AppId = "1570475679676847";
                options.AppSecret = "5b9f4bca5da29e234b706040aca883d3";
            });
        }

        //Load from here to prevent load balancer issues
        public X509Certificate2 LoadCertificateFromStore()
        {
            string thumbPrint = "AD4AD167DE7171FAA3185480C088C877CD702562";

            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,
                    thumbPrint, true);
                if (certCollection.Count == 0)
                {
                    throw new Exception("The specified certificate wasn't found.");
                }
                return certCollection[0];
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}
