using AutoMapper;
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
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.DynamicForms.Controllers
{
    [Route("forms")]
    public class DynamicFormsController : BaseController
    {
        private readonly IHtmlHelper Html;
        private readonly ICookieService _cookieService;

        public DynamicFormsController(IMapper mapper, IEmailService emailService, IConfiguration configuration, IHtmlHelperGeneratorService htmlHelperGeneratorService, ICookieService cookieService)
            : base(mapper, emailService, configuration)
        {
            Html = htmlHelperGeneratorService.HtmlHelper("");
            _cookieService = cookieService;
        }

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

        [NoAjaxRequest]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{formId}")]
        [Route("{formId}/section/{sectionId}")]
        public virtual IActionResult Update(string formId, string sectionId, IFormCollection formData)
        {
            //dto.Id = id;
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetUpdateFormResponse(formData);

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
        public virtual IActionResult UpdateAjax(string formId, string sectionId, IFormCollection formData)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = GetUpdateFormResponse(formData);

            ViewBag.DetailsMode = false;
            return PartialView("_DynamicFormMenuAndForm", response);
        }

        private DynamicTypeDescriptorWrapper GetUpdateFormResponse(IFormCollection formData)
        {
            var wrapper = SetupForm(false);

            //3. Populate with formData
            PopulateForm(wrapper, formData);
            PopulateFormFiles(wrapper, formData.Files);

            //4. Validate
            TryValidateModel(wrapper);

            return wrapper;
        }

        [HttpGet]
        [Route("{formId}/summary")]
        public virtual ActionResult Summary(string formId)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            try
            {
                //1. Setup Form definition and 2. Add Validation
                var wrapper = SetupForm(true);

                //3. Prepropulate from routeData and query string
                PopulateFormRouteData(wrapper, this.RouteData.Values);
                PopulateForm(wrapper, Request.Query);

                TryValidateModel(wrapper);

                ViewBag.ExcludePropertyErrors = false;
                ViewBag.DetailsMode = true;
                ViewBag.PageTitle = Title;
                ViewBag.Admin = false;
                return View("DynamicFormMenuAndFormPage", wrapper);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

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
            dto.Add("CheckboxList", new List<string>());
            dto.Add("CheckboxFalseDefault", false);
            dto.Add("CheckboxTrueDefault", true);
            dto.Add("YesNo", "");
            dto.Add("YesNoYesDefault", "Yes");
            dto.Add("YesNoNoDefault", "No");
            dto.Add("TrueFalse", "");
            dto.Add("TrueFalseTrueDefault", "True");
            dto.Add("TrueFalseFalseDefault", "False");
            FormFile formFile = new FormFile(null, 0, 0, "", "");
            dto.Add("File", formFile);

            //YesNoList
            var sections = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                sections.Add("Section" + i);
                dto.Add("Section" + i, "");
            }

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

            wrapper.AddAttribute("CheckboxList", new CheckboxOrRadioAttribute(Type.GetType("DND.Domain.Blog.Tags.Tag, DND.Domain.Blog"), "Name", "Name"));
            wrapper.AddAttribute("CheckboxList", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("CheckboxList", new LimitCountAttribute(3, 5));
            wrapper.AddAttribute("CheckboxList", new RequiredAttribute());

            wrapper.AddAttribute("Currency", new DataTypeAttribute(DataType.Currency));

            wrapper.AddAttribute("Date", new DataTypeAttribute(DataType.Date));
            wrapper.AddAttribute("Date", new AgeValidatorAttribute(18));
            wrapper.AddAttribute("Date", new RequiredAttribute());

            wrapper.AddAttribute("DateTime", new DataTypeAttribute(DataType.DateTime));

            wrapper.AddAttribute("CheckboxTrueDefault", new RequiredAttribute());

            wrapper.AddAttribute("CheckboxFalseDefault", new RequiredAttribute());

            wrapper.AddAttribute("YesNo", new YesNoCheckboxOrRadioAttribute());
            wrapper.AddAttribute("YesNo", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("YesNo", new RequiredAttribute());

            wrapper.AddAttribute("YesNoYesDefault", new YesNoCheckboxOrRadioAttribute());
            wrapper.AddAttribute("YesNoYesDefault", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("YesNoYesDefault", new RequiredAttribute());

            wrapper.AddAttribute("YesNoNoDefault", new YesNoCheckboxOrRadioAttribute());
            wrapper.AddAttribute("YesNoNoDefault", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("YesNoNoDefault", new RequiredAttribute());

            wrapper.AddAttribute("TrueFalse", new TrueFalseCheckboxOrRadioAttribute());
            wrapper.AddAttribute("TrueFalse", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("TrueFalse", new RequiredAttribute());

            wrapper.AddAttribute("TrueFalseTrueDefault", new TrueFalseCheckboxOrRadioAttribute());
            wrapper.AddAttribute("TrueFalseTrueDefault", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("TrueFalseTrueDefault", new RequiredAttribute());

            wrapper.AddAttribute("TrueFalseFalseDefault", new TrueFalseCheckboxOrRadioAttribute());
            wrapper.AddAttribute("TrueFalseFalseDefault", new CheckboxOrRadioInlineAttribute());
            wrapper.AddAttribute("TrueFalseFalseDefault", new RequiredAttribute());

            foreach (var section in sections)
            {
                wrapper.AddAttribute(section, new YesNoCheckboxOrRadioAttribute());
                wrapper.AddAttribute(section, new CheckboxOrRadioInlineAttribute());
                wrapper.AddAttribute(section, new RequiredAttribute());
            }

            //wrapper.AddAttribute("Submit", new OffsetRightAttribute(1));
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
