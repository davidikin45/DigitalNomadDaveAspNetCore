using AutoMapper;
using DND.Common.ActionResults;
using DND.Common.Alerts;
using DND.Common.Extensions;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace DND.Common.Controllers.Api
{


    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [ApiController]
    public abstract class ApiControllerBase : Controller
    {
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public IUrlHelper UrlHelper { get; }
        public IConfiguration Configuration { get; }


        public ApiControllerBase()
        {

        }

        public ApiControllerBase(IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, IConfiguration configuration = null)
        {
            Mapper = mapper;
            EmailService = emailService;
            UrlHelper = urlHelper;
            Configuration = configuration;
        }

        //https://docs.microsoft.com/en-us/aspnet/core/migration/claimsprincipal-current?view=aspnetcore-2.0
        public string Username
        {
            get
            {
                if (User != null && User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
                {
                    return User.Identity.Name;
                }
                else
                {
                    return "Anonymous";
                }
            }
        }

        protected IActionResult BulkTriggerActionResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.ActionSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return Success(webApiMessages);
        }

        protected IActionResult BulkCreateResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.AddSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return Success(webApiMessages);
        }

        protected IActionResult BulkUpdateResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if(result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.UpdateSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return Success(webApiMessages);
        }

        protected IActionResult BulkDeleteResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.DeleteSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return Success(webApiMessages);
        }

        protected IActionResult ValidationErrors(Result failure)
        {
            var newModelState = new ModelStateDictionary();
            switch (failure.ErrorType)
            {
                case ErrorType.ObjectValidationFailed:
                    newModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    break;
                case ErrorType.ObjectDoesNotExist:
                    return ApiNotFoundErrorMessage(Messages.NotFound);
                case ErrorType.ConcurrencyConflict:
                    newModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    break;
                default:
                    //perhaps should be throwing so Startup returns a 500
                    //throw ex;
                    newModelState.AddModelError("", Messages.UnknownError);
                    break;
            }
            return ValidationErrors(newModelState);
        }

        protected IActionResult ValidationErrors()
        {
            return ValidationErrors(Messages.RequestInvalid, ModelState);
        }

        protected IActionResult ValidationErrors(ModelStateDictionary modelState)
        {
            return ValidationErrors(Messages.RequestInvalid, modelState);
        }

        protected virtual IActionResult ValidationErrors(string message, ModelStateDictionary modelState)
        {
            return new UnprocessableEntityAngularObjectResult(message, modelState);
        }

        protected virtual IActionResult Success<T>(T model)
        {
            return new OkObjectResult(model);
        }

        protected CancellationToken ClientDisconnectedToken()
        {
            return this.HttpContext.RequestAborted;
        }

        protected IActionResult ApiBadRequest()
        {
            return ApiErrorMessage(Messages.RequestInvalid);
        }

        protected IActionResult ApiErrorMessage(string message)
        {
            return ApiErrorMessage(Messages.RequestInvalid, message);
        }

        protected IActionResult ApiNotFound()
        {
            return ApiNotFoundErrorMessage(Messages.NotFound);
        }

        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(errorMessage);
        }

        protected IActionResult ApiNotFoundErrorMessage(string message)
        {
            return ApiErrorMessage(Messages.NotFound, message, 404);
        }

        protected virtual IActionResult ApiErrorMessage(string message, string errorMessage, int errorStatusCode = 400)
        {
            var errorList = new List<string>();
            errorList.Add(errorMessage);

            var response = WebApiMessage.CreateWebApiMessage(message, errorList);

            var result = new ObjectResult(response);
            result.StatusCode = errorStatusCode;

            return result;
        }

        protected virtual IActionResult ApiCreatedSuccessMessage(string message, Object id)
        {
            return ApiSuccessMessage(message, id, HttpStatusCode.Created);
        }

        protected virtual IActionResult ApiSuccessMessage(string message, Object id, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var errorList = new List<string>();

            var response = WebApiMessage.CreateWebApiMessage(message, errorList, id);

            var result = new ObjectResult(response);
            result.StatusCode = (int)statusCode;

            return result;
        }

        protected virtual IActionResult Html(string html)
        {
            return new HTMLActionResult(html);
        }

        protected virtual IActionResult Forbidden()
        {
            return ApiErrorMessage(Messages.Unauthorised, Messages.Unauthorised, 403);
        }

    }
}

