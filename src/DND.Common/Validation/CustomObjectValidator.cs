using DND.Common.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Validation
{
    public class CustomObjectValidator : DefaultObjectValidator
    {
        private readonly ICustomModelMetadataProviderSingleton _customNodelMetadataProvider;
        public CustomObjectValidator(ICustomModelMetadataProviderSingleton modelMetadataProvider, IList<IModelValidatorProvider> validatorProviders)
            :base(modelMetadataProvider, validatorProviders)
        {
            _customNodelMetadataProvider = modelMetadataProvider;
        }

        public override ValidationVisitor GetValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState)
        {
            return new CustomValidationVisitor(
                actionContext,
                validatorProvider,
                validatorCache,
                _customNodelMetadataProvider,
                validationState);
        }
    }
}
