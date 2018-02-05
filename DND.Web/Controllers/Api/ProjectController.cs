using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;
namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/project")]
    public class ProjectController : BaseEntityWebApiControllerAuthorize<ProjectDTO, IProjectService>
    {
        public ProjectController(IProjectService service, IMapper mapper, IEmailService emailService)
            :base(service, mapper, emailService)
        {

        }
    }
}
