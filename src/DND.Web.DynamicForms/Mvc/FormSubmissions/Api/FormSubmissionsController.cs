using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.LookupTables.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/form-submissions")]
    public class FormSubmissionController : ApiControllerEntityAuthorizeBase<FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto, IFormSubmissionApplicationService>
    {
        public FormSubmissionController(IFormSubmissionApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, urlHelper, typeHelperService, appSettings)
        {

        }
    }
}
