using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DND.Common.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static Dictionary<string, string> ToDictionary<T>()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                string description = field.Name;
                string id = field.Name;

                foreach (DescriptionAttribute descriptionAttribute in field.GetCustomAttributes(true).OfType<DescriptionAttribute>())
                {
                    description = descriptionAttribute.Description;
                }

                dictionary.Add(id, description);
            }

            return dictionary;
        }

        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }
    }
}
