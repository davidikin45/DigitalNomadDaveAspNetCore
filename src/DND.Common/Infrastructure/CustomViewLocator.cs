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
        private string MVCImplementationFolder { get; set; }

        public CustomViewLocator(string mvcImplementationFolder)
        {
            MVCImplementationFolder = mvcImplementationFolder;
        }

        private string[] Locations()
        {
            string[] locations = {
                "~/" + MVCImplementationFolder + "{1}/Views/{0}.cshtml",

                "~/" + MVCImplementationFolder + "Shared/Views/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Shared/Views/Bundles/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Shared/Views/Sidebar/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Shared/Views/CRUD/{0}.cshtml",
                 "~/" + MVCImplementationFolder + "Shared/Views/Navigation/{0}.cshtml",
                 "~/" + MVCImplementationFolder + "Shared/Views/Footer/{0}.cshtml",
                 "~/" + MVCImplementationFolder + "Shared/Views/Alerts/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Shared/Views/CookieConsent/{0}.cshtml",

                "~/" + MVCImplementationFolder + "Views/Shared/Bundles/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Views/Shared/Sidebar/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Views/Shared/CRUD/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Views/Shared/Navigation/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Views/Shared/Footer/{0}.cshtml",
                 "~/" + MVCImplementationFolder + "Views/Shared/Alerts/{0}.cshtml",
                "~/" + MVCImplementationFolder + "Views/Shared/CookieConsent/{0}.cshtml"
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
