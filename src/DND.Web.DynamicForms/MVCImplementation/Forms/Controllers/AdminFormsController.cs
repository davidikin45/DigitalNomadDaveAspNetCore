using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.Forms.Controllers
{
    [Route("admin/forms/forms")]
    public class AdminFormsController : BaseEntityControllerAuthorize<FormDto, FormDto, FormDto, FormDeleteDto, IFormApplicationService>
    {
        public AdminFormsController(IFormApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
