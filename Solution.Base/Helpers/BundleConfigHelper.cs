using Newtonsoft.Json;
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
using Solution.Base.Infrastructure;

namespace Solution.Base.Helpers
{
    public static class BundleConfigHelperExtension
    {
        public static BundleConfigHelper BundleConfig(this IHtmlHelper helper)
        {
            return new BundleConfigHelper();
        }

        public static BundleConfigHelper BundleConfig(this IUrlHelper helper)
        {
            return new BundleConfigHelper();
        }

        public class BundleConfigHelper
        {
            private static Lazy<object> _config = new Lazy<dynamic>(() => JsonConvert.DeserializeObject(File.ReadAllText(Server.MapContentPath("~/bundleconfig.json"))));

            public dynamic Menu
            {
                get { return _config.Value; }
            }
        }
    }
}
