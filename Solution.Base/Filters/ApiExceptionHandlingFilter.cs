using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Solution.Base.Alerts;
using Solution.Base.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Solution.Base.Filters
{
    public class ApiExceptionHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ApiExceptionHandlingFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ApiExceptionHandlingFilter>();
        }
        public override void OnException(ExceptionContext context)
        {
            if(context.HttpContext.Request.Path.ToString().StartsWith("/api"))
            {
                HandleEexception(context);
            }        
        }

        private void HandleEexception(ExceptionContext context)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
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
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (context.Exception is OperationCanceledException)
            {
                //.NET generally just doesn't send a response at all

                var errorList = new List<string>();
                errorList.Add(Messages.RequestCancelled);
                _logger.LogInformation(Messages.RequestCancelled);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestCancelled, errorList);
                statusCode = HttpStatusCode.BadRequest;
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
                statusCode = HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;

            var result = new ObjectResult(messageObject);
            result.StatusCode = (int)statusCode;

            context.Result = result;
        }
    }
}
