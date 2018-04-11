using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DND.Common.ActionResults;
using DND.Common.Alerts;
using DND.Common.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using DND.Common.Implementation.Validation;
using DND.Common.Extensions;

namespace DND.Common.Controllers.Api
{


    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    public abstract class BaseWebApiController : Controller
    {
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public IUrlHelper UrlHelper { get; }

        public BaseWebApiController()
        {

        }

        public BaseWebApiController(IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null)
        {
            Mapper = mapper;
            EmailService = emailService;
            UrlHelper = urlHelper;
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
                default:
                    newModelState.AddModelError("", Messages.UnknownError);
                    break;
            }
            return ValidationErrors(newModelState);
        }

        protected IActionResult ValidationErrors(ModelStateDictionary modelState)
        {
            return ValidationErrors(Messages.RequestInvalid, modelState);
        }

        protected virtual IActionResult ValidationErrors(string message, ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors);
            var errorList = new List<string>();
            foreach (var error in errors)
            {
                errorList.Add(error.ErrorMessage);
            }

            var response = WebApiMessage.CreateWebApiMessage(message, errorList, modelState);

            var result = new ObjectResult(response);
            result.StatusCode = 422;

            return result;
        }

        //protected virtual IHttpActionResult BetterJsonError(string message, ValidationErrors errors, int errorStatusCode = 400)
        //{
        //    var errorList = new List<string>();
        //    foreach (var databaseValidationError in errors.Errors)
        //    {
        //        errorList.Add(databaseValidationError.PropertyExceptionMessage);
        //    }

        //    var WebApiMessage = WebApiMessage.CreateWebApiMessage(message, errorList);

        //    return Content((HttpStatusCode)errorStatusCode, WebApiMessage);
        //}

        protected virtual IActionResult Success<T>(T model)
        {
            return new OkObjectResult(model);
        }

        [Obsolete("Do not use the standard Json helpers to return JSON data to the client.  Use either JsonSuccess or JsonError instead.")]
        protected new JsonResult Json<T>(T data)
        {
            throw new InvalidOperationException("Do not use the standard Json helpers to return JSON data to the client.  Use either JsonSuccess or JsonError instead.");
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

