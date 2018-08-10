using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.LookupTables.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/form-submissions")]
    public class FormSubmissionController : BaseEntityWebApiControllerAuthorize<FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto, IFormSubmissionApplicationService>
    {
        public FormSubmissionController(IFormSubmissionApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}
