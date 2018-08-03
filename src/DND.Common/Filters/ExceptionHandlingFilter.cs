using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using DND.Common.Alerts;
using DND.Common.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

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
                HandleApiException(context);
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

        private void HandleApiException(ExceptionContext context)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            WebApiMessage messageObject = null;

            if (context.Exception is ValidationErrors)
            {
                var errors = (ValidationErrors)context.Exception;

                var errorList = new List<string>();
                foreach (var databaseValidationError in errors.Errors)
                {
                    errorList.Add(databaseValidationError.PropertyExceptionMessage);
                }
                _logger.LogInformation(Messages.RequestInvalid);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestInvalid, errorList);
                statusCode = 422;
            }
            else if (context.Exception is OperationCanceledException)
            {
                //.NET generally just doesn't send a response at all

                var errorList = new List<string>();
                errorList.Add(Messages.RequestCancelled);
                _logger.LogInformation(Messages.RequestCancelled);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestCancelled, errorList);
                statusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is TimeoutException)
            {
                var errorList = new List<string>();
                errorList.Add(Messages.RequestTimedOut);
                _logger.LogInformation(Messages.RequestTimedOut);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestTimedOut, errorList);
                statusCode = (int)HttpStatusCode.GatewayTimeout;
            }
            else
            {

                //{
                //  "message": "An error has occurred.",
                //  "exceptionMessage": "Exception of type 'System.Exception' was thrown.",
                //  "exceptionType": "System.Exception",
                //  "stackTrace": "   at DND.Controllers.Api.AdminController.<Posts>d__2.MoveNext() in C:\\Development\\DND\\DND\\Controllers\\Api\\AdminController.cs:line 52\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Threading.Tasks.TaskHelpersExtensions.<CastToObject>d__3`1.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Web.Http.Controllers.ApiControllerActionInvoker.<InvokeActionAsyncCore>d__0.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Web.Http.Filters.ActionFilterAttribute.<CallOnActionExecutedAsync>d__5.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Web.Http.Filters.ActionFilterAttribute.<CallOnActionExecutedAsync>d__5.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Web.Http.Filters.ActionFilterAttribute.<ExecuteActionFilterAsyncCore>d__0.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Web.Http.Controllers.ActionFilterResult.<ExecuteAsync>d__2.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Web.Http.Controllers.ExceptionFilterResult.<ExecuteAsync>d__0.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Web.Http.Controllers.ExceptionFilterResult.<ExecuteAsync>d__0.MoveNext()\r\n--- End of stack trace from previous location where exception was thrown ---\r\n   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   at System.Web.Http.Dispatcher.HttpControllerDispatcher.<SendAsync>d__1.MoveNext()"
                //}

                var errorList = new List<string>();
                errorList.Add(Messages.UnknownError);
                _logger.LogInformation(Messages.UnknownError);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.UnknownError, errorList);
                statusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;

            var result = new ObjectResult(messageObject);
            result.StatusCode = (int)statusCode;

            context.Result = result;
        }
    }
}
