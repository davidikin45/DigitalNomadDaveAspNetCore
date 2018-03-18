using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Infrastructure
{
    public static class ConfigurationManager
    {
       public static string AppSettings(string key)
        {
            return StaticProperties.Configuration.GetValue<string>("Settings:" + key);
        }
    }
}
