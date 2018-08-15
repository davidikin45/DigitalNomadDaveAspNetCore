using DND.Common.DynamicForms;
using DND.Common.Helpers;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using DND.Domain.DynamicForms.Questions.Dtos;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.PresentationServices;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.DynamicForms.PresentationServices
{
    public class DynamicFormsPresentationService : IDynamicFormsPresentationService
    {
        private readonly IDynamicFormsApplicationServices _dynamicFormsApplicationServices;

        public DynamicFormsPresentationService(IDynamicFormsApplicationServices dynamicFormsApplicationServices)
        {
            _dynamicFormsApplicationServices = dynamicFormsApplicationServices;
        }

        public async Task<DynamicFormModel> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await _dynamicFormsApplicationServices.FormApplicationService.GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            if (form == null || !form.Published)
            {
                return null;
            }

            //var section = "section#1_employees_employee#3";

            IEnumerable<SectionDto> sections = form.Sections.Select(s=>s.Section);

            SectionDto slugSection = null;
            bool sectionPart = true; 

            foreach (var sectionUrlSlugPart in sectionUrlSlug.Split('_'))
            {
                var urlSlug = sectionUrlSlugPart.Split('#')[0];
                if(sectionPart)
                {
                    slugSection = sections.First(s => s.UrlSlug == urlSlug);
                    if(slugSection == null)
                    {
                        return null;
                    }
                    sectionPart = false;
                }
                else
                {
                    var questionSection = slugSection.Questions.First(q => q.Question.FieldName == urlSlug);
                    if (questionSection == null)
                    {
                        return null;
                    }
                    sections = questionSection.Question.Sections.Select(s => s.Section);
                    sectionPart = true;
                }
            }

            if(sectionPart)
            {
                return null;
            }

            var formModel = new DynamicFormModel();

            //Setup Form
            foreach (var sectionQuestion in slugSection.Questions)
            {
                var question = sectionQuestion.Question;
                await AddQuestionToFormAsync(formModel, question, cancellationToken);
            }

            return formModel;
        }

        private async Task AddQuestionToFormAsync(DynamicFormModel formModel, QuestionDto question, CancellationToken cancellationToken = default(CancellationToken))
        {
            LookupTableDto lookupTable;
            IEnumerable<string> options;

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
                    question.DefaultAnswer = BoolParser.IsTrue(question.DefaultAnswer) ? "true" : "";
                    break;
                case QuestionType.YesButton:
                    formModel.Add(questionFieldName, false);
                    formModel.AddAttribute(questionFieldName, new BooleanYesButtonAttribute());
                    question.DefaultAnswer = BoolParser.IsTrue(question.DefaultAnswer) ? "true" : "";
                    break;
                case QuestionType.Dropdown:
                    formModel.Add(questionFieldName, string.Empty);
                    lookupTable = await _dynamicFormsApplicationServices.LookupTableApplicationService.GetByIdAsync(question.LookupTableId.HasValue ? question.LookupTableId.Value : 0, cancellationToken, true);
                    options = lookupTable != null ? lookupTable.LookupTableItems.Select(lt => lt.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new DropdownAttribute(options));
                    break;
                case QuestionType.DropdownMany:
                    formModel.Add(questionFieldName, new List<string>());
                    lookupTable = await _dynamicFormsApplicationServices.LookupTableApplicationService.GetByIdAsync(question.LookupTableId.HasValue ? question.LookupTableId.Value : 0, cancellationToken, true);
                    options = lookupTable != null ? lookupTable.LookupTableItems.Select(lt => lt.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new DropdownAttribute(options));
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
                    lookupTable = await _dynamicFormsApplicationServices.LookupTableApplicationService.GetByIdAsync(question.LookupTableId.HasValue ? question.LookupTableId.Value : 0, cancellationToken, true);
                    options = lookupTable != null ? lookupTable.LookupTableItems.Select(lt => lt.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new CheckboxOrRadioAttribute(options));
                    break;
                case QuestionType.RadioListButtons:
                    formModel.Add(questionFieldName, string.Empty);
                    lookupTable = await _dynamicFormsApplicationServices.LookupTableApplicationService.GetByIdAsync(question.LookupTableId.HasValue ? question.LookupTableId.Value : 0, cancellationToken, true);
                    options = lookupTable != null ? lookupTable.LookupTableItems.Select(lt => lt.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new CheckboxOrRadioButtonsAttribute(options));
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
                    lookupTable = await _dynamicFormsApplicationServices.LookupTableApplicationService.GetByIdAsync(question.LookupTableId.HasValue ? question.LookupTableId.Value : 0, cancellationToken, true);
                    options = lookupTable != null ? lookupTable.LookupTableItems.Select(lt => lt.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new CheckboxOrRadioAttribute(options));
                    break;
                case QuestionType.CheckboxListButtons:
                    formModel.Add(questionFieldName, new List<string>());
                    lookupTable = await _dynamicFormsApplicationServices.LookupTableApplicationService.GetByIdAsync(question.LookupTableId.HasValue ? question.LookupTableId.Value : 0, cancellationToken, true);
                    options = lookupTable != null ? lookupTable.LookupTableItems.Select(lt => lt.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new CheckboxOrRadioButtonsAttribute(options));
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

            //Add Conditional Questions
            foreach (var questionQuestion in question.Questions)
            {
                var logicQuestion = questionQuestion.LogicQuestion;
                await AddQuestionToFormAsync(formModel, logicQuestion, cancellationToken);
            }
        }

        public async Task PopulateFormModelFromDbAsync(DynamicFormModel formModel, string formSubmissionId, string sectionSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            FormSectionSubmissionDto sectionSubmission = null;
            if (!string.IsNullOrEmpty(formSubmissionId) && !string.IsNullOrEmpty(sectionSlug))
            {
                var formSubmissionGuid = new Guid(formSubmissionId);
                sectionSubmission = await _dynamicFormsApplicationServices.FormSectionSubmissionApplicationService.GetOneAsync(cancellationToken, fss => fss.FormSubmissionId == formSubmissionGuid && fss.UrlSlug == sectionSlug);
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
