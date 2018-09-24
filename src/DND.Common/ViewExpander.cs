using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common
{
    public class ViewExpander : IViewLocationExpander
    {
        private string MvcImplementationFolder { get; set; }

        public ViewExpander(string MvcImplementationFolder)
        {
            MvcImplementationFolder = MvcImplementationFolder;
        }

        private string[] Locations()
        {
            string[] locations = {
                "~/" + MvcImplementationFolder + "{1}/Views/{0}.cshtml",

                "~/" + MvcImplementationFolder + "Shared/Views/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Shared/Views/Bundles/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Shared/Views/Sidebar/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Shared/Views/CRUD/{0}.cshtml",
                 "~/" + MvcImplementationFolder + "Shared/Views/Navigation/{0}.cshtml",
                 "~/" + MvcImplementationFolder + "Shared/Views/Footer/{0}.cshtml",
                 "~/" + MvcImplementationFolder + "Shared/Views/Alerts/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Shared/Views/CookieConsent/{0}.cshtml",

                "~/" + MvcImplementationFolder + "Views/Shared/Bundles/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Views/Shared/Sidebar/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Views/Shared/CRUD/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Views/Shared/Navigation/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Views/Shared/Footer/{0}.cshtml",
                 "~/" + MvcImplementationFolder + "Views/Shared/Alerts/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Views/Shared/CookieConsent/{0}.cshtml"
            };

            return locations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var appSettings = (IConfiguration)context.ActionContext.HttpContext
                              .RequestServices.GetService(typeof(IConfiguration));

            context.Values["ActiveViewTheme"] = appSettings["AppSettings:ActiveViewTheme"];
        }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //Replace folder view with CustomViews
            var locations = viewLocations.Union(Locations());

            var activeViewTheme = context.Values["ActiveViewTheme"];
            if (!string.IsNullOrWhiteSpace(activeViewTheme))
            {
                var expandedLocations = locations.ToList();

                for (int i = 0; i < locations.Count(); i++)
                {
                    expandedLocations.Insert(i, locations.ElementAt(i)
                        .Replace("/Views/", string.Format("/Views/Themes/{0}/", activeViewTheme)));
                }

                return expandedLocations;
            }
            else
            {
                return locations;
            }
        }
    }
}
