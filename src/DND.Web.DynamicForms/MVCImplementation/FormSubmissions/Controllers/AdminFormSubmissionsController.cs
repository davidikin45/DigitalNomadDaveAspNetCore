using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.FormSubmissions.Controllers
{
    [Route("admin/forms/form-submissions")]
    public class AdminFormSubmissionsController : BaseEntityControllerAuthorize<FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto, IFormSubmissionApplicationService>
    {
        public AdminFormSubmissionsController(IFormSubmissionApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
