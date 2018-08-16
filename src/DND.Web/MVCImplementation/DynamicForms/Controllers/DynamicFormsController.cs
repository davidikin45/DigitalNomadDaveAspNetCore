﻿using AutoMapper;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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

        #region Edit
        [NoAjaxRequest]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}")]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> Edit(string formUrlSlug, string sectionUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var formModel = await _dynamicFormsPresentationService.CreateFormModelFromDbAsync(formUrlSlug, sectionUrlSlug, cts.Token);

            if(formModel == null)
            {
                return BadRequest();
            }

            var formSubmissionId = _cookieService.Get(formUrlSlug);
            if(string.IsNullOrEmpty(_cookieService.Get(formUrlSlug)))
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

            //Add Additional controls
            formModel.Add("Submit", "Continue");
            formModel.AddAttribute("Submit", new NoLabelAttribute());
            formModel.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            ViewBag.DetailsMode = false;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;

            return View("DynamicFormMenuAndFormPage", formModel);
        }

        [AjaxRequest]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}")]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> EditAjax(string formUrlSlug, string sectionUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var formModel = await _dynamicFormsPresentationService.CreateFormModelFromDbAsync(formUrlSlug, sectionUrlSlug, cts.Token);

            if (formModel == null)
            {
                return BadRequest();
            }

            var formSubmissionId = _cookieService.Get(formUrlSlug);
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

            //Add Additional controls
            formModel.Add("Submit", "Continue");
            formModel.AddAttribute("Submit", new NoLabelAttribute());
            formModel.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            ViewBag.DetailsMode = false;
            return View("_DynamicFormMenuAndForm", formModel);
        }

        private DynamicFormModel GetEditFormResponse(RouteData routeData, IQueryCollection queryStringData)
        {
            //1. Setup Form definition and 2. Add Validation
            var form = SetupForm(false);

            //3. Prepropulate from routeData and query string
            form.BindData(null, routeData, queryStringData);

            return form;
        }
        #endregion

        #region Update
        [NoAjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}")]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> Update(DynamicFormModel formModel, IFormCollection formData)
        {
            var formUrlSlug = this.RouteData.Values[DynamicFormsValueProviderKeys.FormUrlSlug].ToString();
            //dto.Id = id;
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            
            //var response = GetUpdateFormResponse(formData, this.RouteData, Request.Query);

            //Manual validation
           // if (TryValidateModel(response))
            //{
                //Save Data
                //Redirect
            //};

            //Model Binder Validation
            if(ModelState.IsValid)
            {
                //Persist to Db and say valid
                //Redirect to next section
                var nextSectionUrlSlug = "";
                return RedirectToAction<DynamicFormsController>(c => c.Edit(formUrlSlug, nextSectionUrlSlug));
            }

            ViewBag.DetailsMode = false;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;
            return View("DynamicFormMenuAndFormPage", formModel);
        }

        [AjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}")]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/section/{*" + DynamicFormsValueProviderKeys.SectionUrlSlug + "}")]
        public virtual async Task<IActionResult> UpdateAjax(DynamicFormModel formModel)
        {
            var formUrlSlug = this.RouteData.Values[DynamicFormsValueProviderKeys.FormUrlSlug].ToString();



            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //var response = GetUpdateFormResponse(formData, this.RouteData, Request.Query);

            //Manual validation
            //if (TryValidateModel(response))
            //{
                //Save Data
                //Redirect
            //};

            //Model Binder Validation
            if (ModelState.IsValid)
            {
                //Persist to Db
                //Redirect to next section
                var nextSectionUrlSlug = ""; 
                return RedirectToAction<DynamicFormsController>(c => c.EditAjax(formUrlSlug, nextSectionUrlSlug));
            }

            ViewBag.DetailsMode = false;
            return PartialView("_DynamicFormMenuAndForm", formModel);
        }

        private DynamicFormModel GetUpdateFormResponse(IFormCollection formData, RouteData routeData, IQueryCollection queryStringData)
        {
            var form = SetupForm(false);

            form.BindData(formData, routeData, queryStringData);    

            return form;
        }
        #endregion

        #region Summary
        [AjaxRequest]
        [HttpGet]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/summary")]
        public virtual async Task<IActionResult> Summary(string formUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetSummaryResponse();

            var sections = new List<DynamicFormModel>() { response, response };

            ViewBag.ExcludePropertyErrors = false;
            ViewBag.DetailsMode = true;
            return View("_DynamicFormMenuAndForm", response);
        }

        [NoAjaxRequest]
        [HttpGet]
        [Route("{" + DynamicFormsValueProviderKeys.FormUrlSlug + "}/summary")]
        public virtual async Task<IActionResult> SummaryAjax(string formUrlSlug)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetSummaryResponse();

            var sections = new List<DynamicFormModel>() { response, response };

            TryValidateModel(sections);

            ViewBag.ExcludePropertyErrors = false;
            ViewBag.DetailsMode = true;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;
            return View("DynamicFormMenuAndFormPage", sections);
        }

        private DynamicFormModel GetSummaryResponse()
        {
            var wrapper = SetupForm(true);


            return wrapper;
        }
        #endregion

        private DynamicFormModel SetupForm(bool summary)
        {
            string containerDiv = "#dynamicForm";

            //1. Setup Form definition
            dynamic model = new DynamicFormModel();
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
