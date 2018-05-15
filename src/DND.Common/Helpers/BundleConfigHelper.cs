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
using DND.Common.Infrastructure;

namespace DND.Common.Helpers
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

            public dynamic Config
            {
                get { return _config.Value; }
            }

            public dynamic Bundle(string outputFileName)
            {
                foreach (var bundle in Config)
                {
                    if(bundle.outputFileName == outputFileName)
                    {
                        return bundle;
                    }
                }
                return null;
            }
        }
    }
}
