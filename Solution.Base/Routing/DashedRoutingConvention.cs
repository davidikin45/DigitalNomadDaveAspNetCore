using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace Solution.Base.Routing
{
    public class DashedRoutingConvention : IControllerModelConvention
    {
        public static string DefaultControllerName { get; set; }
        public static string DefaultActionName { get; set; }

        public DashedRoutingConvention(string defaultControllerName, string defaultActionName)
        {
            DefaultActionName = defaultActionName;
            DefaultControllerName = defaultControllerName;
        }

        public void Apply(ControllerModel controller)
        {
            string kebabControllerName = PascalToKebabCase(controller.ControllerName);

            foreach (var action in controller.Actions)
            {
                string kebabActionName = PascalToKebabCase(action.ActionName);

                var defaultSelectors = action.Selectors.FirstOrDefault(a => a.AttributeRouteModel == null);
                if (defaultSelectors != null) action.Selectors.Remove(defaultSelectors);

                SetSelectors(action, kebabControllerName + "/" + kebabActionName);

                if (action.ActionName == DefaultActionName)
                {
                    SetSelectors(action, kebabControllerName);
                }

                if (action.ActionName == DefaultActionName && controller.ControllerName == DefaultControllerName)
                {
                    SetSelectors(action, "");
                }
            }
        }


        private void SetSelectors(ActionModel action, string selectors)
        {
            action.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = selectors
                }
            });
        }
        private static string PascalToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
    }
}
