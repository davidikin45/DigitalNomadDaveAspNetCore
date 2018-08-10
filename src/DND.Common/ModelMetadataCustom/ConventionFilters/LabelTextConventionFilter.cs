using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using DND.Common.ModelMetadataCustom.Interfaces;
using Microsoft.Extensions.Options;
using DND.Common.AppSettings;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Common.ModelMetadataCustom.DynamicFormsAttributes;

namespace DND.Common.ModelMetadataCustom.ConventionFilters
{
    public class LabelTextConventionFilter : IDisplayMetadataFilter
    {
        private readonly DisplayConventionsDisableOptions _displayConventionsDisableOptions;
        public LabelTextConventionFilter(IOptions<DisplayConventionsDisableOptions> displayConventionsDisableOptions)
        {
            _displayConventionsDisableOptions = displayConventionsDisableOptions.Value;
        }

        public void TransformMetadata(DisplayMetadataProviderContext context)
        {
            if (!_displayConventionsDisableOptions.LabelText)
            {
                var propertyAttributes = context.Attributes;
                var modelMetadata = context.DisplayMetadata;
                var propertyName = context.Key.Name;


                if (IsTransformRequired(propertyName, modelMetadata, propertyAttributes))
                {

                    modelMetadata.DisplayName = () => GetStringWithSpaces(propertyName);
                }
            }
        }

        private bool IsTransformRequired(string propertyName, DisplayMetadata modelMetadata, IReadOnlyList<object> propertyAttributes)
        {
            if (!string.IsNullOrEmpty(modelMetadata.SimpleDisplayProperty))
                return false;

            if (propertyAttributes.OfType<DisplayNameAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<DisplayAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<SubmitButtonAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<EditLinkAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<IconLinkAttribute>().Any())
                return false;

            if (propertyAttributes.OfType<NoLabelAttribute>().Any())
                return false;

            if (string.IsNullOrEmpty(propertyName))
                return false;

            return true;
        }

        private string GetStringWithSpaces(string input)
        {
            return Regex.Replace(
               input,
               "(?<!^)" +
               "(" +
               "  [A-Z][a-z] |" +
               "  (?<=[a-z])[A-Z] |" +
               "  (?<![A-Z])[A-Z]$" +
               ")",
               " $1",
               RegexOptions.IgnorePatternWhitespace);
        }
    }
}