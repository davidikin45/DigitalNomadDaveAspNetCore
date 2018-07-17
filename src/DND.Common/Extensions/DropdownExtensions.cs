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

namespace DND.Common.Extensions
{
    public static class DropdownExtensions
    {
        public static IList<SelectListItem> GetSelectListFromDatabase<TIDbContext>(this IHtmlHelper htmlHelper, string propertyName) where TIDbContext : IBaseDbContext
        {
            var modelExplorer = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata metadata = modelExplorer.Metadata;

            var modelType = ((Type)metadata.AdditionalValues["ModelType"]);
            var keyProperty = ((string)metadata.AdditionalValues["KeyProperty"]);
            var valueProperty = ((string)metadata.AdditionalValues["ValueProperty"]);
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
                    if(!string.IsNullOrWhiteSpace(bindingProperty))
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
                   Text = GetValueString(item, valueProperty).Replace(physicalFolderPath, ""),
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
                   Text = GetValueString(item, valueProperty),
                   Value = item.GetPropValue(keyProperty) != null ? item.GetPropValue(keyProperty).ToString().Replace(physicalFilePath, "") : "",
                   Selected = item.GetPropValue(keyProperty) != null && ids.Contains(item.GetPropValue(keyProperty).ToString().Replace(physicalFilePath, ""))
               }));
            }
            else if(metadata.DataTypeName == "ModelRepeater")
            {
                ((IEnumerable)modelExplorer.Model).Cast<Object>().ToList().ForEach(item =>

                    items.Add(new SelectListItem()
                    {
                        Text = GetValueString(item, valueProperty),
                        Value = item.GetPropValue(keyProperty) != null ? item.GetPropValue(keyProperty).ToString() : "",
                        Selected = item.GetPropValue(keyProperty) != null && ids.Contains(item.GetPropValue(keyProperty).ToString())
                    }));
            }
            else
            {
                using (var db = htmlHelper.Database<TIDbContext>())
                {

                    var pi = modelType.GetProperty(orderByProperty);
                    IEnumerable<Object> query = null;

                    if (orderByType == "asc")
                    {
                        query = (db.Queryable(modelType)).ToList().OrderBy(x => pi.GetValue(x, null));
                    }
                    else
                    {
                        query = (db.Queryable(modelType)).ToList().OrderByDescending(x => pi.GetValue(x, null));
                    }

                    query.ToList().ForEach(item =>

                    items.Add(new SelectListItem()
                    {
                        Text = GetValueString(item, valueProperty),
                        Value = item.GetPropValue(keyProperty) != null ? item.GetPropValue(keyProperty).ToString() : "",
                        Selected = item.GetPropValue(keyProperty) != null && ids.Contains(item.GetPropValue(keyProperty).ToString())
                    }));
                }
            }

            if (metadata.IsNullableValueType || nullable)
            {
                items.Insert(0, new SelectListItem { Text = "", Value = "" });
            }

            return items;
        }

        private static string GetValueString(object obj, string format)
        {
            string value = format;

            if (!value.Contains("{") && !value.Contains(" "))
            {
                value = "{" + value + "}";
            }

            bool found = false;

            foreach (var property in obj.GetProperties())
            {
                if (value.Contains("{" + property.Name + "}"))
                {
                    found = true;
                    value = value.Replace("{" + property.Name + "}", obj.GetPropValue(property.Name) != null ? obj.GetPropValue(property.Name).ToString() : "");
                }
            }

            if (!found)
            {
                value = "";
            }

            return value;
        }

        public static IHtmlContent DropDownListFromDatabase<TIDbContext>(this IHtmlHelper htmlHelper, string propertyName, object htmlAttributes = null) where TIDbContext : IBaseDbContext
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
