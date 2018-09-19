using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.Sections.Controllers
{
    [Route("admin/forms/sections")]
    public class AdminSectionsController : MvcControllerEntityAuthorizeBase<SectionDto, SectionDto, SectionDto, SectionDeleteDto, ISectionApplicationService>
    {
        public AdminSectionsController(ISectionApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
