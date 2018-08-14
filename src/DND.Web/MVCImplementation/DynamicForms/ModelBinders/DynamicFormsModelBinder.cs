using DND.Common.DynamicForms;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Common.ModelMetadataCustom.Providers;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.DynamicForms.ModelBinders
{
    //[ModelBinder(BinderType = typeof(DynamicFormsModelBinder))] Can add to object OR controller parameter
    public class DynamicFormsModelBinder : IModelBinder
    {
        private readonly IDynamicFormsApplicationServices _dynamicFormsApplicationService;
        private readonly ModelBinderProviderContext _modelBinderProviderContext;

        public DynamicFormsModelBinder(ModelBinderProviderContext modelBinderProviderContext,
            IDynamicFormsApplicationServices dynamicFormsApplicationService)
        {
            _dynamicFormsApplicationService = dynamicFormsApplicationService;
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
            var formIdValue = bindingContext.ValueProvider.GetValue("FormId");
            var sectionIdValue = bindingContext.ValueProvider.GetValue("SectionId");

            if (formIdValue == null || sectionIdValue == null)
            {
                return null;
            }

            int formId;
            if (!Int32.TryParse(formIdValue.FirstValue, out formId) || formId == 0)
            {
                return null;
            }

            int sectionId;
            if (!Int32.TryParse(sectionIdValue.FirstValue, out sectionId) || sectionId == 0)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return null;
            }

            var model = await CreateFormModelFromDbAsync(formId, sectionId, _dynamicFormsApplicationService.FormApplicationService);

            if (model != null)
            {
                string formSubmissionId = "";
                var formSubmissionIdValue = bindingContext.ValueProvider.GetValue("FormSubmissionId");
                if (formSubmissionIdValue != null)
                {
                    formSubmissionId = formSubmissionIdValue.FirstValue;
                }

                string sectionSlug = "";
                var sectionSlugValue = bindingContext.ValueProvider.GetValue("SectionSlug");
                if (sectionSlugValue != null)
                {
                    sectionSlug = sectionSlugValue.FirstValue;
                }

                await PopulateFormValuesFromDbAsync(model, formSubmissionId, sectionId, sectionSlug, _dynamicFormsApplicationService.FormSectionSubmissionApplicationService);
            }

            return model;
        }

        public async static Task<DynamicFormModel> CreateFormModelFromDbAsync(int formId, int sectionId, IFormApplicationService formApplicationService, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await formApplicationService.GetByIdAsync(formId, cancellationToken, true, true);

            if (form == null || !form.Published)
            {
                return null;
            }

            var section = form.Sections.First(s => s.SectionId == sectionId);

            if (section == null || section.Section == null)
            {
                return null;
            }

            var formModel = new DynamicFormModel(formId, sectionId);

            //Setup Form
            foreach (var sectionQuestion in section.Section.Questions)
            {
                var question = sectionQuestion.Question;

                var questionFieldName = question.FieldName;
                switch (question.QuestionType)
                {
                    case QuestionType.Text:
                        formModel.Add(questionFieldName, string.Empty);
                        break;
                    case QuestionType.TextArea:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new MultilineTextAttribute(5));
                        break;
                    case QuestionType.Number:
                        formModel.Add(questionFieldName, new Nullable<int>());
                        break;
                    case QuestionType.Slider:
                        formModel.Add(questionFieldName, new Nullable<int>());
                        formModel.AddAttribute(questionFieldName, new SliderAttribute());
                        break;
                    case QuestionType.Currency:
                        formModel.Add(questionFieldName, new Nullable<decimal>());
                        formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.Currency));
                        break;
                    case QuestionType.Date:
                        formModel.Add(questionFieldName, new Nullable<DateTime>());
                        formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.Date));
                        break;
                    case QuestionType.DateTime:
                        formModel.Add(questionFieldName, new Nullable<DateTime>());
                        formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.DateTime));
                        break;
                    case QuestionType.PhoneNumber:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.PhoneNumber));
                        break;
                    case QuestionType.Email:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.EmailAddress));
                        break;
                    case QuestionType.Checkbox:
                        formModel.Add(questionFieldName, false);
                        break;
                    case QuestionType.YesButton:
                        formModel.Add(questionFieldName, false);
                        formModel.AddAttribute(questionFieldName, new BooleanYesButtonAttribute());
                        break;
                    case QuestionType.Dropdown:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new DropdownAttribute(typeof(LookupTableItem), nameof(LookupTableItem.Text), nameof(LookupTableItem.Value)));
                        formModel.AddAttribute(questionFieldName, new DbWhereClauseEqualsAttribute(nameof(LookupTableItem.LookupTableId), question.LookupTableId.HasValue ? question.LookupTableId.Value : 0));
                        break;
                    case QuestionType.DropdownMany:
                        formModel.Add(questionFieldName, new List<string>());
                        formModel.AddAttribute(questionFieldName, new DropdownAttribute(typeof(LookupTable), nameof(LookupTable.Name), nameof(LookupTable.Name)));
                        formModel.AddAttribute(questionFieldName, new DbWhereClauseEqualsAttribute(nameof(LookupTableItem.LookupTableId), question.LookupTableId.HasValue ? question.LookupTableId.Value : 0));
                        break;
                    case QuestionType.RadioListTrueFalse:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new TrueFalseCheckboxOrRadioAttribute());
                        break;
                    case QuestionType.RadioListYesNo:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new YesNoCheckboxOrRadioAttribute());
                        break;
                    case QuestionType.RadioList:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new CheckboxOrRadioAttribute(typeof(LookupTableItem), nameof(LookupTableItem.Text), nameof(LookupTableItem.Value)));
                        formModel.AddAttribute(questionFieldName, new DbWhereClauseEqualsAttribute(nameof(LookupTableItem.LookupTableId), question.LookupTableId.HasValue ? question.LookupTableId.Value : 0));
                        break;
                    case QuestionType.RadioListButtons:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new CheckboxOrRadioButtonsAttribute(typeof(LookupTableItem), nameof(LookupTableItem.Text), nameof(LookupTableItem.Value)));
                        formModel.AddAttribute(questionFieldName, new DbWhereClauseEqualsAttribute(nameof(LookupTableItem.LookupTableId), question.LookupTableId.HasValue ? question.LookupTableId.Value : 0));
                        break;
                    case QuestionType.RadioListButtonsYesNo:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new YesNoCheckboxOrRadioButtonsAttribute());
                        break;
                    case QuestionType.RadioListButtonsTrueFalse:
                        formModel.Add(questionFieldName, string.Empty);
                        formModel.AddAttribute(questionFieldName, new TrueFalseCheckboxOrRadioButtonsAttribute());
                        break;
                    case QuestionType.CheckboxList:
                        formModel.Add(questionFieldName, new List<string>());
                        formModel.AddAttribute(questionFieldName, new CheckboxOrRadioAttribute(typeof(LookupTableItem), nameof(LookupTableItem.Text), nameof(LookupTableItem.Value)));
                        formModel.AddAttribute(questionFieldName, new DbWhereClauseEqualsAttribute(nameof(LookupTableItem.LookupTableId), question.LookupTableId.HasValue ? question.LookupTableId.Value : 0));
                        break;
                    case QuestionType.CheckboxListButtons:
                        formModel.Add(questionFieldName, new List<string>());
                        formModel.AddAttribute(questionFieldName, new CheckboxOrRadioButtonsAttribute(typeof(LookupTableItem), nameof(LookupTableItem.Text), nameof(LookupTableItem.Value)));
                        formModel.AddAttribute(questionFieldName, new DbWhereClauseEqualsAttribute(nameof(LookupTableItem.LookupTableId), question.LookupTableId.HasValue ? question.LookupTableId.Value : 0));
                        break;
                    case QuestionType.File:
                        formModel.Add(questionFieldName, new FormFile(null, 0, 0, "", ""));
                        break;
                    case QuestionType.FileImage:
                        formModel.Add(questionFieldName, new FormFile(null, 0, 0, "", ""));
                        formModel.AddAttribute(questionFieldName, new FileImageAcceptAttribute());
                        break;
                    case QuestionType.FileVideo:
                        formModel.Add(questionFieldName, new FormFile(null, 0, 0, "", ""));
                        formModel.AddAttribute(questionFieldName, new FileVideoAcceptAttribute());
                        break;
                    case QuestionType.FileAudio:
                        formModel.Add(questionFieldName, new FormFile(null, 0, 0, "", ""));
                        formModel.AddAttribute(questionFieldName, new FileAudioAcceptAttribute());
                        break;
                    case QuestionType.FileImageVideo:
                        formModel.Add(questionFieldName, new FormFile(null, 0, 0, "", ""));
                        formModel.AddAttribute(questionFieldName, new FileImageVideoAcceptAttribute());
                        break;
                    case QuestionType.MultipleFiles:
                        formModel.Add(questionFieldName, new List<FormFile>());
                        break;
                    case QuestionType.MultipleFilesImage:
                        formModel.Add(questionFieldName, new List<FormFile>());
                        formModel.AddAttribute(questionFieldName, new FileImageAcceptAttribute());
                        break;
                    case QuestionType.MultipleFilesVideo:
                        formModel.Add(questionFieldName, new List<FormFile>());
                        formModel.AddAttribute(questionFieldName, new FileVideoAcceptAttribute());
                        break;
                    case QuestionType.MultipleFilesAudio:
                        formModel.Add(questionFieldName, new List<FormFile>());
                        formModel.AddAttribute(questionFieldName, new FileAudioAcceptAttribute());
                        break;
                    case QuestionType.MultipleFilesImageVideo:
                        formModel.Add(questionFieldName, new List<FormFile>());
                        formModel.AddAttribute(questionFieldName, new FileImageVideoAcceptAttribute());
                        break;
                    default:
                        throw new Exception("QuestionType not Mapped");
                }

                //Add Default Values
                if (!string.IsNullOrEmpty(question.DefaultAnswer))
                {
                    foreach (var defaultValue in question.DefaultAnswer.Split(','))
                    {
                        formModel[questionFieldName] = defaultValue;
                    }
                }

                //Add Question Text 
                //Add Placeholder
                formModel.AddAttribute(questionFieldName, new DisplayAttribute() { Name = question.QuestionText, Prompt = question.Placeholder });

                //Add Help Text
                if (!string.IsNullOrEmpty(question.HelpText))
                {
                    formModel.AddAttribute(questionFieldName, new HelpTextAttribute(question.HelpText));

                }

                //Add Validation
                foreach (var questionValidation in question.Validations)
                {
                    switch (questionValidation.ValidationType)
                    {
                        case QuestionValidationType.Required:
                            var validation = new RequiredAttribute();

                            if (!string.IsNullOrWhiteSpace(questionValidation.CustomValidationMessage))
                            {
                                validation.ErrorMessage = questionValidation.CustomValidationMessage;
                            }
                            formModel.AddAttribute(questionFieldName, validation);
                            break;
                        default:
                            throw new Exception("ValidationType not Mapped");
                    }
                }
            }

            return formModel;
        }

        public async static Task PopulateFormValuesFromDbAsync(DynamicFormModel formModel, string formSubmissionId, int sectionId, string sectionSlug, IFormSectionSubmissionApplicationService formSectionSubmissionApplicationService, CancellationToken cancellationToken = default(CancellationToken))
        {
            FormSectionSubmissionDto sectionSubmission = null;
            if (!string.IsNullOrEmpty(formSubmissionId) && !string.IsNullOrEmpty(sectionSlug) && sectionId > 0)
            {
                var formSubmissionGuid = new Guid(formSubmissionId);
                sectionSubmission = await formSectionSubmissionApplicationService.GetOneAsync(cancellationToken, fss => fss.FormSubmissionId == formSubmissionGuid && fss.SectionId == sectionId && fss.UrlSlug == sectionSlug);
            }

            if (sectionSubmission != null)
            {
                foreach (var propertyName in formModel.GetDynamicMemberNames())
                {
                    var persistedValue = sectionSubmission.QuestionAnswers.Where(qa => qa.FieldName == propertyName).FirstOrDefault();
                    if (persistedValue != null)
                    {
                        bool isCollection = formModel.IsCollectionProperty(propertyName);

                        foreach (var csvSplit in persistedValue.Answer.Split(','))
                        {
                            formModel[propertyName] = csvSplit.Trim();
                        }
                        if (!isCollection)
                        {
                            break;
                        }

                    }
                }
            }
        }
    }
}
