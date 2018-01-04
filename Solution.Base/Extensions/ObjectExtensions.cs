using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class ObjectExtensions
    {
        public static string Name<T, TProp>(this T o, Expression<Func<T, TProp>> propertySelector)
        {
            MemberExpression body = (MemberExpression)propertySelector.Body;
            return body.Member.Name;
        }

        public static string GetEnumDescription(this Enum enumValue)
        {
            string enumValueAsString = enumValue.ToString();

            var type = enumValue.GetType();
            FieldInfo fieldInfo = type.GetField(enumValueAsString);
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                var attribute = (DescriptionAttribute)attributes[0];
                return attribute.Description;
            }

            return enumValueAsString;
        }

        public static T1 CopyFrom<T1, T2>(this T1 obj, T2 otherObject)
        where T1 : class
        where T2 : class
        {
            PropertyInfo[] srcFields = otherObject.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            PropertyInfo[] destFields = obj.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in srcFields)
            {
                var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
                if (dest != null && dest.CanWrite)
                    dest.SetValue(obj, property.GetValue(otherObject, null), null);
            }

            return obj;
        }

        public static object GetPropValue(this object obj, string propName)
        {
            if (HasProperty(obj, propName))
            {
                return obj.GetType().GetProperty(propName).GetValue(obj, null);
            }
            return null;
        }

        public static bool HasProperty(this object obj, string propName)
        {
            return obj.GetType().GetProperty(propName) != null;
        }


        public static PropertyInfo[] GetProperties(this object obj)
        {
            return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static Type GetDynamicType(dynamic obj)
        {
            return obj?.GetType();
        }

        public static Type GetCollectionItemType(object collection)
        {
            var type = collection.GetType();
            var ienum = type.GetInterface(typeof(IEnumerable<>).Name);
            return ienum != null
                ? ienum.GetGenericArguments()[0]
                : null;
        }
    }
}
