﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using DND.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DND.Common.Extensions
{
    public static class ModelHelperExtensions
    {

        //Model Type
        public static Type ModelType(this ViewDataDictionary viewData)
        {
            return ModelType(viewData.Model);
        }

        public static Type ModelType(this object model)
        {
            var type = model.GetType();
            var ienum = type.GetInterface(typeof(IEnumerable<>).Name);
            type = ienum != null
              ? ienum.GetGenericArguments()[0]
              : type;
            return type;
        }

        //ModelMetadata
        public static Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata ModelMetadata(this ViewDataDictionary viewData)
        {
            return ModelMetadata(viewData.Model);
        }

        public static Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata ModelMetadata(this ViewDataDictionary viewData, object model)
        {
            return ModelMetadata(model);
        }

        public static Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata ModelMetadata(this object model)
        {
            var type = ModelType(model);
            var modelMetaData = Infrastructure.ModelMetadataProvider.GetMetadataForType(type);
            return modelMetaData;
        }

        //Labels
        public static HtmlString DisplayName(this IHtmlHelper html, object model, string propertyName)
        {
            return DisplayName(model, propertyName);
        }

        public static String EnumDisplayName(this object e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());
            DisplayAttribute[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            return null != displayAttributes && displayAttributes.Length > 0 ? displayAttributes[0].Name : e.ToString();
        }

        public static HtmlString DisplayName(this object model, string propertyName)
        {
            Type type = ModelType(model);
            var modelMetadata = DND.Common.Infrastructure.ModelMetadataProvider.GetMetadataForProperty(type, propertyName);
            var value = modelMetadata.DisplayName ?? modelMetadata.PropertyName;
            return new HtmlString(HtmlEncoder.Default.Encode(value));
        }

        //Display
        public static IHtmlContent Display<T>(this IHtmlHelper html, T model, string propertyName)
        {
            HtmlHelper<T> newHtml = html.For<T>(model);
            return newHtml.Display(propertyName);
        }

        public static IHtmlContent Display(this IHtmlHelper html, dynamic model, string propertyName)
        {
            HtmlHelper<dynamic> newHtml = HtmlHelperExtensions.For(html, model);
            return newHtml.Display(propertyName);
        }

        //Values
        public static HtmlString DisplayTextSimple(this IHtmlHelper html, string propertyName)
        {
            return DisplayTextSimple(html, html.ViewData.Model, propertyName);
        }


        public static HtmlString DisplayTextSimple(this IHtmlHelper html, object model, string propertyName)
        {
            var newViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
            var modelExporer = ExpressionMetadataProvider.FromStringExpression(propertyName, newViewData, html.MetadataProvider);

            string value = "";

            if (modelExporer != null)
            {
                value = modelExporer.GetSimpleDisplayText() ?? string.Empty;
                //if (modelExporer.Metadata.HtmlEncode)
                //{
                //    value = HtmlEncoder.Default.Encode(value);
                //}

            }

            return new HtmlString(value);
        }

        public static HtmlString DisplayFormatString(this object model, string propertyName)
        {
            Type type = ModelType(model);
            var modelMetadata = DND.Common.Infrastructure.ModelMetadataProvider.GetMetadataForType(type);

            var propertyMetadata = (from p in modelMetadata.Properties
                                    where p.PropertyName == propertyName
                                    select p).FirstOrDefault<Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata>();
            string value = "";

            if (propertyMetadata != null)
            {
                value = propertyMetadata.DisplayFormatString;
                if (propertyMetadata.HtmlEncode)
                {
                    value = HtmlEncoder.Default.Encode(value);
                }
            }

            return new HtmlString(value);
        }

    }
}
