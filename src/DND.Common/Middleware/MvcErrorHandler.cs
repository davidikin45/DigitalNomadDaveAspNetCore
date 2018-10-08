using DND.Common.Infrastructure.Validation.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Middleware
{
    public static class MvcErrorHandler
    {
        public static (IActionResult result, bool exceptionHandled) HandleException(ClaimsPrincipal user, Exception exception)
        {
            bool exceptionHandled = false;
            IActionResult result = null;

            if (exception is UnauthorizedErrors)
            {
                if(user.Identity.IsAuthenticated)
                {
                    result = new ForbidResult();
                }
                else
                {
                    result = new ChallengeResult();
                }
                exceptionHandled = true;
            }
            else if (exception is OperationCanceledException)
            {

            }
            else if (exception is TimeoutException)
            {

            }
            else
            {

            }

            return (result, exceptionHandled);
        }
    }
}
