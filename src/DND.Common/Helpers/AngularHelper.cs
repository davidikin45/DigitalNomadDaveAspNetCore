using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using HtmlTags;
using Humanizer;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DND.Common.Helpers
{
    public static class AngularHelperExtension
    {
        public static AngularHelper<TModel> Angular<TModel>(this IHtmlHelper<TModel> helper)
        {
            return new AngularHelper<TModel>(helper);
        }
    }

    public class AngularHelper<TModel>
    {
        private readonly IHtmlHelper<TModel> _htmlHelper;


        public AngularHelper(IHtmlHelper<TModel> helper)
        {
            _htmlHelper = helper;
        }

        public AngularModelHelper<TModel> ModelFor(string expressionPrefix)
        {
            return new AngularModelHelper<TModel>(_htmlHelper, expressionPrefix);
        }

        public HtmlString BindingForModelUsingTemplates()
        {
            var prefix = (string)(_htmlHelper.ViewBag.Prefix);

            if (prefix != null)
            {
                prefix = prefix + ".";
            }

            var input = _htmlHelper.IdForModel().ToString();
            var camelCase = input.ConvertFullNameToCamelCase();
            return new HtmlString(prefix + camelCase);
        }

        public HtmlTag FormForModel(string expressionPrefix)
        {
            var modelHelper = ModelFor(expressionPrefix);

            var formGroupForMethodGeneric = typeof(AngularModelHelper<TModel>)
                .GetMethod("FormGroupForGeneric");

            var wrapperTag = new HtmlTag("div").NoTag();

            foreach (var prop in typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttributes().OfType<HiddenInputAttribute>().Any()) continue;

                var formGroupForProp = formGroupForMethodGeneric
                    .MakeGenericMethod(prop.PropertyType);

                var propertyLambda = MakeLambda(prop);

                var formGroupTag = (HtmlTag)formGroupForProp.Invoke(modelHelper,
                    new[] { propertyLambda });

                wrapperTag.Append(formGroupTag);
            }

            return wrapperTag;
        }

        //Constructs a lambda of the form x => x.PropName
        private object MakeLambda(PropertyInfo prop)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            var property = Expression.Property(parameter, prop);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(TModel), prop.PropertyType);

            //x => x.PropName
            return Expression.Lambda(funcType, property, parameter);
        }

    }

    public class AngularModelHelper<TModel>
    {
        protected readonly IHtmlHelper Helper;
        private readonly string _expressionPrefix;

        public AngularModelHelper(IHtmlHelper helper, string expressionPrefix)
        {
            Helper = helper;
            _expressionPrefix = expressionPrefix;
        }

        public IHtmlContent EditorForModelUsingTemplates()
        {
            return Helper.EditorForModel("Angular/Object",
                new { Prefix = _expressionPrefix });
        }

        /// <summary>
        /// Converts an lambda expression into a camel-cased string, prefixed
        /// with the helper's configured prefix expression, ie:
        /// vm.model.parentProperty.childProperty
        /// </summary>
        public HtmlString ExpressionFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var expressionText = ExpressionForInternal(property);
            return new HtmlString(expressionText);
        }

        /// <summary>
        /// Converts a lambda expression into a camel-cased AngularJS binding expression, ie:
        /// {{vm.model.parentProperty.childProperty}} 
        /// </summary>
        public HtmlString BindingFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return new HtmlString("{{" + ExpressionForInternal(property) + "}}");
        }

        /// <summary>
        /// Creates a div with an ng-repeat directive to enumerate the specified property,
        /// and returns a new helper you can use for strongly-typed bindings on the items
        /// in the enumerable property.
        /// </summary>
        public AngularNgRepeatHelper<TSubModel> Repeat<TSubModel>(
            Expression<Func<TModel, IEnumerable<TSubModel>>> property, string variableName)
        {
            var propertyExpression = ExpressionForInternal(property);
            return new AngularNgRepeatHelper<TSubModel>(
                Helper, variableName, propertyExpression);
        }

        public string ExpressionForInternal<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var camelCaseName = property.ToCamelCaseName();

            var expression = !string.IsNullOrEmpty(_expressionPrefix)
                ? _expressionPrefix + "." + camelCaseName
                : camelCaseName;

            return expression;
        }

        private string ExpressionForFormFieldTouched<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return ExpressionForFormField(property, "$touched");
        }

        private string ExpressionForFormFieldValid<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return ExpressionForFormField(property, "$valid");
        }

        private string ExpressionForFormField<TProp>(Expression<Func<TModel, TProp>> property, string method)
        {
            var camelCaseName = property.ToCamelCaseName();

            var expression = ExpressionForForm(camelCaseName + "." + method);

            return expression;
        }

        private string ExpressionForFormSubmitted()
        {
            var expression = ExpressionForForm("$submitted");

            return expression;
        }

        private string ExpressionForForm(string method)
        {
            var expression = "form." + method;

            return expression;
        }

        public HtmlTag EnumDropdownFor<TProp>(Expression<Func<TModel, TProp>> property, bool showLabel = true, int xs = 0, int sm = 0, int md = 0, int lg = 0, int xl = 0)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(property, new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()), Helper.MetadataProvider);
            Type propertyType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;
            PropertyInfo propertyInfo = metadata.Container.ModelType.GetProperty(metadata.Metadata.PropertyName);
            EnumDatasourceAttribute[] enumAttributes = propertyInfo.GetCustomAttributes(typeof(EnumDatasourceAttribute), false) as EnumDatasourceAttribute[];

            var name = Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.ExpressionHelper.GetExpressionText(property);

            Boolean valueAsString = false;
            if (enumAttributes.Length > 0)
            {
                propertyType = enumAttributes[0].Datasource;
                valueAsString = true;
            }

            var expression = ExpressionForInternal(property);

            var formGroup = CommonFormGroupFor(property, showLabel, xs, sm, md, lg, xl);

            IEnumerable<Object> enumValues = Enum.GetValues(propertyType).Cast<Object>();
            IEnumerable<SelectListItem> items = from enumValue in enumValues
                                                select new SelectListItem
                                                {
                                                    Text = GetText(enumValue),
                                                    Value = valueAsString ? enumValue.ToString() : ((Int32)enumValue).ToString(),
                                                    Selected = enumValue.Equals(metadata.Model)
                                                };

            var select = new HtmlTag("select").AddClass("form-control")
                  .Attr("ng-model", expression)
                    .Attr("name", name);

            formGroup.Append(select);

            int i = 0;
            foreach (SelectListItem item in items)
            {
                var option = new HtmlTag("option")
                 .Attr("value", item.Value)
                 .AppendHtml(item.Text);

                if (item.Selected)
                {
                    option.Attr("selected");

                }

                select.Append(option);

                i++;
            }

            ApplyHelpToFormGroup(formGroup, metadata.Metadata);

            return formGroup;

        }


        public HtmlTag EnumRadioButtonListFor<TProp>(Expression<Func<TModel, TProp>> property, Boolean inline, int xs = 0, int sm = 0, int md = 0, int lg = 0, int xl = 0)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(property, new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()), Helper.MetadataProvider);
            Type propertyType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;
            PropertyInfo propertyInfo = metadata.Metadata.ContainerType.GetProperty(metadata.Metadata.PropertyName);
            EnumDatasourceAttribute[] enumAttributes = propertyInfo.GetCustomAttributes(typeof(EnumDatasourceAttribute), false) as EnumDatasourceAttribute[];

            var name = Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.ExpressionHelper.GetExpressionText(property);

            Boolean valueAsString = false;
            if (enumAttributes.Length > 0)
            {
                propertyType = enumAttributes[0].Datasource;
                valueAsString = true;
            }

            var expression = ExpressionForInternal(property);

            var formGroup = CommonFormGroupFor(property, false, xs, sm, md, lg, xl);

            IEnumerable<Object> enumValues = Enum.GetValues(propertyType).Cast<Object>();
            IEnumerable<SelectListItem> items = from enumValue in enumValues
                                                select new SelectListItem
                                                {
                                                    Text = GetText(enumValue),
                                                    Value = valueAsString ? enumValue.ToString() : ((Int32)enumValue).ToString(),
                                                    Selected = (metadata.Model != null && enumValue.Equals(metadata.Model))
                                                };

            int i = 0;
            foreach (SelectListItem item in items)
            {

                var div = new HtmlTag("div").AddClass("form-check");

                var radioLabel = new HtmlTag("label");
                if (inline)
                {
                    radioLabel.AddClass("form-check-inline");
                }
                else
                {
                    radioLabel.AddClass("form-check-label");
                }

                var input = new HtmlTag("input")
                 .AddClasses("form-check-input")
                 .Attr("ng-model", expression)
                 .Attr("name", name)
                 .Attr("value", item.Value)
                 .Attr("type", "radio");

                radioLabel.Append(input).AppendHtml(" " + item.Text);
                div.Append(radioLabel);

                if (inline)
                {
                    formGroup.Append(radioLabel);
                }
                else
                {
                    formGroup.Append(div);
                }

                i++;
            }

            ApplyHelpToFormGroup(formGroup, metadata.Metadata);

            return formGroup;

        }

        private static String GetText(Object e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());
            DisplayAttribute[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            return null != displayAttributes && displayAttributes.Length > 0 ? displayAttributes[0].Name : e.ToString();
        }

        public HtmlTag FormGroupForGeneric<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return FormGroupFor(property, true, 12, 12, 12, 12, 12, null, null);
        }

        public HtmlTag FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property, bool showLabel = true, int xs = 0, int sm = 0, int md = 0, int lg = 0, int xl = 0, string dependentBinding = null, string typeAhead = null)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(property, new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()), Helper.MetadataProvider);
            Type propertyType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;

            var name = Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.ExpressionHelper.GetExpressionText(property);
            var expression = ExpressionForInternal(property);

            var touched = ExpressionForFormFieldTouched(property);
            var valid = ExpressionForFormFieldTouched(property);
            var submitted = ExpressionForFormSubmitted();

            var formGroup = CommonFormGroupFor(property, showLabel, xs, sm, md, lg, xl, dependentBinding);

            var labelText = metadata.Metadata.DisplayName ?? name.Humanize(LetterCasing.Title);

            if (propertyType.IsEnum && !showLabel)
            {
                IEnumerable<Object> enumValues = Enum.GetValues(propertyType).Cast<Object>();
                IEnumerable<SelectListItem> items = from enumValue in enumValues
                                                    select new SelectListItem
                                                    {
                                                        Text = GetText(enumValue),
                                                        Value = ((Int32)enumValue).ToString(),
                                                        Selected = enumValue.Equals(metadata.Model)
                                                    };

                int i = 0;
                foreach (SelectListItem item in items)
                {
                    var radioLabel = new HtmlTag("label")
                  .AddClass("form-check-inline");

                    var input = new HtmlTag("input")
                     .AddClasses("form-check-input")
                     .Attr("ng-model", expression)
                     .Attr("name", name)
                     .Attr("value", item.Value)
                     .Attr("type", "radio");

                    radioLabel.Append(input).AppendHtml(" " + item.Text);

                    formGroup.Append(radioLabel);
                    i++;
                }

            }
            else if (propertyType.IsEnum && showLabel)
            {

                var select = new HtmlTag("select").AddClass("form-control")
                  .Attr("ng-model", expression)
                    .Attr("name", name);

                formGroup.Append(select);

                IEnumerable<Object> enumValues = Enum.GetValues(propertyType).Cast<Object>();
                IEnumerable<SelectListItem> items = from enumValue in enumValues
                                                    select new SelectListItem
                                                    {
                                                        Text = GetText(enumValue),
                                                        Value = ((Int32)enumValue).ToString(),
                                                        Selected = (metadata.Model != null && enumValue.Equals(metadata.Model))
                                                    };

                int i = 0;
                foreach (SelectListItem item in items)
                {
                    var option = new HtmlTag("option")
                     .Attr("value", item.Value)
                     .AppendHtml(item.Text);

                    if (i == 0)
                    {
                        option.Attr("selected");

                    }

                    select.Append(option);

                    i++;
                }
            }
            else if (metadata.ModelType == typeof(Boolean))
            {

                var checkLabel = new HtmlTag("label")
                .AddClass("form-check-inline");

                var input = new HtmlTag("input")
                 .AddClasses("form-check-input")
                 .Attr("ng-model", expression)
                 .Attr("name", name)
                 .Attr("value", false)
                 .Attr("type", "checkbox");

                checkLabel.Append(input).AppendHtml(" " + labelText);

                formGroup.Append(checkLabel);
            }
            else
            {
                formGroup
                .Attr("form-group-validation", name);

                string tagName = "input";

                switch (metadata.Metadata.DataTypeName)
                {
                    case "MultilineText":
                    case "Html":
                        tagName = "textarea";
                        break;
                }

                var placeholder = metadata.Metadata.Placeholder ??
                                  ((metadata.Metadata.DisplayName ?? name.Humanize(LetterCasing.Title)) + "...");
                //Creates <input ng-model="expression"
                //		   class="form-control" name="Name" type="text" >
                var input = new HtmlTag(tagName)
                    .AddClasses("form-control", "form-control-success", "form-control-danger")
                    .Attr("ng-model", expression)
                    .Attr("name", name)
                    .Attr("type", "text")
                    .Attr("placeholder", placeholder);



                if (typeAhead != null)
                {
                    input.Attr("typeahead", typeAhead);
                    input.Attr("ng-model-options", "{debounce: {default: 500, blur: 250 }, getterSetter: true}");
                    input.Attr("typeahead-editable", "false");
                    input.Attr("typeahead-min-length", "0");
                    input.Attr("autocomplete", "off");

                }


                ApplyValidationToInput(input, metadata.Metadata, name, submitted, touched, valid);

                formGroup
                    .Append(input);
            }

            ApplyHelpToFormGroup(formGroup, metadata.Metadata);

            return formGroup;
        }

        private HtmlTag CommonFormGroupFor<TProp>(Expression<Func<TModel, TProp>> property, bool showLabel = true, int xs = 0, int sm = 0, int md = 0, int lg = 0, int xl = 0, string dependentBinding = null)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(property, new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()), Helper.MetadataProvider);
            Type propertyType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;
            var name = Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.ExpressionHelper.GetExpressionText(property);


            var expression = ExpressionForInternal(property);

            var touched = ExpressionForFormFieldTouched(property);
            var valid = ExpressionForFormFieldTouched(property);
            var submitted = ExpressionForFormSubmitted();

            //Creates <div class="form-group has-feedback"
            //				form-group-validation="Name">
            var formGroup = new HtmlTag("div")
                .AddClasses("form-group");

            if (xs != 0) formGroup.AddClass("col-" + xs.ToString());
            else formGroup.AddClass("hidden-xs-down");
            if (sm != 0) formGroup.AddClass("col-sm-" + sm.ToString());
            else formGroup.AddClass("hidden-sm-down");
            if (md != 0) formGroup.AddClass("col-md-" + md.ToString());
            else formGroup.AddClass("hidden-md-down");
            if (lg != 0) formGroup.AddClass("col-lg-" + lg.ToString());
            else formGroup.AddClass("hidden-lg-down");
            if (xl != 0) formGroup.AddClass("col-xl-" + xl.ToString());
            else formGroup.AddClass("hidden-xl-down");


            if (dependentBinding != null)
            {
                formGroup.Attr("ng-hide", "!" + dependentBinding);
            }

            var labelText = metadata.Metadata.DisplayName ?? name.Humanize(LetterCasing.Title);


            //Creates <label class="form-control-label" for="Name">Name</label>
            var label = new HtmlTag("label")
                .AddClass("col-form-label")
                .Attr("for", name)
                .Text(labelText);

            if (!showLabel)
            {
                label.AddClass("sr-only");
            }

            formGroup
            .Append(label);

            return formGroup;
        }

        private void ApplyHelpToFormGroup(HtmlTag formGroup, ModelMetadata metadata)
        {
            var help = metadata.Description;
            if (help != null && help != "")
            {
                var helpTag = new HtmlTag("small")
                .AddClasses("form-text", "text-muted")
                .Text(help);

                formGroup
                .Append(helpTag);
            }

        }

        private void ApplyValidationToInput(HtmlTag input, ModelMetadata metadata, string name, string submitted, string touched, string valid)
        {
            if (metadata.IsRequired)
                input.Attr("required", "");

            switch (metadata.DataTypeName)
            {
                case "DateTime":
                    input.Attr("type", "datetime-local");
                    break;
                case "Date":
                    //input.Attr("is-open", "true");
                    //input.Attr("uib-datepicker-popup", "");
                    input.Attr("type", "date");
                    break;
                case "Time":
                    input.Attr("type", "time");
                    break;
                case "EmailAddress":
                    input.Attr("type", "email");
                    break;
                case "PhoneNumber":
                    input.Attr("pattern", @"[\ 0-9()-]+");
                    break;
                case "Password":
                    input.Attr("type", "password");
                    break;
                case "Url":
                case "ImageUrl":
                    input.Attr("pattern", @"https ?:\/\/ (www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");
                    break;

                case "Upload":
                    input.Attr("type", "file");
                    break;
            }


        }

    }

    public class AngularNgRepeatHelper<TModel> : AngularModelHelper<TModel>, IDisposable
    {
        public AngularNgRepeatHelper(IHtmlHelper helper,
            string variableName, string propertyExpression)
            : base(helper, variableName)
        {
            var div = new HtmlTag("div");
            div.Attr("ng-repeat",
                string.Format("{0} in {1}", variableName, propertyExpression));
            div.NoClosingTag();

            Helper.ViewContext.Writer.Write(div.ToString());
        }

        void IDisposable.Dispose()
        {
            Helper.ViewContext.Writer.Write("</div>");
        }
    }
}
