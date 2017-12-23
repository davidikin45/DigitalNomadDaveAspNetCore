using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DND.Base.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IHtmlContent EnumDropDownListForStringValue<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes = null)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);

            Type enumType = GetNonNullableModelType(metadata);
            Type baseEnumType = Enum.GetUnderlyingType(enumType);
            List<SelectListItem> items = new List<SelectListItem>();


            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                string text = field.Name;
                string value = field.Name;
                bool selected = field.GetValue(null).Equals(metadata.Model);

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

            if (metadata.Metadata.IsNullableValueType)
            {
                items.Insert(0, new SelectListItem { Text = "", Value = "" });
            }

            return HtmlHelperSelectExtensions.DropDownListFor(htmlHelper, expression, items, htmlAttributes);
        }

        private static Type GetNonNullableModelType(ModelExplorer modelExplorer)
        {
            Type realModelType = modelExplorer.ModelType;
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);


            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }

            return realModelType;
        }
    }
}
