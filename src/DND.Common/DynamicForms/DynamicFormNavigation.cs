using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace DND.Common.DynamicForms
{
    public class DynamicFormNavigation
    {
        public List<DynamicFormNavigationMenuItem> MenuItems { get; set; } = new List<DynamicFormNavigationMenuItem>();
    }

    public class DynamicFormNavigationMenuItem
    {
        public string LinkText { get; set; }

        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; } = new RouteValueDictionary();

        public bool IsValid { get; set; }
        public bool Visited { get; set; }

        public DynamicFormNavigation ChildNavigation { get; set; }
    }
}
