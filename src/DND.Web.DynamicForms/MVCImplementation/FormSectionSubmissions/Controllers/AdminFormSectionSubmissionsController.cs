using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.FormSectionSubmissions.Controllers
{
    [Route("admin/forms/form-section-submissions")]
    public class AdminFormSectionSubmissionsController : BaseEntityControllerAuthorize<FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto, IFormSectionSubmissionApplicationService>
    {
        public AdminFormSectionSubmissionsController(IFormSectionSubmissionApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
