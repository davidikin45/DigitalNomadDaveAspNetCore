using DND.Common.DynamicForms;
using DND.Common.Extensions;
using DND.Common.Helpers;
using DND.Common.Interfaces.Services;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.DynamicFormsAttributes;
using DND.Common.ModelMetadataCustom.Providers;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using DND.Domain.DynamicForms.Questions.Dtos;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.DynamicForms.PresentationServices
{
    public class DynamicFormsPresentationService : IDynamicFormsPresentationService
    {
        private readonly IDynamicFormsApplicationServices _dynamicFormsApplicationServices;
        private readonly IHtmlHelperGeneratorService _htmlHelperGeneratorService;

        public DynamicFormsPresentationService(IDynamicFormsApplicationServices dynamicFormsApplicationServices, IHtmlHelperGeneratorService htmlHelperGeneratorService)
        {
            _dynamicFormsApplicationServices = dynamicFormsApplicationServices;
            _htmlHelperGeneratorService = htmlHelperGeneratorService;
        }

        #region Form Definition
        private List<string> GetSectionUrlSlugParts(string sectionUrlSlug)
        {
            return sectionUrlSlug.Split('/').ToList();
        }

        private static ConcurrentDictionary<string, FormDto> formDtos = new ConcurrentDictionary<string, FormDto>();
        public async Task<FormDto> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            return formDtos.GetOrAdd(formUrlSlug, await _dynamicFormsApplicationServices.FormApplicationService.GetFormByUrlSlugAsync(formUrlSlug, cancellationToken));
        }

        public async Task<bool> IsValidUrl(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            if (form == null || !form.Published)
            {
                return false;
            }

            return GetSectionByUrlSlug(form, sectionUrlSlug) != null;
        }

        private async Task<SectionDto> GetSectionByUrlSlugsAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);
            return GetSectionByUrlSlug(form, sectionUrlSlug);
        }

        private SectionDto GetSectionByUrlSlug(FormDto form, string sectionUrlSlug)
        {
            SectionDto slugSection = null;

            IEnumerable<SectionDto> sections = form.Sections.Select(s => s.Section);

            if (sectionUrlSlug != null)
            {
                int i = 0;
                foreach (var sectionUrlSlugPart in GetSectionUrlSlugParts(sectionUrlSlug))
                {
                    if (i % 2 == 0)
                    {
                        slugSection = sections.First(s => s.UrlSlug == sectionUrlSlugPart);
                        if (slugSection == null)
                        {
                            return null;
                        }
                        sections = slugSection.Sections.Select(s => s.ChildSection);

                    }
                    else if (i % 2 == 1)
                    {
                        //index
                        //number
                    }

                    i++;
                }

                if (i % 2 != 0)
                {
                    return null;
                }
            }
            else if (sections.Count() > 0)
            {
                slugSection = sections.First();
            }

            return slugSection;
        }

        public async Task<DynamicForm> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            if (form == null || !form.Published)
            {
                return null;
            }

            var slugSection = GetSectionByUrlSlug(form, sectionUrlSlug);

            if (slugSection == null)
            {
                return null;
            }

            var formModel = new DynamicForm();

            //Setup Form Section
            if (slugSection != null)
            {
                foreach (var sectionQuestion in slugSection.Questions)
                {
                    var question = sectionQuestion.Question;
                    AddQuestionToForm(formModel, question);
                }
            }

            return formModel;
        }

        private void AddQuestionToForm(DynamicForm formModel, QuestionDto question)
        {
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
                    formModel.Add(questionFieldName, default(int));
                    break;
                case QuestionType.Slider:
                    formModel.Add(questionFieldName, default(int));
                    formModel.AddAttribute(questionFieldName, new SliderAttribute());
                    break;
                case QuestionType.Currency:
                    formModel.Add(questionFieldName, default(decimal));
                    formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.Currency));
                    break;
                case QuestionType.Date:
                    formModel.Add(questionFieldName, default(DateTime));
                    formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.Date));
                    break;
                case QuestionType.DateTime:
                    formModel.Add(questionFieldName, default(DateTime));
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
                case QuestionType.Website:
                    formModel.Add(questionFieldName, string.Empty);
                    formModel.AddAttribute(questionFieldName, new DataTypeAttribute(DataType.Url));
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
                    options = question.LookupTable != null ? question.LookupTable.LookupTableItems.Select(lti => lti.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new DropdownAttribute(options));
                    break;
                case QuestionType.DropdownMany:
                    formModel.Add(questionFieldName, new List<string>());
                    options = question.LookupTable != null ? question.LookupTable.LookupTableItems.Select(lti => lti.Text) : new List<string>();
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
                    options = question.LookupTable != null ? question.LookupTable.LookupTableItems.Select(lti => lti.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new CheckboxOrRadioAttribute(options));
                    break;
                case QuestionType.RadioListButtons:
                    formModel.Add(questionFieldName, string.Empty);
                    options = question.LookupTable != null ? question.LookupTable.LookupTableItems.Select(lti => lti.Text) : new List<string>();
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
                    options = question.LookupTable != null ? question.LookupTable.LookupTableItems.Select(lti => lti.Text) : new List<string>();
                    formModel.AddAttribute(questionFieldName, new CheckboxOrRadioAttribute(options));
                    break;
                case QuestionType.CheckboxListButtons:
                    formModel.Add(questionFieldName, new List<string>());
                    options = question.LookupTable != null ? question.LookupTable.LookupTableItems.Select(lti => lti.Text) : new List<string>();
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
            //foreach (var questionQuestion in question.Questions)
            //{
            //    var logicQuestion = questionQuestion.LogicQuestion;
            //    await AddQuestionToFormAsync(formModel, logicQuestion, cancellationToken);
            //}
        }
        #endregion

        #region Form Sections
        private async Task<Dictionary<string, SectionDto>> GetSectionsAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            var sectionNames = new Dictionary<string, string>();
            var sectionRoutes = new Dictionary<string, SectionDto>();
            var navigation = GetFormNavigation(formUrlSlug, form.Sections.Select(s => s.Section).ToList(), form.Sections.Select(s => s.Name).ToList(), sectionRoutes, sectionNames);

            return sectionRoutes;
        }

        private async Task<Dictionary<string, string>> GetSectionNamesAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            var sectionNames = new Dictionary<string, string>();
            var sectionRoutes = new Dictionary<string, SectionDto>();
            var navigation = GetFormNavigation(formUrlSlug, form.Sections.Select(s => s.Section).ToList(), form.Sections.Select(s => s.Name).ToList(), sectionRoutes, sectionNames);

            return sectionNames;
        }
        #endregion

        #region Form Section Submissions
        public async Task PopulateFormModelFromDbAsync(DynamicForm formModel, string formSubmissionId, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            FormSectionSubmissionDto sectionSubmission = null;
            if (!string.IsNullOrEmpty(formSubmissionId) && !string.IsNullOrEmpty(sectionUrlSlug))
            {
                var formSubmissionGuid = new Guid(formSubmissionId);
                sectionSubmission = await _dynamicFormsApplicationServices.FormSectionSubmissionApplicationService.GetOneAsync(cancellationToken, fss => fss.FormSubmissionId == formSubmissionGuid && fss.UrlSlug == sectionUrlSlug, true);
            }

            if (sectionSubmission != null)
            {
                var properties = formModel.GetProperties();

                foreach (var propertyName in formModel.GetDynamicMemberNames())
                {
                    var persistedValue = sectionSubmission.QuestionAnswers.Where(qa => qa.FieldName == propertyName).FirstOrDefault();
                    if (persistedValue != null)
                    {
                        bool isCollection = formModel.IsCollectionProperty(propertyName);
                        var property = properties.Find(propertyName, true);

                        if (isCollection)
                        {
                            var genericCollectionType = typeof(List<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]);
                            var newCollection = Activator.CreateInstance(genericCollectionType);

                            formModel[propertyName] = newCollection;

                            var addMethod = genericCollectionType.GetMethod("Add");

                            foreach (var csvSplit in persistedValue.Answer.Split(','))
                            {
                                var convertedValue = Convert.ChangeType(csvSplit.Trim(), property.PropertyType.GetGenericArguments()[0]);
                                addMethod.Invoke(newCollection, new object[] { convertedValue });
                            }
                        }
                        else if (property.PropertyType == typeof(DateTime))
                        {
                            if (!String.IsNullOrWhiteSpace(persistedValue.Answer))
                            {
                                var parsedValue = DateTime.Parse(persistedValue.Answer);
                                formModel[propertyName] = parsedValue;
                            }
                            else
                            {
                                formModel[propertyName] = new DateTime();
                            }
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            formModel[propertyName] = BoolParser.GetValue(persistedValue.Answer);
                        }
                        else if (property.PropertyType == typeof(decimal))
                        {
                            var convertedValue = decimal.Parse(persistedValue.Answer, NumberStyles.Currency);
                            formModel[propertyName] = convertedValue;
                        }
                        else
                        {
                            var convertedValue = Convert.ChangeType(persistedValue.Answer, property.PropertyType);
                            formModel[propertyName] = convertedValue;
                        }
                    }
                }
            }
        }

        public Dictionary<string, string> GetFormDisplayValues(DynamicForm formModel)
        {
            var displayValues = new Dictionary<string, string>();

            var htmlHelper = _htmlHelperGeneratorService.HtmlHelper(formModel);
            var properties = ((CustomModelMetadata)htmlHelper.ViewData.ModelMetadata).PropertiesRuntime(formModel);
            var propertyModelExplorers = htmlHelper.ModelExplorerPropertiesRuntime();

            foreach (var prop in htmlHelper.ViewData.ModelMetadata.Properties.Where(p => p.ShowForDisplay))
            {

                if (prop.IsCollectionType)
                {
                    var displayString = htmlHelper.Display(prop.PropertyName).Render();
                    displayValues.Add(prop.PropertyName, displayString);
                }
                else if(prop.ModelType == typeof(Boolean))
                {
                    var displayString = htmlHelper.Display(prop.PropertyName).Render();
                    displayValues.Add(prop.PropertyName, displayString);
                }
                else if (prop.ModelType == typeof(DateTime))
                {
                    var displayString = ((DateTime)formModel[prop.PropertyName]).ToString("O");
                    displayValues.Add(prop.PropertyName, displayString);
                }
                else
                {
                    var displayString = htmlHelper.DisplayText(prop.PropertyName);
                    displayValues.Add(prop.PropertyName, displayString);
                }
            }

            return displayValues;
        }

        public async Task SaveFormModelToDb(DynamicForm formModel, string formSubmissionId, string formUrlSlug, string sectionUrlSlug, bool isValid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var formDisplayValues = GetFormDisplayValues(formModel);

            var formSubmissionGuid = new Guid(formSubmissionId);
            FormSubmissionDto submission = await _dynamicFormsApplicationServices.FormSubmissionApplicationService.GetByIdAsync(formSubmissionGuid, cancellationToken);
            bool newSubmission = true;
            if(submission != null)
            {
                newSubmission = false;
            }

            FormSectionSubmissionDto sectionSubmission = null;
            if (!string.IsNullOrEmpty(formSubmissionId) && !string.IsNullOrEmpty(sectionUrlSlug))
            {
                sectionSubmission = await _dynamicFormsApplicationServices.FormSectionSubmissionApplicationService.GetOneAsync(cancellationToken, fss => fss.FormSubmissionId == formSubmissionGuid && fss.UrlSlug == sectionUrlSlug, true);
            }

            bool newSectionSubmission = false;
            if (sectionSubmission == null)
            {
                newSectionSubmission = true;
                sectionSubmission = new FormSectionSubmissionDto() { FormSubmissionId = formSubmissionGuid, UrlSlug = sectionUrlSlug };
            }

            sectionSubmission.Valid = isValid;

            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            var section = await GetSectionByUrlSlugsAsync(formUrlSlug, sectionUrlSlug);

            foreach (var sectionQuestion in section.Questions)
            {
                var question = sectionQuestion.Question;

                var answer = "";
                if (formDisplayValues.ContainsKey(question.FieldName))
                {
                    answer = formDisplayValues[question.FieldName];
                }

                var questionAnswer = sectionSubmission.QuestionAnswers.FirstOrDefault(qa => qa.FieldName == question.FieldName);
                if (questionAnswer == null)
                {
                    questionAnswer = new FormSectionSubmissionQuestionAnswerDto() { FieldName = question.FieldName, FormSectionSubmissionId = sectionSubmission.Id };
                    sectionSubmission.QuestionAnswers.Add(questionAnswer);
                }

                questionAnswer.Question = question.QuestionText;
                questionAnswer.Answer = answer;
            }

            if(newSubmission)
            {
                var newFormSubmission = new FormSubmissionDto() { Id = formSubmissionGuid, FormId = form.Id };
                newFormSubmission.Sections.Add(sectionSubmission);
                await _dynamicFormsApplicationServices.FormSubmissionApplicationService.CreateAsync(newFormSubmission, "", cancellationToken);
            }
            else
            {
                if (newSectionSubmission)
                {
                    await _dynamicFormsApplicationServices.FormSectionSubmissionApplicationService.CreateAsync(sectionSubmission, "", cancellationToken);
                }
                else
                {
                    await _dynamicFormsApplicationServices.FormSectionSubmissionApplicationService.UpdateGraphAsync(sectionSubmission.Id, sectionSubmission, "", cancellationToken);
                }
            }
        }
        #endregion

        #region Valid Submission Url
        public async Task<bool> IsValidSubmissionUrl(string formSubmissionId, string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(sectionUrlSlug == "summary" || sectionUrlSlug == "confirmation")
            {
                return true;
            }

            var sections = await GetSectionsAsync(formUrlSlug, formSubmissionId, cancellationToken);
            return sections.ContainsKey(sectionUrlSlug);
        }
        #endregion

        #region First and Next Setion
        public async Task<string> GetFirstSectionUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sections = await GetSectionsAsync(formUrlSlug, "", cancellationToken);
            if(sections.Count > 0)
            {
                return sections.First().Key;
            }
            else
            {
                return "";
            }
        }

        public async Task<string> GetNextSectionUrlSlug(string formSubmissionId, string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sections = await GetSectionsAsync(formUrlSlug, formSubmissionId, cancellationToken);

            bool returnNextSection = false;
            foreach (var section in sections)
            {
                if (returnNextSection)
                {
                    return section.Key;
                }

                if (section.Key == sectionUrlSlug)
                {
                    returnNextSection = true;
                }
            }

            return "";
        }
        #endregion

        #region Navigation
        public async Task<DynamicFormNavigation> GetFormNavigationAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var form = await GetFormByUrlSlugAsync(formUrlSlug, cancellationToken);

            var sectionNames = new Dictionary<string, string>();
            var sectionRoutes = new Dictionary<string, SectionDto>();
            var navigation = GetFormNavigation(formUrlSlug, form.Sections.Select(s => s.Section).ToList(), form.Sections.Select(s => s.Name).ToList(), sectionRoutes, sectionNames);

            var summary = new DynamicFormNavigationMenuItem() { LinkText = "Summary", ActionName = "Summary", ControllerName = "DynamicForms", IsValid = false };

            navigation.MenuItems.Add(summary);

            return navigation;
        }

        private DynamicFormNavigation GetFormNavigation(string formUrlSlug, List<SectionDto> sections, List<string> sectionNames, Dictionary<string, SectionDto> sectionRoutesDict, Dictionary<string, string> sectionNamesDict, string sectionUrlSlugPrefix = "")
        {
            var navigation = new DynamicFormNavigation();

            var dict = new Dictionary<string, int>();
            for (int i = 0; i < sections.Count(); i++)
            {
                var section = sections[i];
                var sectionName = !string.IsNullOrWhiteSpace(sectionNames[i]) ? sectionNames[i] : section.Name;

                if (!dict.ContainsKey(section.UrlSlug))
                {
                    dict.Add(section.UrlSlug, 0);
                }
                dict[section.UrlSlug]++;

                sectionName = sectionName.Replace("{i}", dict[section.UrlSlug].ToString());

                var menuItem = new DynamicFormNavigationMenuItem() { LinkText = sectionName, ActionName = "Edit", ControllerName = "DynamicForms", IsValid = false };
                menuItem.RouteValues.Add(DynamicFormsValueProviderKeys.FormUrlSlug, formUrlSlug);

                var sectionUrlSlug = sectionUrlSlugPrefix + section.UrlSlug + "/" + dict[section.UrlSlug].ToString();
                menuItem.RouteValues.Add(DynamicFormsValueProviderKeys.SectionUrlSlug, sectionUrlSlug);

                sectionRoutesDict.Add(sectionUrlSlug, section);
                sectionNamesDict.Add(sectionUrlSlug, sectionName);

                var childNavigation = GetFormNavigation(formUrlSlug, section.Sections.Select(s => s.ChildSection).ToList(), section.Sections.Select(s => s.Name).ToList(), sectionRoutesDict, sectionNamesDict, sectionUrlSlug + "/");
                if (childNavigation.MenuItems.Count != 0)
                {
                    menuItem.ChildNavigation = childNavigation;
                }

                navigation.MenuItems.Add(menuItem);
            }

            return navigation;
        }

        public async Task<DynamicFormContainer> CreateFormContainerAsync(DynamicForm formModel, string formUrlSlug, string sectionUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Add Additional controls
            formModel.Add("Submit", "Continue");
            formModel.AddAttribute("Submit", new NoLabelAttribute());
            formModel.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            var navigation = await GetFormNavigationAsync(formUrlSlug, formSubmissionId, cancellationToken);

            var formContainer = new DynamicFormContainer();
            formContainer.Forms.Add(formModel);
            formContainer.Navigation = navigation;

            return formContainer;
        }
        #endregion

        #region Summary
        public async Task<DynamicFormContainer> CreateFormSummaryContainerAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var formContainer = new DynamicFormContainer();
            formContainer.Navigation = await GetFormNavigationAsync(formUrlSlug, formSubmissionId, cancellationToken);

            var sections = await GetSectionsAsync(formUrlSlug, formSubmissionId, cancellationToken);
            var sectionNames = await GetSectionNamesAsync(formUrlSlug, formSubmissionId, cancellationToken);

            foreach (var section in sections)
            {
                var headingForm = new DynamicForm();
                headingForm.Add("SectionHeading", sectionNames[section.Key]);
                headingForm.AddAttribute("SectionHeading", new HeadingAttributeH3("text-success"));

                formContainer.Forms.Add(headingForm);

                var formModel = await CreateFormModelFromDbAsync(formUrlSlug, section.Key);
                await PopulateFormModelFromDbAsync(formModel, formSubmissionId, section.Key, cancellationToken);

                formContainer.Forms.Add(formModel);
            }

            var additionalControls = new DynamicForm();
            additionalControls.Add("Submit", "Submit");
            additionalControls.AddAttribute("Submit", new NoLabelAttribute());
            additionalControls.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            formContainer.Forms.Add(additionalControls);

            return formContainer;
        }
        #endregion

        #region Confirmation
        public Task<DynamicFormContainer> CreateFormConfirmationContainerAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
