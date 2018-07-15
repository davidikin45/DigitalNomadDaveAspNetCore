using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Common.Implementation.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading;
using DND.Common.ActionResults;
using DND.Common.Alerts;
using System.Net;
using System.Linq.Expressions;
using DND.Common.Helpers;
using DND.Common.Extensions;
using DND.Common.Email;
using DND.Common.Interfaces.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace DND.Common.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseController : Controller
    {
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public IConfiguration Configuration { get; }

        public BaseController()
        {

        }

        public BaseController(IMapper mapper = null, IEmailService emailService = null, IConfiguration configuration = null)
        {
            Mapper = mapper;
            EmailService = emailService;
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

        protected virtual BetterJsonResult BetterJsonError(string message, ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors);

            var JsonResult = new BetterJsonResult(message, modelState);

            foreach (var error in errors)
            {
                JsonResult.AddError(error.ErrorMessage);
            }

            return JsonResult;
        }

        protected virtual BetterJsonResult BetterJsonError(string message, ValidationErrors errors, int errorStatusCode = 400)
        {
            var JsonResult = new BetterJsonResult(message);
            JsonResult.ErrorStatusCode = errorStatusCode;
            foreach (var databaseValidationError in errors.Errors)
            {
                JsonResult.AddError(databaseValidationError.PropertyExceptionMessage);
            }

            return JsonResult;
        }

        protected virtual BetterJsonResult BetterJsonError(string message, string errorMessage, int errorStatusCode = 400)
        {
            var JsonResult = new BetterJsonResult(message);
            JsonResult.ErrorStatusCode = errorStatusCode;
            JsonResult.AddError(errorMessage);

            return JsonResult;
        }

        protected virtual BetterJsonResult<T> BetterJsonSuccess<T>(T model)
        {
            return new BetterJsonResult<T>() { Value = model };
        }

        protected ActionResult RedirectToAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            return ControllerExtensions.RedirectToAction(this, action);
        }

        [Obsolete("Do not use the standard Json helpers to return JSON data to the client.  Use either BetterJsonSuccess or BetterJsonError instead.")]
        protected JsonResult Json<T>(T data)
        {
            throw new InvalidOperationException("Do not use the standard Json helpers to return JSON data to the client.  Use either BetterJsonSuccess or BetterJsonError instead.");
        }

        protected CancellationToken ClientDisconnectedToken()
        {
            return HttpContext.RequestAborted;
        }
        #region "Errors"

        protected BetterJsonResult Error(string message)
        {
            return BetterJsonError("The request is invalid.", message);
        }

        protected BetterJsonResult ValidationErrors(ModelStateDictionary modelState)
        {
            return BetterJsonError("The request is invalid.", modelState);
        }

        protected BetterJsonResult ValidationErrors(ValidationErrors errors)
        {
            return BetterJsonError("The request is invalid.", errors);
        }

        protected BetterJsonResult OperationCancelledError(OperationCanceledException ex)
        {
            //Logger.Error(ex, "The request was cancelled.");
            return BetterJsonError("The request was cancelled.", "The request was cancelled.", (int)HttpStatusCode.InternalServerError);
        }

        protected BetterJsonResult OtherError(Exception ex)
        {
            //Logger.Error(ex, Messages.UnknownError);
            return BetterJsonError(Messages.UnknownError, Messages.UnknownError, (int)HttpStatusCode.InternalServerError);
        }
        #endregion

        protected ActionResult HandleReadException()
        {
            return RedirectToControllerDefault().WithError(this, Messages.RequestInvalid);
        }

        protected void HandleUpdateException(Result failure, IBaseDtoConcurrencyAware dto, bool clearPostData)
        {
            //TODO: Need to research how to turn off automatic model validation if doing it in application service layer
            //Clears all Post Data
            if (clearPostData)
            {
                ModelState.Clear();
            }

            switch (failure.ErrorType)
            {
                case ErrorType.ObjectValidationFailed:
                    ModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    break;
                case ErrorType.ConcurrencyConflict:
                    ModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    dto.RowVersion = failure.NewRowVersion;
                    //Update RowVersion on DTO
                    break;
                default:
                    ModelState.AddModelError("", Messages.UnknownError);
                    break;
            }
        }

        protected void HandleUpdateException(Exception ex)
        {
            if (ex is ValidationErrors)
            {
                var propertyErrors = (ValidationErrors)ex;
                ModelState.AddValidationErrors(propertyErrors);
            }
            else
            {
                ModelState.AddModelError("", Messages.UnknownError);
            }
        }

        protected virtual ActionResult RedirectToHome()
        {
            return RedirectToRoute("Default");
        }

        protected virtual ActionResult RedirectToControllerDefault()
        {
            return RedirectToAction("Index");
        }

        protected string ControllerName
        {
            get { return this.ControllerContext.RouteData.Values["controller"].ToString(); }
        }

        protected string Title
        {
            get { return this.HttpContext.Request.Path; }
        }
    }
}
