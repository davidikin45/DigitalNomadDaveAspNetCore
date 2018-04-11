using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using DND.Common.Helpers;
using DND.Common.Interfaces.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace DND.Common.Extensions
{
    public static class ControllerExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor actionDescriptor) where T : Attribute
        {
            var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                return controllerActionDescriptor.MethodInfo.GetCustomAttributes<T>();
            }

            return Enumerable.Empty<T>();
        }

        public static void AddValidationErrors(this ModelStateDictionary modelState, IEnumerable<ValidationResult> errors)
        {
            foreach (var err in errors)
            {
                if (err.MemberNames.Count() > 0)
                {
                    foreach (var prop in err.MemberNames)
                    {
                        modelState.AddModelError(prop, err.ErrorMessage);
                    }
                }
                else
                {
                    modelState.AddModelError("", err.ErrorMessage);
                }
            }
        }

        public static void AddValidationErrors(this ModelStateDictionary modelState, IValidationErrors errors)
        {
            foreach (var error in errors.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.PropertyExceptionMessage);
            }
        }

        public static RedirectToRouteResult RedirectToAction<TController>(this TController controller, Expression<Action<TController>> action) where TController : Controller
        {
            return RedirectToAction((Controller)controller, action);
        }

        public static RedirectToRouteResult RedirectToAction<TController>(this Controller controller, Expression<Action<TController>> action) where TController : Controller
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            RouteValueDictionary routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            return new RedirectToRouteResult(routeValues);
        }
    }
}
