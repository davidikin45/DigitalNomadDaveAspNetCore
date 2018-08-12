using AutoMapper;
using DND.Common.ActionResults;
using DND.Common.Controllers;
using DND.Common.Dynamic;
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
using Microsoft.Extensions.Primitives;
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


        public DynamicFormsController(
            IMapper mapper, 
            IEmailService emailService, IConfiguration configuration, 
            IHtmlHelperGeneratorService htmlHelperGeneratorService, 
            ICookieService cookieService, 
            IHostingEnvironment hostingEnvironment)
            : base(mapper, emailService, configuration)
        {
            Html = htmlHelperGeneratorService.HtmlHelper("");
            _cookieService = cookieService;
            _hostingEnvironment = hostingEnvironment;
        }

        #region IFrame Embed
        [Route("embed/{formId}.js")]
        public virtual async Task<IActionResult> IFrameEmbed(string formId)
        {
            string absoluteFormUrl = Url.AbsoluteUrl<DynamicFormsController>(c => c.Edit(formId, ""), true);
            string iFrameEmbedPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"MVCImplementation\DynamicForms\Scripts\IFrameEmbed.js");
            var iFrameEmbedScript = System.IO.File.ReadAllText(iFrameEmbedPath);
            iFrameEmbedScript = iFrameEmbedScript.Replace("{absoluteFormUrl}", absoluteFormUrl);
            return new JavaScriptResult(iFrameEmbedScript);
        }
        #endregion

        #region Edit
        [NoAjaxRequest]
        [Route("{formId}")]
        [Route("{formId}/section/{sectionId}")]
        public virtual async Task<IActionResult> Edit(string formId, string sectionId)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetEditFormResponse(this.RouteData.Values, Request.Query);

            ViewBag.DetailsMode = false;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;

            return View("DynamicFormMenuAndFormPage", response);
        }

        [AjaxRequest]
        [Route("{formId}")]
        [Route("{formId}/section/{sectionId}")]
        public virtual async Task<IActionResult> EditAjax(string formId, string sectionId)
        {
            var response = GetEditFormResponse(this.RouteData.Values, Request.Query);

            ViewBag.DetailsMode = false;
            return View("_DynamicFormMenuAndForm", response);
        }

        private DynamicTypeDescriptorWrapper GetEditFormResponse(RouteValueDictionary routeData, IQueryCollection queryString)
        {
            //1. Setup Form definition and 2. Add Validation
            var wrapper = SetupForm(false);

            //3. Prepropulate from routeData and query string
            PopulateFormRouteData(wrapper, this.RouteData.Values);
            PopulateForm(wrapper, Request.Query);

            return wrapper;
        }
        #endregion

        #region Update
        [NoAjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{formId}")]
        [Route("{formId}/section/{sectionId}")]
        public virtual async Task<IActionResult> Update(string formId, string sectionId, IFormCollection formData)
        {
            //dto.Id = id;
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetUpdateFormResponse(formData);

            //Manual validation
            if (TryValidateModel(response))
            {
                //Save Data
                //Redirect
            };

            ViewBag.DetailsMode = false;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;
            return View("DynamicFormMenuAndFormPage", response);
        }

        [AjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{formId}")]
        [Route("{formId}/section/{sectionId}")]
        public virtual async Task<IActionResult> UpdateAjax(string formId, string sectionId, IFormCollection formData)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetUpdateFormResponse(formData);


            //Manual validation
            if (TryValidateModel(response))
            {
                //Save Data
                //Redirect
            };

            ViewBag.DetailsMode = false;
            return PartialView("_DynamicFormMenuAndForm", response);
        }

        private DynamicTypeDescriptorWrapper GetUpdateFormResponse(IFormCollection formData)
        {
            var wrapper = SetupForm(false);

            //3. Populate with formData
            PopulateForm(wrapper, formData);
            PopulateFormFiles(wrapper, formData.Files);

            return wrapper;
        }
        #endregion

        #region Summary
        [AjaxRequest]
        [HttpGet]
        [Route("{formId}/summary")]
        public virtual async Task<IActionResult> Summary(string formId)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetSummaryResponse();

            ViewBag.ExcludePropertyErrors = false;
            ViewBag.DetailsMode = true;
            return View("_DynamicFormMenuAndForm", response);
        }

        [NoAjaxRequest]
        [HttpGet]
        [Route("{formId}/summary")]
        public virtual async Task<IActionResult> SummaryAjax(string formId)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetSummaryResponse();

            ViewBag.ExcludePropertyErrors = false;
            ViewBag.DetailsMode = true;
            ViewBag.PageTitle = Title;
            ViewBag.Admin = false;
            return View("DynamicFormMenuAndFormPage", response);
        }

        private DynamicTypeDescriptorWrapper GetSummaryResponse()
        {
            var wrapper = SetupForm(true);

            TryValidateModel(wrapper);

            return wrapper;
        }
        #endregion

        private DynamicFormModel SetupForm(bool summary)
        {
            string containerDiv = "#dynamicForm";

            //1. Setup Form definition
            DynamicFormModel model = new DynamicFormModel();
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
                model.Add("Submit", "Next");
            }

            //2. Add Display and Validation
            model.AddAttribute("Text", new DisplayAttribute() { Name = "What is your Name?" });
            model.AddAttribute("Text", new RequiredAttribute() { ErrorMessage ="Please enter your name1." });
            model.AddAttribute("Email", new DataTypeAttribute(DataType.EmailAddress));
            model.AddAttribute("Email", new HelpTextAttribute("Your personal email please"));
            model.AddAttribute("PhoneNumber", new DataTypeAttribute(DataType.PhoneNumber));
            model.AddAttribute("Email", new RequiredAttribute() { ErrorMessage = "Please enter your Email3." });
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
            model.AddAttribute("RadioList", new RequiredAttribute());
            model.AddAttribute("RadioListButtons", new CheckboxOrRadioButtonsAttribute(new List<string>() { "Option 1", "Option 2", "Option 3", "Option 4" }));

            model.AddAttribute("CheckboxList", new CheckboxOrRadioAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            //wrapper.AddAttribute("CheckboxList", new CheckboxOrRadioInlineAttribute());
            model.AddAttribute("CheckboxList", new LimitCountAttribute(3, 5));
            model.AddAttribute("CheckboxList", new RequiredAttribute());

            model.AddAttribute("CheckboxListButtons", new CheckboxOrRadioButtonsAttribute(new List<string>() { "Option 1", "Option 2", "Option 3", "Option 4" }));

            model.AddAttribute("Currency", new DataTypeAttribute(DataType.Currency));

            model.AddAttribute("Date", new DataTypeAttribute(DataType.Date));
            model.AddAttribute("Date", new AgeValidatorAttribute(18));
            model.AddAttribute("Date", new RequiredAttribute());

            model.AddAttribute("DateTime", new DataTypeAttribute(DataType.DateTime));

            model.AddAttribute("Checkbox", new RequiredAttribute());

            model.AddAttribute("YesButton", new BooleanYesButtonAttribute());

            model.AddAttribute("YesNo", new YesNoCheckboxOrRadioAttribute());
            model.AddAttribute("YesNo", new CheckboxOrRadioInlineAttribute());
            model.AddAttribute("YesNo", new RequiredAttribute());

            model.AddAttribute("YesNoButtons", new YesNoCheckboxOrRadioButtonsAttribute());
            model.AddAttribute("YesNoButtons", new RequiredAttribute());

            model.AddAttribute("YesNoButtonsBoolean", new BooleanYesNoButtonsAttribute());

            model.AddAttribute("TrueFalse", new TrueFalseCheckboxOrRadioAttribute());
            model.AddAttribute("TrueFalse", new CheckboxOrRadioInlineAttribute());
            model.AddAttribute("TrueFalse", new RequiredAttribute());

            model.AddAttribute("TrueFalseButtons", new TrueFalseCheckboxOrRadioButtonsAttribute());
            model.AddAttribute("TrueFalseButtons", new RequiredAttribute());

            model.AddAttribute("TrueFalseButtonsBoolean", new BooleanTrueFalseButtonsAttribute());

            model.AddAttribute("MultipleMediaFiles", new FileImageAudioVideoAcceptAttribute());

            model.AddAttribute("Submit", new NoLabelAttribute());
            model.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            return model;
        }

        private void PopulateForm(DynamicFormModel form, IEnumerable<KeyValuePair<string, StringValues>> formData)
        {
            foreach (var item in formData)
            {
                if (form.ContainsProperty(item.Key))
                {
                    foreach (var value in item.Value)
                    {
                        foreach (var csvSplit in value.Split(','))
                        {
                            form[item.Key] = csvSplit.Trim();
                        }
                    }
                }
            }
        }

        private void PopulateFormRouteData(DynamicFormModel form, IEnumerable<KeyValuePair<string, object>> formData)
        {
            foreach (var item in formData)
            {
                if (form.ContainsProperty(item.Key))
                {
                    form[item.Key] = item.Value.ToString().Trim();
                }
            }
        }

        private void PopulateFormFiles(DynamicFormModel form, IFormFileCollection formData)
        {
            if (formData != null)
            {
                foreach (var item in formData)
                {
                    if (form.ContainsProperty(item.Name))
                    {
                        form[item.Name] = (FormFile)item;
                    }
                }
            }
        }
    }
}
