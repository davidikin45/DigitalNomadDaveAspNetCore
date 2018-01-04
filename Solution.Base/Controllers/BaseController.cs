using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Implementation.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading;
using Solution.Base.ActionResults;
using Solution.Base.Alerts;
using System.Net;
using System.Linq.Expressions;
using Solution.Base.Helpers;
using Solution.Base.Extensions;
using Solution.Base.Email;

namespace Solution.Base.Controllers
{
    public abstract class BaseController : Controller
    {
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }

        public BaseController()
        {

        }

        public BaseController(IMapper mapper = null, IEmailService emailService = null)
        {
            Mapper = mapper;
            EmailService = emailService;
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
