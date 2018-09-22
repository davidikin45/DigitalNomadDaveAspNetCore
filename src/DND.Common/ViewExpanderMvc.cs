using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common
{
    //Functional instead of the standard categorical organization
    public class ViewExpanderMvc : IViewLocationExpander
    {
        private string[] Locations()
        {
            string[] locations = {
                "~/Mvc/{1}/Views/{0}.cshtml",
                "~/Mvc/Shared/Views/{0}.cshtml"
            };

            return locations;
        }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Union(Locations());
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
