using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.Sections.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/sections")]
    public class SectionsController : BaseEntityWebApiControllerAuthorize<SectionDto, SectionDto, SectionDto, SectionDeleteDto, ISectionApplicationService>
    {
        public SectionsController(ISectionApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}
