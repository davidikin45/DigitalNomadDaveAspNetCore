using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Infrastructure
{
    public class CustomViewLocator : IViewLocationExpander
    {
        private static readonly string[] Locations =
       {
        "~/Views/Shared/Sidebar/{0}.cshtml",
        "~/Views/Shared/CRUD/{0}.cshtml",
        "~/Views/Shared/Navigation/{0}.cshtml",
         "~/Views/Shared/Footer/{0}.cshtml",
         "~/Views/Shared/Alerts/{0}.cshtml"
        };

        public void PopulateValues(ViewLocationExpanderContext context)
        { }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //Replace folder view with CustomViews
            return viewLocations.Union(Locations);
        }
    }
}
