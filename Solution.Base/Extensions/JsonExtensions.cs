using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace Solution.Base.Extensions
{
    public static class JsonExtensions
    {
        //public static string ToJson<T>(this T obj, bool includeNull = true)
        //{
        //	var settings = new JsonSerializerSettings
        //	{
        //		ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //		Converters = new JsonConverter[] { new StringEnumConverter() },
        //		NullValueHandling = includeNull ? NullValueHandling.Include : NullValueHandling.Ignore
        //	};

        //	return JsonConvert.SerializeObject(obj, settings);
        //}

        public static string ToXml(this object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.WriteObject(tw, o);
            }
            catch (Exception ex)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(obj));
        }

        public static Dictionary<string, object> DeserializeJsonToDictionary(this string jsonString)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Dictionary<string, object> dictionary = javaScriptSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            try
            {
                Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    System.Collections.Generic.KeyValuePair<string, object> current = enumerator.Current;
                    if (current.Value.GetType() == dictionary.GetType())
                    {
                        dictionary2.Add(current.Key, DeserializeJsonToDictionary((Dictionary<string, object>)current.Value));
                    }
                    else
                    {
                        dictionary2.Add(current.Key, System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(current.Value));
                    }
                }
            }
            finally
            {

            }
            return dictionary2;
        }

        public static Dictionary<string, object> DeserializeJsonToDictionary(Dictionary<string, object> values)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            try
            {
                Dictionary<string, object>.Enumerator enumerator = values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    System.Collections.Generic.KeyValuePair<string, object> current = enumerator.Current;
                    if (current.Value.GetType() == values.GetType())
                    {
                        dictionary.Add(current.Key, DeserializeJsonToDictionary((Dictionary<string, object>)current.Value));
                    }
                    else
                    {
                        dictionary.Add(current.Key, System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(current.Value));
                    }
                }
            }
            finally
            {

            }
            return dictionary;
        }

        public static string ToCamelCaseName<TModel, TProp>(
            this Expression<Func<TModel, TProp>> property)
        {
            //Turns x => x.SomeProperty.SomeValue into "SomeProperty.SomeValue"
            var pascalCaseName = ExpressionHelper.GetExpressionText(property);

            //Turns "SomeProperty.SomeValue" into "someProperty.someValue"
            var camelCaseName = ConvertFullNameToCamelCase(pascalCaseName);
            return camelCaseName;
        }

        //Converts expressions of the form Some.PropertyName to some.propertyName
        public static string ConvertFullNameToCamelCase(this string pascalCaseName)
        {
            if (pascalCaseName is null)
            {
                return pascalCaseName;
            }

            var parts = pascalCaseName.Split('.')
                .Select(ConvertToCamelCase);

            return string.Join(".", parts);
        }

        //Borrowed from JSON.NET. Turns a single name into camel case.
        private static string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (!char.IsUpper(s[0]))
                return s;
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                    break;
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }
            return new string(chars);
        }
    }
}
