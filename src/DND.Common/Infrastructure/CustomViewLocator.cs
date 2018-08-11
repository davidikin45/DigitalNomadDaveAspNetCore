using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

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
