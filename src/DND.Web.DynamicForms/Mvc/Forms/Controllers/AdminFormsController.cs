using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.DynamicForms.Mvc.Forms.Controllers
{
    [Route("admin/forms/forms")]
    public class AdminFormsController : MvcControllerEntityAuthorizeBase<FormDto, FormDto, FormDto, FormDeleteDto, IFormApplicationService>
    {
        public AdminFormsController(IFormApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
