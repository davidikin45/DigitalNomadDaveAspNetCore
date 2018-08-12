using AutoMapper;
using DND.Common.ActionResults;
using DND.Common.Controllers;
using DND.Common.Dynamic;
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
using System.Dynamic;
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

        private DynamicTypeDescriptorWrapper SetupForm(bool summary)
        {
            string containerDiv = "#dynamicForm";

            //1. Setup Form definition
            ExpandoObject dto = new ExpandoObject();
            dto.Add("Text", "");
            dto.Add("Email", "");
            dto.Add("Website", "");
            dto.Add("PhoneNumber", "");
            dto.Add("TextArea", "");
            dto.Add("Number", 0);
            dto.Add("Slider", 50);

            if (summary)
            {
                dto.Add("IconButton", "");
            }

            var section2Link = Html.ActionLink("Section 2", "Edit", "DynamicForms", new { sectionId = "section2", formId = "insurance" }, new { @class="text-danger", data_ajax = "true", data_ajax_method = "GET", data_ajax_mode = "replace", data_ajax_update = containerDiv }).Render();
            dto.Add("SectionHeading", section2Link);

            decimal currency = 0;
            dto.Add("Currency", currency);
            dto.Add("Date", new DateTime());
            dto.Add("DateTime", new DateTime());
            dto.Add("Dropdown", "");
            dto.Add("DropdownMany", new List<string>());
            dto.Add("RadioList", "");
            dto.Add("RadioListButtons", "");
            dto.Add("CheckboxList", new List<string>());
            dto.Add("CheckboxListButtons", new List<string>());
            dto.Add("Checkbox", false);
            dto.Add("YesButton", false);

            dto.Add("YesNo", "");
            dto.Add("YesNoButtons", "");
            dto.Add("YesNoButtonsBoolean", false);

            dto.Add("TrueFalse", "");
            dto.Add("TrueFalseButtons", "");
            dto.Add("TrueFalseButtonsBoolean", false);

            FormFile formFile = new FormFile(null, 0, 0, "", "");
            dto.Add("File", formFile);
            dto.Add("MultipleFiles", new List<FormFile>() { });
            dto.Add("MultipleMediaFiles", new List<FormFile>() { });

            //YesNoList
            //var sections = new List<string>();
            //for (int i = 0; i < 10; i++)
            //{
            //    sections.Add("Section" + i);
            //    dto.Add("Section" + i, "");
            //}

            if (summary)
            {
                dto.Add("Submit", "Submit");
            }
            else
            {
                dto.Add("Submit", "Next");
            }

            var wrapper = new DynamicTypeDescriptorWrapper(dto);

            //2. Add Display and Validation
            wrapper.AddAttribute("Text", new DisplayAttribute() { Name = "What is your Name?" });
            wrapper.AddAttribute("Text", new RequiredAttribute() { ErrorMessage ="Please enter your name1." });
            wrapper.AddAttribute("Email", new DataTypeAttribute(DataType.EmailAddress));
            wrapper.AddAttribute("Email", new HelpTextAttribute("Your personal email please"));
            wrapper.AddAttribute("PhoneNumber", new DataTypeAttribute(DataType.PhoneNumber));
            wrapper.AddAttribute("Email", new RequiredAttribute() { ErrorMessage = "Please enter your Email3." });
            wrapper.AddAttribute("Website", new DataTypeAttribute(DataType.Url));
            wrapper.AddAttribute("TextArea", new MultilineTextAttribute(5));

            wrapper.AddAttribute("Number", new NumberValidatorAttribute());

            wrapper.AddAttribute("Slider", new SliderAttribute(0, 100));

            if (summary)
            {
                wrapper.AddAttribute("IconButton", new OffsetRightAttribute(1));
                wrapper.AddAttribute("IconButton", new EditLinkAttribute("Edit", "DynamicForms", containerDiv));
                wrapper.AddAttribute("IconButton", new LinkRouteValueAttribute("formId", "insurance"));
                wrapper.AddAttribute("IconButton", new LinkRouteValueAttribute("sectionId", "section2"));
            }
            //text-success
            wrapper.AddAttribute("SectionHeading", new HeadingAttributeH3("text-danger"));

            wrapper.AddAttribute("Dropdown", new DropdownAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            wrapper.AddAttribute("DropdownMany", new DropdownAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));

            wrapper.AddAttribute("RadioList", new CheckboxOrRadioAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            wrapper.AddAttribute("RadioList", new RequiredAttribute());
            wrapper.AddAttribute("RadioListButtons", new CheckboxOrRadioButtonsAttribute(new List<string>() { "Option 1", "Option 2", "Option 3", "Option 4" }));

            wrapper.AddAttribute("CheckboxList", new CheckboxOrRadioAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            //wrapper.AddAttribute("CheckboxList", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("CheckboxList", new LimitCountAttribute(3, 5));
            wrapper.AddAttribute("CheckboxList", new RequiredAttribute());

            wrapper.AddAttribute("CheckboxListButtons", new CheckboxOrRadioButtonsAttribute(new List<string>() { "Option 1", "Option 2", "Option 3", "Option 4" }));

            wrapper.AddAttribute("Currency", new DataTypeAttribute(DataType.Currency));

            wrapper.AddAttribute("Date", new DataTypeAttribute(DataType.Date));
            wrapper.AddAttribute("Date", new AgeValidatorAttribute(18));
            wrapper.AddAttribute("Date", new RequiredAttribute());

            wrapper.AddAttribute("DateTime", new DataTypeAttribute(DataType.DateTime));

            wrapper.AddAttribute("Checkbox", new RequiredAttribute());

            wrapper.AddAttribute("YesButton", new BooleanYesButtonAttribute());

            wrapper.AddAttribute("YesNo", new YesNoCheckboxOrRadioAttribute());
            wrapper.AddAttribute("YesNo", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("YesNo", new RequiredAttribute());

            wrapper.AddAttribute("YesNoButtons", new YesNoCheckboxOrRadioButtonsAttribute());
            wrapper.AddAttribute("YesNoButtons", new RequiredAttribute());

            wrapper.AddAttribute("YesNoButtonsBoolean", new BooleanYesNoButtonsAttribute());

            wrapper.AddAttribute("TrueFalse", new TrueFalseCheckboxOrRadioAttribute());
            wrapper.AddAttribute("TrueFalse", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("TrueFalse", new RequiredAttribute());

            wrapper.AddAttribute("TrueFalseButtons", new TrueFalseCheckboxOrRadioButtonsAttribute());
            wrapper.AddAttribute("TrueFalseButtons", new RequiredAttribute());

            wrapper.AddAttribute("TrueFalseButtonsBoolean", new BooleanTrueFalseButtonsAttribute());

            //foreach (var section in sections)
            //{
            //    wrapper.AddAttribute(section, new YesNoCheckboxOrRadioAttribute());
            //    wrapper.AddAttribute(section, new CheckboxOrRadioInlineAttribute());
            //    wrapper.AddAttribute(section, new RequiredAttribute());
            //}

            //wrapper.AddAttribute("Submit", new OffsetRightAttribute(1));

            wrapper.AddAttribute("MultipleMediaFiles", new FileImageAudioVideoAcceptAttribute());

            wrapper.AddAttribute("Submit", new NoLabelAttribute());
            wrapper.AddAttribute("Submit", new SubmitButtonAttribute("btn btn-block btn-success"));

            return wrapper;
        }

        private void PopulateForm(DynamicTypeDescriptorWrapper form, IEnumerable<KeyValuePair<string, StringValues>> formData)
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

        private void PopulateFormRouteData(DynamicTypeDescriptorWrapper form, IEnumerable<KeyValuePair<string, object>> formData)
        {
            foreach (var item in formData)
            {
                if (form.ContainsProperty(item.Key))
                {
                    form[item.Key] = item.Value.ToString().Trim();
                }
            }
        }

        private void PopulateFormFiles(DynamicTypeDescriptorWrapper form, IFormFileCollection formData)
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
