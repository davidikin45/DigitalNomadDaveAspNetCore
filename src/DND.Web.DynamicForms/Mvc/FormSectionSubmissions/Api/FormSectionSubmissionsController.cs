using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.FormSectionSubmissions.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/form-section-submissions")]
    public class FormSectionSubmissionsController : ApiControllerEntityAuthorizeBase<FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto, IFormSectionSubmissionApplicationService>
    {
        public FormSectionSubmissionsController(IFormSectionSubmissionApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, urlHelper, typeHelperService, appSettings)
        {

        }
    }
}
