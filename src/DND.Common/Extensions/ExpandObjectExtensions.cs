using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class ExpandObjectExtensions
    {
        public static ExpandoObject Add(this ExpandoObject expandoObject, string property, object value)
        {
            var dict = expandoObject as IDictionary<string, object>;
            dict.Add(property, value);
            return expandoObject;
        }
    }
}
