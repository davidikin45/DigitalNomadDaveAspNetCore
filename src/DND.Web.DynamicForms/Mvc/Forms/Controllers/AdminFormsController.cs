using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.Forms.Controllers
{
    [Route("admin/forms/forms")]
    public class AdminFormsController : MvcControllerEntityAuthorizeBase<FormDto, FormDto, FormDto, FormDeleteDto, IFormApplicationService>
    {
        public AdminFormsController(IFormApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
