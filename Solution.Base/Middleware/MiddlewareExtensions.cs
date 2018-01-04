using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTasks(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTasksMiddleware>();
        }

        public static IApplicationBuilder UseContentHandler(
           this IApplicationBuilder builder, List<string> publicUploadFolders)
        {
            return builder.UseMiddleware<ContentHandlerMiddleware>(publicUploadFolders);
        }

        public static IApplicationBuilder UseResponseCachingCustom(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseCachingCustomMiddleware>();
        }
    }
}
