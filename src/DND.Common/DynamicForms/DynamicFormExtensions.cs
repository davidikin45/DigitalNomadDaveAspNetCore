﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;

namespace DND.Common.DynamicForms
{
    //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-2.1
    public static class DynamicFormExtensions
    {
        public static DynamicFormModel BindData(this DynamicFormModel form, IFormCollection formData, RouteData routeData, IQueryCollection queryStringData)
        {
            foreach (var propertyName in form.GetDynamicMemberNames())
            {

                if(formData != null && formData.ContainsKey(propertyName))
                {
                    bool isCollection = form.IsCollectionProperty(propertyName);
                    foreach (var value in formData[propertyName])
                    {
                        foreach (var csvSplit in value.Split(','))
                        {
                            form[propertyName] = csvSplit.Trim();
                        }
                        if (!isCollection)
                        {
                            break;
                        }
                    }
                }
                else if(routeData != null && routeData.Values.ContainsKey(propertyName))
                {
                    form[propertyName] = routeData.Values[propertyName].ToString().Trim();
                }
                else if (queryStringData != null && queryStringData.ContainsKey(propertyName))
                {
                    bool isCollection = form.IsCollectionProperty(propertyName);
                    foreach (var value in queryStringData[propertyName])
                    {
                        foreach (var csvSplit in value.Split(','))
                        {
                            form[propertyName] = csvSplit.Trim();
                        }
                        if (!isCollection)
                        {
                            break;
                        }
                    }
                }
            }

            form.BindFiles(formData.Files);

            return form;
        }

        private static DynamicFormModel BindFiles(this DynamicFormModel form, IFormFileCollection files)
        {
            foreach (var item in files)
            {
                if (form.ContainsProperty(item.Name))
                {
                    form[item.Name] = (FormFile)item;
                }
            }

            return form;
        }
    }
}
