using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IHtmlContent EnumDropDownListForStringValue<TModel, TEnum>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes = null)
        {

            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = modelExplorer.Metadata;

            Type enumType = GetNonNullableModelType(metadata);
            Type baseEnumType = Enum.GetUnderlyingType(enumType);
            List<SelectListItem> items = new List<SelectListItem>();


            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                string text = field.Name;
                string value = field.Name;
                bool selected = field.GetValue(null).Equals(modelExplorer.Model);

                foreach (DisplayAttribute displayAttribute in field.GetCustomAttributes(true).OfType<DisplayAttribute>())
                {
                    text = displayAttribute.GetName();
                }

                items.Add(new SelectListItem()
                {
                    Text = text,
                    Value = value,
                    Selected = selected
                });
            }

            if (metadata.IsNullableValueType)
            {
                items.Insert(0, new SelectListItem { Text = "", Value = "" });
            }

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        private static Type GetNonNullableModelType(Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);


            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }

            return realModelType;
        }

    }
}
