using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Solution.Base.Helpers;
using Solution.Base.Interfaces.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class ControllerExtensions
    {

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
