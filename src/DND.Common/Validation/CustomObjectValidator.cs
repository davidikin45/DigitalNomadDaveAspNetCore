using DND.Common.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace DND.Common.Validation
{
    //.Net Core Object validator
    public class CustomObjectValidator : DefaultObjectValidator
    {
        private readonly ICustomModelMetadataProviderSingleton _customModelMetadataProvider;
        public CustomObjectValidator(ICustomModelMetadataProviderSingleton modelMetadataProvider, IList<IModelValidatorProvider> validatorProviders)
            :base(modelMetadataProvider, validatorProviders)
        {
            _customModelMetadataProvider = modelMetadataProvider;
        }

        public override ValidationVisitor GetValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState)
        {
            return new CustomValidationVisitor(
                actionContext,
                validatorProvider,
                validatorCache,
                _customModelMetadataProvider,
                validationState);
        }
    }
}
