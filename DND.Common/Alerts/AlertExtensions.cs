using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using DND.Common.Extensions;

namespace DND.Common.Alerts
{
	public static class AlertExtensions
	{
		const string Alerts = "_Alerts";

		public static List<Alert> GetAlerts(this ITempDataDictionary tempData)
		{
			if (!tempData.ContainsKey(Alerts))
			{
				tempData.Put(Alerts, new List<Alert>());
			}

			return tempData.Get<List<Alert>>(Alerts);
		}

        public static void AddAlert(this ITempDataDictionary tempData, Alert alert)
        {
            var alerts = GetAlerts(tempData);
            alerts.Add(alert);
            tempData.Put(Alerts, alerts);
        }

        //Model Errors should be shown in validationsummary not an alert
        //public static ActionResult WithModelErrors(this ActionResult result, ModelStateDictionary modelState)
        //{
        //    foreach (KeyValuePair<string, System.Web.Mvc.ModelState> property in modelState)
        //    {
        //        foreach (System.Web.Mvc.ModelError modelError in property.Value.Errors)
        //        {
        //            var errorMessage = modelError.ErrorMessage;
        //            result = result.WithError(errorMessage);
        //        }
        //    }
        //    return result;
        //}

        public static ActionResult WithSuccess(this ActionResult result, Controller controller, string message)
		{
			return new AlertDecoratorResult(result, "alert-success", message, controller);
		}

		public static ActionResult WithInfo(this ActionResult result, Controller controller, string message)
		{
			return new AlertDecoratorResult(result, "alert-info", message, controller);
		}

		public static ActionResult WithWarning(this ActionResult result, Controller controller, string message)
		{
			return new AlertDecoratorResult(result, "alert-warning", message, controller);
		}

		public static ActionResult WithError(this ActionResult result, Controller controller, string message)
		{
			return new AlertDecoratorResult(result, "alert-danger", message, controller);
		}
	}
}