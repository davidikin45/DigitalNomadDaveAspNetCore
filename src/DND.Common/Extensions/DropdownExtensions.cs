using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using DND.Common.Helpers;
using DND.Common.Interfaces.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using HtmlTags;

namespace DND.Common.Extensions
{
    public static class DropdownExtensions
    {
        public static IList<SelectListItem> GetSelectListFromDatabase<TIDbContext>(this IHtmlHelper<dynamic> htmlHelper, string propertyName, bool selectedOnly = false) where TIDbContext : IBaseDbContext
        {
            var modelExplorer = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = modelExplorer.Metadata;

            var dropdownModelType = ((Type)metadata.AdditionalValues["DropdownModelType"]);
            var keyProperty = ((string)metadata.AdditionalValues["KeyProperty"]);
            var valueProperty = ((string)metadata.AdditionalValues["DisplayExpression"]);
            var bindingProperty = ((string)metadata.AdditionalValues["BindingProperty"]);

            var orderByProperty = ((string)metadata.AdditionalValues["OrderByProperty"]);
            var orderByType = ((string)metadata.AdditionalValues["OrderByType"]);

            var physicalFilePath = ((string)metadata.AdditionalValues["PhysicalFilePath"]);
            var physicalFolderPath = ((string)metadata.AdditionalValues["PhysicalFolderPath"]);

            var nullable = ((bool)metadata.AdditionalValues["Nullable"]);

            Type propertyType = GetNonNullableModelType(metadata);
            List<SelectListItem> items = new List<SelectListItem>();
            List<string> ids = new List<string>();

            if (propertyType != typeof(string) && (propertyType.GetInterfaces().Contains(typeof(IEnumerable))))
            {
                if (modelExplorer.Model != null)
                {
                    foreach (var val in (IEnumerable)modelExplorer.Model)
                    {
                        if (val != null)
                        {
                            if (!string.IsNullOrWhiteSpace(bindingProperty))
                            {
                                ids.Add(val.GetPropValue(bindingProperty).ToString());
                            }
                            else
                            {
                                ids.Add(val.ToString());
                            }
                        }
                    }
                }
            }
            else
            {
                if (modelExplorer.Model != null)
                {
                    if (!string.IsNullOrWhiteSpace(bindingProperty))
                    {
                        ids.Add(modelExplorer.Model.GetPropValue(bindingProperty).ToString());
                    }
                    else
                    {
                        ids.Add(modelExplorer.Model.ToString());
                    }
                }
            }

            if (!string.IsNullOrEmpty(physicalFolderPath))
            {
                var repository = htmlHelper.FileSystemGenericRepositoryFactory().CreateFolderRepositoryReadOnly(default(CancellationToken), physicalFolderPath, true);
                var data = repository.GetAll(LamdaHelper.GetOrderByFunc<DirectoryInfo>(orderByProperty, orderByType), null, null);
                keyProperty = nameof(DirectoryInfo.FullName);

                data.ToList().ForEach(item =>

               items.Add(new SelectListItem()
               {
                   Text = GetDisplayString(htmlHelper, item, valueProperty).Replace(physicalFolderPath, ""),
                   Value = item.GetPropValue(nameof(DirectoryInfo.FullName)) != null ? item.GetPropValue(nameof(DirectoryInfo.FullName)).ToString().Replace(physicalFolderPath, "") : "",
                   Selected = item.GetPropValue(keyProperty) != null && ids.Contains(item.GetPropValue(keyProperty).ToString().Replace(physicalFolderPath, ""))
               }));
            }
            else if (!string.IsNullOrEmpty(physicalFilePath))
            {
                var repository = htmlHelper.FileSystemGenericRepositoryFactory().CreateFileRepositoryReadOnly(default(CancellationToken), physicalFilePath, true);
                var data = repository.GetAll(LamdaHelper.GetOrderByFunc<FileInfo>(orderByProperty, orderByType), null, null);
                keyProperty = nameof(FileInfo.FullName);

                data.ToList().ForEach(item =>

               items.Add(new SelectListItem()
               {
                   Text = GetDisplayString(htmlHelper, item, valueProperty),
                   Value = item.GetPropValue(keyProperty) != null ? item.GetPropValue(keyProperty).ToString().Replace(physicalFilePath, "") : "",
                   Selected = item.GetPropValue(keyProperty) != null && ids.Contains(item.GetPropValue(keyProperty).ToString().Replace(physicalFilePath, ""))
               }));
            }
            else if (metadata.DataTypeName == "ModelRepeater")
            {
                foreach (var item in htmlHelper.ViewData.Model)
                {
                    var itemObject = (Object)item;

                    items.Add(new SelectListItem()
                    {
                        Text = GetDisplayString(htmlHelper, item, valueProperty),
                        Value = itemObject.GetPropValue(keyProperty) != null ? itemObject.GetPropValue(keyProperty).ToString() : "",
                        Selected = itemObject.GetPropValue(keyProperty) != null && ids.Contains(itemObject.GetPropValue(keyProperty).ToString())
                    });
                }
            }
            else
            {
                using (var db = htmlHelper.Database<TIDbContext>())
                {

                    var pi = dropdownModelType.GetProperty(orderByProperty);

                    Type iQueryableType = typeof(IQueryable<>).MakeGenericType(new[] { dropdownModelType });

                    IEnumerable<Object> query = db.Queryable(dropdownModelType);
           
                    if (selectedOnly)
                    {
                        var whereClause = LamdaHelper.SearchForEntityByIds(dropdownModelType, ids.Cast<Object>());
                        query = (IEnumerable<Object>)typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.Where)).MakeGenericMethod(dropdownModelType).Invoke(null, new object[] { query, whereClause });
                    }

                    if (orderByType == "asc")
                    {
                        query = query.ToList().OrderBy(x => pi.GetValue(x, null));
                    }
                    else
                    {
                        query = query.ToList().OrderByDescending(x => pi.GetValue(x, null));
                    }

                    query.ToList().ForEach(item =>

                    items.Add(new SelectListItem()
                    {
                        Text = GetDisplayString(htmlHelper, item, valueProperty),
                        //Value = item.GetPropValue(keyProperty) != null ? item.GetPropValue(keyProperty).ToString() : "",
                        //Selected = item.GetPropValue(keyProperty) != null && ids.Contains(item.GetPropValue(keyProperty).ToString())
                        Value = GetDisplayString(htmlHelper, item, keyProperty),
                        Selected = ids.Contains(GetDisplayString(htmlHelper, item, keyProperty))
                    }));
                }
            }

