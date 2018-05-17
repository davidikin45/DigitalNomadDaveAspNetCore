﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Infrastructure;

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
            public static string MVCImplementationFolder { get; set; }
            private static Lazy<object> _menu = new Lazy<dynamic>(() => JsonConvert.DeserializeObject(File.ReadAllText(Server.MapContentPath("~/" + MVCImplementationFolder + "Shared/Views/Navigation/navigation.json"))));

            public dynamic Menu
            {
                get { return _menu.Value; }
            }

            private static Lazy<object> _adminMenu = new Lazy<dynamic>(() => JsonConvert.DeserializeObject(File.ReadAllText(Server.MapContentPath("~/" + MVCImplementationFolder + "Shared/Views/Navigation/admin-navigation.json"))));

            public dynamic AdminMenu
            {
                get { return _adminMenu.Value; }
            }
        }
    }
}