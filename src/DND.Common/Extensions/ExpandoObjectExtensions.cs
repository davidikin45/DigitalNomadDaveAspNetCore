using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DND.Common.Extensions
{
    public static class ExpandoObjectExtensions
    {
        //Automapper works with ExpandoObject rather than JObject
        public static ExpandoObject ToExpando(this string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IDictionary<string, object> dictionary = serializer.Deserialize<IDictionary<string, object>>(json);
            return dictionary.Expando();
        }

        public static ExpandoObject Expando(this IDictionary<string, object> dictionary)
        {
            ExpandoObject expandoObject = new ExpandoObject();
            IDictionary<string, object> objects = expandoObject;

            foreach (var item in dictionary)
            {
                bool processed = false;

                if (item.Value is IDictionary<string, object>)
                {
                    objects.Add(item.Key, Expando((IDictionary<string, object>)item.Value));
                    processed = true;
                }
                else if (item.Value is ICollection)
                {
                    List<object> itemList = new List<object>();

                    foreach (var item2 in (ICollection)item.Value)

                        if (item2 is IDictionary<string, object>)
                            itemList.Add(Expando((IDictionary<string, object>)item2));
                        else
                            itemList.Add(Expando(new Dictionary<string, object> { { "Unknown", item2 } }));

                    if (itemList.Count > 0)
                    {
                        objects.Add(item.Key, itemList);
                        processed = true;
                    }
                }

                if (!processed)
                    objects.Add(item);
            }

            return expandoObject;
        }
    }
}