            if (metadata.IsNullableValueType || nullable)
            {
                items.Insert(0, new SelectListItem { Text = "", Value = "" });
            }

            return items;
        }

        private static string GetDisplayString(IHtmlHelper htmlHelper, dynamic obj, string displayExpression)
        {
            string value = displayExpression;

            if (!value.Contains("{") && !value.Contains(" "))
            {
                value = "{" + value + "}";
            }

            var replacementTokens = GetReplacementTokens(value);
            foreach (var token in replacementTokens)
            {
                var propertyName = token.Substring(1, token.Length - 2);
                var displayString = HtmlHelperExtensions.ToString(ModelHelperExtensions.Display(htmlHelper, obj, propertyName));
                value = value.Replace(token, displayString);
            }

            return value;
        }

        private static List<String> GetReplacementTokens(String str)
        {
            Regex regex = new Regex(@"{(.*?)}", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(str);

            // Results include braces (undesirable)
            return matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }

        public static IHtmlContent DropDownListFromDatabase<TIDbContext>(this IHtmlHelper<dynamic> htmlHelper, string propertyName, object htmlAttributes = null) where TIDbContext : IBaseDbContext
        {
            IList<SelectListItem> items = GetSelectListFromDatabase<TIDbContext>(htmlHelper, propertyName);

            Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider).Metadata;
            Type propertyType = GetNonNullableModelType(metadata);

            if (propertyType != typeof(string) && (propertyType.GetInterfaces().Contains(typeof(IEnumerable))))
            {
                return htmlHelper.ListBox(propertyName, items, htmlAttributes);
            }
            else
            {
                return htmlHelper.DropDownList(propertyName, items, htmlAttributes);
            }
        }

        public static IHtmlContent CheckboxFromDatabase<TIDbContext>(this IHtmlHelper<dynamic> htmlHelper, string propertyName, object htmlAttributes = null) where TIDbContext : IBaseDbContext
        {
            IList<SelectListItem> items = GetSelectListFromDatabase<TIDbContext>(htmlHelper, propertyName);

            Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider).Metadata;
            Type propertyType = GetNonNullableModelType(metadata);

            var sb = new StringBuilder();
            if (propertyType != typeof(string) && (propertyType.GetInterfaces().Contains(typeof(IEnumerable))))
            {
                return htmlHelper.ValueCheckboxList(propertyName, items);
            }
            else
            {
                return htmlHelper.ValueRadioButtonList(propertyName, items);
            }
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
