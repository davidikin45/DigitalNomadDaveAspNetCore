using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.FormSectionSubmissions.Controllers
{
    [Route("admin/forms/form-section-submissions")]
    public class AdminFormSectionSubmissionsController : MvcControllerEntityAuthorizeBase<FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto, IFormSectionSubmissionApplicationService>
    {
        public AdminFormSectionSubmissionsController(IFormSectionSubmissionApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
