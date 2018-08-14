using DND.Common.DynamicForms;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace DND.Web.MVCImplementation.DynamicForms.ModelBinders
{
    public class DynamicFormsModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelType = context.Metadata.ModelType;
            if (modelType.IsAssignableFrom(typeof(DynamicFormModel)))
            {
                var dynamicFormsApplicationServices = (IDynamicFormsApplicationServices)context.Services.GetService(typeof(IDynamicFormsApplicationServices));
                return new DynamicFormsModelBinder(context, dynamicFormsApplicationServices);
                //return new BinderTypeModelBinder(typeof(DynamicFormsModelBinder));
                //BinderTypeModelBinder acts as a factory for model binders and provides dependency injection (DI). The AuthorEntityBinder requires DI to access EF Core. Use BinderTypeModelBinder if your model binder requires services from DI.
            }

            return null;
        }
    }
}
