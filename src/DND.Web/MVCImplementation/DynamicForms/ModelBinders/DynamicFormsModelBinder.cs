using DND.Common.DynamicForms;
using DND.Common.ModelMetadataCustom.Providers;
using DND.Interfaces.DynamicForms.PresentationServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.DynamicForms.ModelBinders
{
    //[ModelBinder(BinderType = typeof(DynamicFormsModelBinder))] Can add to object OR controller parameter
    public class DynamicFormsModelBinder : IModelBinder
    {
        private readonly IDynamicFormsPresentationService _dynamicFormsPresentationService;
        private readonly ModelBinderProviderContext _modelBinderProviderContext;

        public DynamicFormsModelBinder(ModelBinderProviderContext modelBinderProviderContext,
            IDynamicFormsPresentationService dynamicFormsPresentationService)
        {
            _dynamicFormsPresentationService = dynamicFormsPresentationService;
            _modelBinderProviderContext = modelBinderProviderContext;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            //Binder only works for DynamicFormModel
            if (bindingContext.ModelType.IsAssignableFrom(typeof(DynamicFormModel)))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            return BindModelCoreAsync(bindingContext);
        }

        private async Task BindModelCoreAsync(ModelBindingContext bindingContext)
        {
            bindingContext.Model = await CreateModelAsync(bindingContext);
            var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();
            for (var i = 0; i < ((CustomModelMetadata)_modelBinderProviderContext.Metadata).PropertiesRuntime(bindingContext.Model).Count; i++)
            {
                var property = bindingContext.ModelMetadata.Properties[i];
                propertyBinders.Add(property, _modelBinderProviderContext.CreateBinder(property));
            }

            var complexModelBinder = new ComplexTypeModelBinder(propertyBinders);

            await complexModelBinder.BindModelAsync(bindingContext);
        }

        protected async Task<object> CreateModelAsync(ModelBindingContext bindingContext)
        {
            var formSlugResult = bindingContext.ValueProvider.GetValue("FormSlug");
            var sectionSlugResult = bindingContext.ValueProvider.GetValue("SectionSlug");

            if (formSlugResult == ValueProviderResult.None)
            {
                return null;
            }

            var formSlug = formSlugResult.FirstValue;
            var sectionSlug = "";
            if(sectionSlugResult != ValueProviderResult.None)
            {
                sectionSlug = sectionSlugResult.FirstValue;
            }

            var model = await _dynamicFormsPresentationService.CreateFormModelFromDbAsync(formSlug, sectionSlug);

            //if (model != null)
            //{
            //    string formSubmissionId = "";
            //    var formSubmissionIdResult = bindingContext.ValueProvider.GetValue("FormSubmissionId");
            //    if (formSubmissionIdResult != ValueProviderResult.None)
            //    {
            //        formSubmissionId = formSubmissionIdResult.FirstValue;
            //    }

            //    await _dynamicFormsPresentationService.PopulateFormModelFromDbAsync(model, formSubmissionId, sectionSlug);
            //}

            return model;
        }     
    }
}
