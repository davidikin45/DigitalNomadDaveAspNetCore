﻿using DND.Common.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Common.DynamicForms
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
            if (!bindingContext.ModelType.IsAssignableFrom(typeof(DynamicForm)))
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
            var propertiesRuntime2 = ((CustomModelMetadata)bindingContext.ModelMetadata).PropertiesRuntime(bindingContext.Model);
            var propertiesRuntime = ((CustomModelMetadata)_modelBinderProviderContext.Metadata).PropertiesRuntime(bindingContext.Model);
            for (var i = 0; i < propertiesRuntime.Count; i++)
            {
                var property = propertiesRuntime[i];
                propertyBinders.Add(property, _modelBinderProviderContext.CreateBinder(property));
            }

            var complexModelBinder = new CustomComplexTypeModelBinder(propertyBinders);

            await complexModelBinder.BindModelAsync(bindingContext);
        }

        protected async Task<object> CreateModelAsync(ModelBindingContext bindingContext)
        {
            var formSlugResult = bindingContext.ValueProvider.GetValue(DynamicFormsValueProviderKeys.FormUrlSlug);
            var sectionSlugResult = bindingContext.ValueProvider.GetValue(DynamicFormsValueProviderKeys.SectionUrlSlug);

            if (formSlugResult == ValueProviderResult.None)
            {
                return null;
            }

            var formSlug = formSlugResult.FirstValue;
            string sectionSlug = default(string);
            if(sectionSlugResult != ValueProviderResult.None)
            {
                sectionSlug = sectionSlugResult.FirstValue;
            }

            var model = await _dynamicFormsPresentationService.CreateFormModelFromDbAsync(formSlug, sectionSlug);

            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = DynamicFormsValueProviderKeys.FormSubmissionId;
            }

            if (model != null)
            {
                string formSubmissionId = default(string);
                var formSubmissionIdResult = bindingContext.ValueProvider.GetValue(modelName);
                if (formSubmissionIdResult != ValueProviderResult.None)
                {
                    formSubmissionId = formSubmissionIdResult.FirstValue;
                }

                await _dynamicFormsPresentationService.PopulateFormModelFromDbAsync(model, formSubmissionId, sectionSlug);
            }

            return model;
        }     
    }

    public class CustomComplexTypeModelBinder : ComplexTypeModelBinder
    {
        private IDictionary<ModelMetadata, IModelBinder> _propertyBinders;
        public CustomComplexTypeModelBinder(IDictionary<ModelMetadata, IModelBinder> propertyBinders)
            :base(propertyBinders)
        {
            _propertyBinders = propertyBinders;
        }

        protected override Task BindProperty(ModelBindingContext bindingContext)
        {
            var propertyBinder = _propertyBinders.First(pb => ((CustomModelMetadata)pb.Key).Identity.Name == ((CustomModelMetadata)bindingContext.ModelMetadata).Identity.Name).Value;
            return propertyBinder.BindModelAsync(bindingContext);
        }

        protected override void SetProperty(ModelBindingContext bindingContext, string modelName, ModelMetadata propertyMetadata, ModelBindingResult result)
        {
            base.SetProperty(bindingContext, modelName, propertyMetadata, result);
        }
    }
}
