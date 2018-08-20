using AutoMapper;
using DND.Common.ActionResults;
using DND.Common.Controllers;
using DND.Common.DynamicForms;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.Filters;
using DND.Common.Helpers;
using DND.Common.Interfaces.Services;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.DynamicFormsAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Common.ModelMetadataCustom.ValidationAttributes;
using DND.Common.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.DynamicForms.Controllers
{
    [Route("forms")]
    public class DynamicFormsController : BaseController
    {
        private readonly IHtmlHelper Html;
        private readonly ICookieService _cookieService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDynamicFormsPresentationService _dynamicFormsPresentationService;
        private readonly IHtmlHelperGeneratorService _htmlHelperGeneratorService;

        public DynamicFormsController(
            IMapper mapper, 
            IEmailService emailService, IConfiguration configuration, 
            IHtmlHelperGeneratorService htmlHelperGeneratorService, 
            ICookieService cookieService, 
            IHostingEnvironment hostingEnvironment,
            IDynamicFormsPresentationService dynamicFormsPresentationService)
            : base(mapper, emailService, configuration)
        {
            Html = htmlHelperGeneratorService.HtmlHelper("");
            _cookieService = cookieService;
            _hostingEnvironment = hostingEnvironment;
            _dynamicFormsPresentationService = dynamicFormsPresentationService;
        }

        #region IFrame Embed
        [Route("{"+ DynamicFormsValueProviderKeys.FormUrlSlug + "}/embed")]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/embed.js")]
        public virtual async Task<IActionResult> Embed(string formUrlSlug)
        {
            string absoluteFormUrl = Url.AbsoluteUrl<DynamicFormsController>(c => c.Edit(formUrlSlug, ""), true);
            string iFrameEmbedPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"MVCImplementation\DynamicForms\Scripts\IFrameEmbed.js");
            var iFrameEmbedScript = System.IO.File.ReadAllText(iFrameEmbedPath);
            iFrameEmbedScript = iFrameEmbedScript.Replace("{absoluteFormUrl}", absoluteFormUrl);
            return new JavaScriptResult(iFrameEmbedScript);
        }
        #endregion

        #region Initial Page Redirect
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}")]
        public virtual async Task<IActionResult> InitialPage(string formUrlSlug)
        {
            var nextSectionUrlSlug = await _dynamicFormsPresentationService.GetFirstSectionUrlSlugAsync(formUrlSlug);
            if(string.IsNullOrWhiteSpace(nextSectionUrlSlug))
            {
                return BadRequest();
            }
            else
            {
                return Redirect(Html.Url().Action<DynamicFormsController>(c => c.Edit(formUrlSlug, nextSectionUrlSlug)).Replace("%2F","/"));
            }
        }
        #endregion

        #region Edit
        [NoAjaxRequest]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> Edit(string formUrlSlug, string sectionUrlSlug)
        {
            return await Edit("DynamicFormContainerPage", false, formUrlSlug, sectionUrlSlug);
        }

        [AjaxRequest]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> EditAjax(string formUrlSlug, string sectionUrlSlug)
        {
            return await Edit("_DynamicFormContainer", true, formUrlSlug, sectionUrlSlug);
        }

        private async Task<IActionResult> Edit(string viewName, bool partial, string formUrlSlug, string sectionUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var formModel = await _dynamicFormsPresentationService.CreateFormModelFromDbAsync(formUrlSlug, sectionUrlSlug, cts.Token);

            if (formModel == null)
            {
                return BadRequest();
            }

            var formSubmissionId = _cookieService.Get(formUrlSlug);

            if (!(await _dynamicFormsPresentationService.IsValidSubmissionUrl(formSubmissionId, formUrlSlug, sectionUrlSlug, cts.Token)))
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(_cookieService.Get(formUrlSlug)))
            {
                formSubmissionId = Guid.NewGuid().ToString();
                _cookieService.Set(formUrlSlug, formSubmissionId, 14);
            }
            else
            {
                if (sectionUrlSlug == null)
                {
                    sectionUrlSlug = await _dynamicFormsPresentationService.GetFirstSectionUrlSlugAsync(formUrlSlug);
                }

                await _dynamicFormsPresentationService.PopulateFormModelFromDbAsync(formModel, formSubmissionId, sectionUrlSlug);
            }

            var formContainer = await _dynamicFormsPresentationService.CreateFormContainerAsync(formModel, formUrlSlug, sectionUrlSlug, formSubmissionId, cts.Token);
  
            ViewBag.DetailsMode = false;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;

            if(partial)
            {
                return PartialView(viewName, formContainer);
            }
            else
            {
                return View(viewName, formContainer);
            }
        }
        #endregion

        #region Update
        [NoAjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> Update(List<DynamicForm> formModels)
        {
            return await Update("DynamicFormContainerPage", false, formModels);
        }

        [AjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> UpdateAjax(List<DynamicForm> formModels)
        {
            return await Update("_DynamicFormContainer", true, formModels);
        }

        private async Task<IActionResult> Update(string viewName, bool partial, List<DynamicForm> formModels)
        {
            var formUrlSlug = this.RouteData.Values[DynamicFormsValueProviderKeys.FormUrlSlug].ToString();
            var sectionUrlSlug = this.RouteData.Values[DynamicFormsValueProviderKeys.SectionUrlSlug].ToString();
            //dto.Id = id;
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var formSubmissionId = _cookieService.Get(formUrlSlug);

            if (!(await _dynamicFormsPresentationService.IsValidSubmissionUrl(formSubmissionId, formUrlSlug, sectionUrlSlug, cts.Token)))
            {
                return BadRequest();
            }

            var isValid = ModelState.IsValid;
            await _dynamicFormsPresentationService.SaveFormModelToDb(formModels.First(), formSubmissionId, formUrlSlug, sectionUrlSlug, isValid, cts.Token);

            if (isValid)
            {
                var nextSectionUrlSlug = await _dynamicFormsPresentationService.GetNextSectionUrlSlug(formSubmissionId, formUrlSlug, sectionUrlSlug);
                if(!string.IsNullOrWhiteSpace(nextSectionUrlSlug))
                {
                    if (partial)
                    {
                        return Redirect(Html.Url().Action<DynamicFormsController>(c => c.EditAjax(formUrlSlug, nextSectionUrlSlug)).Replace("%2F", "/"));
                    }
                    else
                    {
                        return Redirect(Html.Url().Action<DynamicFormsController>(c => c.Edit(formUrlSlug, nextSectionUrlSlug)).Replace("%2F", "/"));
                    }
                }
                else
                {
                    if (partial)
                    {
                        return RedirectToAction<DynamicFormsController>(c => c.SummaryAjax(formUrlSlug));
                    }
                    else
                    {
                        return RedirectToAction<DynamicFormsController>(c => c.Summary(formUrlSlug));
                    }
                }
            }

            var formContainer = await _dynamicFormsPresentationService.CreateFormContainerAsync(formModels.First(), formUrlSlug, sectionUrlSlug, formSubmissionId, cts.Token);

            ViewBag.DetailsMode = false;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;

            if (partial)
            {
                return PartialView(viewName, formContainer);
            }
            else
            {
                return View(viewName, formContainer);
            }
        }
        #endregion

        #region Summary View
        [AjaxRequest]
        [HttpGet]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/summary")]
        public virtual async Task<IActionResult> Summary(string formUrlSlug)
        {
            return await Summary("_DynamicFormContainer", true, formUrlSlug);
        }

        [NoAjaxRequest]
        [HttpGet]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/summary")]
        public virtual async Task<IActionResult> SummaryAjax(string formUrlSlug)
        {
            return await Summary("DynamicFormContainerPage", false, formUrlSlug);
        }

        private async Task<IActionResult> Summary(string viewName, bool partial, string formUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            var formSubmissionId = _cookieService.Get(formUrlSlug);

            var formContainer = await _dynamicFormsPresentationService.CreateFormSummaryContainerAsync(formUrlSlug, formSubmissionId, cts.Token);

            TryValidateModel(formContainer.Forms);

            ViewBag.ExcludePropertyErrors = false;
            ViewBag.DetailsMode = true;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;

            if (partial)
            {
                return PartialView(viewName, formContainer);
            }
            else
            {
                return View(viewName, formContainer);
            }
        }

        #endregion

        #region Summary Submit
        [AjaxRequest]
        [HttpPost]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/summary")]
        public virtual async Task<IActionResult> SummarySubmit(string formUrlSlug)
        {
            return await SummarySubmit("_DynamicFormContainer", true, formUrlSlug);
        }

        [NoAjaxRequest]
        [HttpPost]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/summary")]
        public virtual async Task<IActionResult> SummarySubmitAjax(string formUrlSlug)
        {
            return await SummarySubmit("DynamicFormContainerPage", false, formUrlSlug);
        }

        private async Task<IActionResult> SummarySubmit(string viewName, bool partial, string formUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var formSubmissionId = _cookieService.Get(formUrlSlug);

            if (!(await _dynamicFormsPresentationService.IsValidSubmissionUrl(formSubmissionId, formUrlSlug, "summary", cts.Token)))
            {
                return BadRequest();
            }

            if (partial)
            {
                return RedirectToAction<DynamicFormsController>(c => c.ConfirmationAjax(formUrlSlug));
            }
            else
            {
                return RedirectToAction<DynamicFormsController>(c => c.Confirmation(formUrlSlug));
            }
        }
        #endregion

        #region Confirmation
        [AjaxRequest]
        [HttpGet]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/confirmation")]
        public virtual async Task<IActionResult> Confirmation(string formUrlSlug)
        {
            return await Confirmation("_DynamicFormContainer", true, formUrlSlug);
        }

        [NoAjaxRequest]
        [HttpGet]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/confirmation")]
        public virtual async Task<IActionResult> ConfirmationAjax(string formUrlSlug)
        {
            return await Confirmation("DynamicFormContainerPage", false, formUrlSlug);
        }

        private async Task<IActionResult> Confirmation(string viewName, bool partial, string formUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            var formSubmissionId = _cookieService.Get(formUrlSlug);

            if (!(await _dynamicFormsPresentationService.IsValidSubmissionUrl(formSubmissionId, formUrlSlug, "confirmation", cts.Token)))
            {
                return BadRequest();
            }

            var formContainer = _dynamicFormsPresentationService.CreateFormConfirmationContainerAsync(formUrlSlug, formSubmissionId, cts.Token);

            ViewBag.ExcludePropertyErrors = false;
            ViewBag.DetailsMode = true;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;

            if (partial)
            {
                return PartialView(viewName, formContainer);
            }
            else
            {
                return View(viewName, formContainer);
            }
        }

        #endregion

        private DynamicForm SetupForm(bool summary)
        {
            string containerDiv = "#dynamicForm";

            //1. Setup Form definition
            dynamic model = new DynamicForm();
            model.Add("Text", "");
            model.Add("Email", "");
            model.Add("Website", "");
            model.Add("PhoneNumber", "");
            model.Add("TextArea", "");
            model.Add("Number", 0);
            model.Add("Slider", 50);

            if (summary)
            {
                model.Add("IconButton", "");
            }

            var section2Link = Html.ActionLink("Section 2", "Edit", "DynamicForms", new { sectionId = "section2", formId = "insurance" }, new { @class="text-danger", data_ajax = "true", data_ajax_method = "GET", data_ajax_mode = "replace", data_ajax_update = containerDiv }).Render();
            model.Add("SectionHeading", section2Link);

            decimal currency = 0;
            model.Add("Currency", currency);
            model.Add("Date", new DateTime());
            model.Add("DateTime", new DateTime());
            model.Add("Dropdown", "");
            model.Add("DropdownMany", new List<string>());
            model.Add("RadioList", "");
            model.Add("RadioListButtons", "");
            model.Add("CheckboxList", new List<string>());
            model.Add("CheckboxListButtons", new List<string>());
            model.Add("Checkbox", false);
            model.Add("YesButton", false);

            model.Add("YesNo", "");
            model.Add("YesNoButtons", "");



            model.Add("YesNoButtonsBoolean", false);

            model.Add("TrueFalse", "");
            model.Add("TrueFalseButtons", "");
            model.Add("TrueFalseButtonsBoolean", false);

            FormFile formFile = new FormFile(null, 0, 0, "", "");
            model.Add("File", formFile);
            model.Add("MultipleFiles", new List<FormFile>() { });
            model.Add("MultipleMediaFiles", new List<FormFile>() { });

            if (summary)
            {
                model.Add("Submit", "Submit");
            }
            else
            {
                model.Add("Submit", "Continue");
            }

            //2. Add Display and Validation
            model.AddAttribute("Text", new DisplayAttribute() { Name = "What is your Name?" });
            model.AddAttribute("Email", new DataTypeAttribute(DataType.EmailAddress));
            model.AddAttribute("Email", new HelpTextAttribute("Your personal email please"));
            model.AddAttribute("PhoneNumber", new DataTypeAttribute(DataType.PhoneNumber));
            model.AddAttribute("Website", new DataTypeAttribute(DataType.Url));
            model.AddAttribute("TextArea", new MultilineTextAttribute(5));

            model.AddAttribute("Number", new NumberValidatorAttribute());

            model.AddAttribute("Slider", new SliderAttribute(0, 100));

            if (summary)
            {
                model.AddAttribute("IconButton", new OffsetRightAttribute(1));
                model.AddAttribute("IconButton", new EditLinkAttribute("Edit", "DynamicForms", containerDiv));
                model.AddAttribute("IconButton", new LinkRouteValueAttribute("formId", "insurance"));
                model.AddAttribute("IconButton", new LinkRouteValueAttribute("sectionId", "section2"));
            }
            //text-success
            model.AddAttribute("SectionHeading", new HeadingAttributeH3("text-danger"));

            model.AddAttribute("Dropdown", new DropdownAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            model.AddAttribute("DropdownMany", new DropdownAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));

            model.AddAttribute("RadioList", new CheckboxOrRadioAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            model.AddAttribute("RadioListButtons", new CheckboxOrRadioButtonsAttribute(new List<string>() { "Option 1", "Option 2", "Option 3", "Option 4" }));

            model.AddAttribute("CheckboxList", new CheckboxOrRadioAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            //wrapper.AddAttribute("CheckboxList", new CheckboxOrRadioInlineAttribute());
            model.AddAttribute("CheckboxList", new LimitCountAttribute(3, 5));

            model.AddAttribute("CheckboxListButtons", new CheckboxOrRadioButtonsAttribute(new List<string>() { "Option 1", "Option 2", "Option 3", "Option 4" }));

            model.AddAttribute("Currency", new DataTypeAttribute(DataType.Currency));

            model.AddAttribute("Date", new DataTypeAttribute(DataType.Date));
            model.AddAttribute("Date", new AgeValidatorAttribute(18));

            model.AddAttribute("DateTime", new DataTypeAttribute(DataType.DateTime));

            model.AddAttribute("YesButton", new BooleanYesButtonAttribute());

            model.AddAttribute("YesNo", new YesNoCheckboxOrRadioAttribute());
            model.AddAttribute("YesNo", new CheckboxOrRadioInlineAttribute());

            model.AddAttribute("YesNoButtons", new YesNoCheckboxOrRadioButtonsAttribute());

            model.AddAttribute("YesNoButtonsBoolean", new BooleanYesNoButtonsAttribute());

            model.AddAttribute("TrueFalse", new TrueFalseCheckboxOrRadioAttribute());
            model.AddAttribute("TrueFalse", new CheckboxOrRadioInlineAttribute());

            model.AddAttribute("TrueFalseButtons", new TrueFalseCheckboxOrRadioButtonsAttribute());

            model.AddAttribute("TrueFalseButtonsBoolean", new BooleanTrueFalseButtonsAttribute());

            model.AddAttribute("MultipleMediaFiles", new FileImageAudioVideoAcceptAttribute());

            model.AddAttribute("Submit", new NoLabelAttribute());
            model.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            return model;
        }
    }
}