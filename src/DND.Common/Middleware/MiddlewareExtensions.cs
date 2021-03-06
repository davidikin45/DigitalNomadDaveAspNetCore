﻿using DND.Common.Hangfire;
using DND.Common.Infrastructure.Settings;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace DND.Common.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuth(
           this IApplicationBuilder builder)
        {
           return builder.UseMiddleware<BasicAuthMiddleware>();
        }

        public static IApplicationBuilder UseRequestTasks(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTasksMiddleware>();
        }

        public static IApplicationBuilder UseContentHandler(
           this IApplicationBuilder builder, AppSettings appSettings, List<string> publicUploadFolders, int cacheDays)
        {
            return builder.UseMiddleware<ContentHandlerMiddleware>(publicUploadFolders, appSettings, cacheDays);
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

        public static IApplicationBuilder UseStackifyPrefix(this IApplicationBuilder app)
        {
            return app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
        }
    }
}
