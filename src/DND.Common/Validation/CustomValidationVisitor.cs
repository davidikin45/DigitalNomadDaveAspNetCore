using DND.Common.ModelMetadataCustom.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Validation
{
    public class CustomValidationVisitor : ValidationVisitor
    {
        public CustomValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, ICustomModelMetadataProviderSingleton metadataProvider, ValidationStateDictionary validationState)
            :base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState)
        { }

        protected override bool Visit(ModelMetadata metadata, string key, object model)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            if (model != null && !CurrentPath.Push(model))
            {
                // This is a cycle, bail.
                return true;
            }

            var entry = GetValidationEntry(model);
            key = entry?.Key ?? key ?? string.Empty;
            metadata = entry?.Metadata ?? metadata;
            var strategy = entry?.Strategy;

            if (ModelState.HasReachedMaxErrors)
            {
                SuppressValidation(key);
                return false;
            }
            else if (entry != null && entry.SuppressValidation)
            {
                // Use the key on the entry, because we might not have entries in model state.
                SuppressValidation(entry.Key);
                CurrentPath.Pop(model);
                return true;
            }

            using (StateManager.Recurse(this, key ?? string.Empty, metadata, model, strategy))
            {
                if (Metadata.IsEnumerableType)
                {
                    return VisitComplexType(DefaultCollectionValidationStrategy.Instance);
                }

                if (Metadata.IsComplexType)
                {
                    return VisitComplexType(CustomComplexObjectValidationStrategy.Instance);
                }

                return VisitSimpleType();
            }
        }
    }
}
