using DND.Common.Alerts;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Validation.Errors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace DND.Common.Middleware
{
    public static class ApiErrorHandler
    {
        public static (WebApiMessage message, int statusCode, bool exceptionHandled) HandleApiException(ClaimsPrincipal user, Exception exception, ILogger logger)
        {
            bool exceptionHandled = false;
            int statusCode = (int)HttpStatusCode.InternalServerError;
            WebApiMessage messageObject = null;

            if (exception is UnauthorizedErrors)
            {
                var errors = (UnauthorizedErrors)exception;

                var errorList = new List<string>();
                foreach (var validationError in errors.Errors)
                {
                    errorList.Add(validationError.PropertyExceptionMessage);
                }
                logger.LogInformation(Messages.Unauthorised);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.Unauthorised, errorList);
                if(user.Identity.IsAuthenticated)
                {
                    statusCode = (int)HttpStatusCode.Forbidden;
                }
                else
                {
                    statusCode = (int)HttpStatusCode.Unauthorized;
                }
                exceptionHandled = true;
            }
            else if (exception is OperationCanceledException)
            {
                //.NET generally just doesn't send a response at all

                var errorList = new List<string>();
                errorList.Add(Messages.RequestCancelled);
                logger.LogInformation(Messages.RequestCancelled);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestCancelled, errorList);
                statusCode = (int)HttpStatusCode.BadRequest;
                exceptionHandled = true;
            }
            else if (exception is TimeoutException)
            {
                var errorList = new List<string>();
                errorList.Add(Messages.RequestTimedOut);
                logger.LogInformation(Messages.RequestTimedOut);

                messageObject = WebApiMessage.CreateWebApiMessage(Messages.RequestTimedOut, errorList);
                statusCode = (int)HttpStatusCode.GatewayTimeout;
                exceptionHandled = true;
            }
            else
            {

            }

            return (messageObject, statusCode, exceptionHandled);
        }

        public static (WebApiMessage message, int statusCode, bool exceptionHandled) HandleApiExceptionGlobal(Exception exception, bool showExceptionMessage, ILogger logger)
        {
            bool exceptionHandled = true;
            int statusCode = (int)HttpStatusCode.InternalServerError;
            WebApiMessage messageObject = null;

            var errorList = new List<string>();

            if(showExceptionMessage)
            {
                errorList.Add(Messages.UnknownError);
            }
            else
            {
                errorList.Add(exception.Message);
            }

            logger.LogError(500, exception, exception.Message);

            messageObject = WebApiMessage.CreateWebApiMessage(Messages.UnknownError, errorList);
            statusCode = (int)HttpStatusCode.InternalServerError;

            return (messageObject, statusCode, exceptionHandled);
        }
    }
}
