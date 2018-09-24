using DND.Common.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DND.Common.Helpers
{
    public static class NavigationMenuHelperExtension
    {
        public static NavigationMenuHelper NavigationMenu(this IHtmlHelper helper)
        {
            return new NavigationMenuHelper();
        }

        public static NavigationMenuHelper NavigationMenu(this IUrlHelper helper)
        {
            return new NavigationMenuHelper();
        }

        public class NavigationMenuHelper
        {
            public static string MvcImplementationFolder { get; set; }
            private static Lazy<object> _menu = new Lazy<dynamic>(() => JsonConvert.DeserializeObject(File.ReadAllText(Server.MapContentPath("navigation.json"))));

            public dynamic Menu
            {
                get { return _menu.Value; }
            }

            private static Lazy<object> _adminMenu = new Lazy<dynamic>(() => JsonConvert.DeserializeObject(File.ReadAllText(Server.MapContentPath("navigation-admin.json"))));

            public dynamic AdminMenu
            {
                get { return _adminMenu.Value; }
            }
        }
    }
}
