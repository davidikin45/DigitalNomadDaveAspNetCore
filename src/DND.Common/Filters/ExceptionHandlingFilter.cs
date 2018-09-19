using DND.Common.Alerts;
using DND.Common.Infrastructure.Validation.Errors;
using DND.Common.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace DND.Common.Filters
{
    //This will only handle MVC errors
    //https://stackoverflow.com/questions/42582758/asp-net-core-middleware-vs-filters
    public class ExceptionHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ExceptionHandlingFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionHandlingFilter>();
        }
        public override void OnException(ExceptionContext context)
        {
            LogException(context);
            if (context.HttpContext.Request.Path.ToString().StartsWith("/api"))
            {
                var result = ApiErrorHandler.HandleApiException(context.Exception, _logger);
                if (result.exceptionHandled)
                {
                    context.ExceptionHandled = true;

                    var objectResult = new ObjectResult(result.message);
                    objectResult.StatusCode = result.statusCode;

                    context.Result = objectResult;
                }
            }
        }

        private void LogException(ExceptionContext context)
        {
            if (context.Exception is ValidationErrors)
            {

            }
            if (context.Exception is TimeoutException)
            {

            }
            else if (context.Exception is OperationCanceledException)
            {

            }
            else
            {
                _logger.LogError(context.Exception, Messages.UnknownError);
            }
        }
    }
}
