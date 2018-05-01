using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DND.Common.Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTasks(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTasksMiddleware>();
        }

        public static IApplicationBuilder UseContentHandler(
           this IApplicationBuilder builder, IConfiguration configuration, List<string> publicUploadFolders)
        {
            return builder.UseMiddleware<ContentHandlerMiddleware>(publicUploadFolders, configuration);
        }

        public static IApplicationBuilder UseResponseCachingCustom(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseCachingCustomMiddleware>();
        }

        public static IApplicationBuilder UseVersionedStaticFiles(
         this IApplicationBuilder app, int days)
        {
           return app.UseWhen(context => context.Request.Query.ContainsKey("v"),
                  appBranch =>
                  {
                      //cache js, css
                      appBranch.UseStaticFiles(new StaticFileOptions
                      {
                          OnPrepareResponse = ctx =>
                          {
                              if (days > 0)
                              {
                                  TimeSpan timeSpan = new TimeSpan(days * 24, 0, 0);
                                  ctx.Context.Response.GetTypedHeaders().Expires = DateTime.Now.Add(timeSpan).Date.ToUniversalTime();
                                  ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                  {
                                      Public = true,
                                      MaxAge = timeSpan
                                  };
                              }
                              else
                              {
                                  ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                  {
                                      NoCache = true
                                  };
                              }
                          }
                      });
                  }
             );
        }

        public static IApplicationBuilder UseNonVersionedStaticFiles(
       this IApplicationBuilder app, int days)
        {
           return app.UseWhen(context => !context.Request.Query.ContainsKey("v"),
                  appBranch =>
                  {
                      //cache js, css
                      appBranch.UseStaticFiles(new StaticFileOptions
                      {
                          OnPrepareResponse = ctx =>
                          {
                              if (days > 0)
                              {
                                  TimeSpan timeSpan = new TimeSpan(days * 24, 0, 0);
                                  ctx.Context.Response.GetTypedHeaders().Expires = DateTime.Now.Add(timeSpan).Date.ToUniversalTime();
                                  ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                  {
                                      Public = true,
                                      MaxAge = timeSpan
                                  };
                              }
                              else
                              {
                                  ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                                  {
                                      NoCache = true
                                  };
                              }
                          }
                      });
                  }
             );
        }

        public static IServiceCollection AddHangfireSqlServer(this IServiceCollection services, string connectionString)
        {
            return services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
        }

        public static IServiceCollection AddHangfireSqlite(this IServiceCollection services, string connectionString)
        {
            return services.AddHangfire(config => config.UseSQLiteStorage(connectionString));
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder builder)
        {

            builder.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationfilter() },
                AppPath = "/admin"
            });
            builder.UseHangfireServer();
            return builder;
        }

    }
}
