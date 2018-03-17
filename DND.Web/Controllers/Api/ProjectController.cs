using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;
using Solution.Base.Interfaces.Services;

namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/project")]
    public class ProjectController : BaseEntityWebApiControllerAuthorize<ProjectDTO, IProjectApplicationService>
    {
        public ProjectController(IProjectApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
