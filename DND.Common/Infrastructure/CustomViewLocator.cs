using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure
{
    public class CustomViewLocator : IViewLocationExpander
    {
        private string ImplementationFolder { get; set; }

        public CustomViewLocator(string implementationFolder)
        {
            ImplementationFolder = implementationFolder;
        }

        private string[] Locations()
        {
            string[] locations = {
                "~/" + ImplementationFolder + "{1}/Views/{0}.cshtml",

                "~/" + ImplementationFolder + "Shared/Views/{0}.cshtml",
                "~/" + ImplementationFolder + "Shared/Views/Bundles/{0}.cshtml",
                "~/" + ImplementationFolder + "Shared/Views/Sidebar/{0}.cshtml",
                "~/" + ImplementationFolder + "Shared/Views/CRUD/{0}.cshtml",
                 "~/" + ImplementationFolder + "Shared/Views/Navigation/{0}.cshtml",
                 "~/" + ImplementationFolder + "Shared/Views/Footer/{0}.cshtml",
                 "~/" + ImplementationFolder + "Shared/Views/Alerts/{0}.cshtml",

                "~/" + ImplementationFolder + "Views/Shared/Bundles/{0}.cshtml",
                "~/" + ImplementationFolder + "Views/Shared/Sidebar/{0}.cshtml",
                "~/" + ImplementationFolder + "Views/Shared/CRUD/{0}.cshtml",
                "~/" + ImplementationFolder + "Views/Shared/Navigation/{0}.cshtml",
                "~/" + ImplementationFolder + "Views/Shared/Footer/{0}.cshtml",
                 "~/" + ImplementationFolder + "Views/Shared/Alerts/{0}.cshtml"
            };

            return locations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        { }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //Replace folder view with CustomViews
            return viewLocations.Union(Locations());
        }
    }
}
