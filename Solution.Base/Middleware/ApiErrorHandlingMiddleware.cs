using Microsoft.AspNetCore.Http;
using Solution.Base.Alerts;
using Solution.Base.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Middleware
{
    public class ApiErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ApiErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            WebApiMessage messageObject = null;

            if (exception is ValidationErrors)
            {
                var errors = (ValidationErrors)exception;

                var errorList = new List<string>();
                foreach (var databaseValidationError in errors.Errors)
                {
                    errorList.Add(databaseValidationError.PropertyExceptionMessage);
                }

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestInvalid, errorList);
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (exception is OperationCanceledException)
            {
                //.NET generally just doesn't send a response at all

                var errorList = new List<string>();
                errorList.Add(Messages.RequestCancelled);

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


                messageObject = WebApiMessage.CreateWebApiMessage(Messages.UnknownError, errorList);
                statusCode = HttpStatusCode.InternalServerError;
            }

            //var contentType = negotiator.Negotiate(typeof(WebApiMessage), context.Request, formatters);
            //var result = new NegotiatedContentResult<WebApiMessage>(statusCode, messageObject, negotiator, context.Request, formatters);

            return context.Response.WriteAsync("");
        }
    }
}
