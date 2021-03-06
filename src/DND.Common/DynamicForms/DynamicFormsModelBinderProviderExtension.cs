﻿using Microsoft.AspNetCore.Mvc;

namespace DND.Common.DynamicForms
{
    //https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-model-binding?view=aspnetcore-2.1
    public static class DynamicFormsModelBinderProviderExtension
    {
        public static void UseDynamicFormsModelBinding(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new DynamicFormsModelBinderProvider());
        }
    }
}
