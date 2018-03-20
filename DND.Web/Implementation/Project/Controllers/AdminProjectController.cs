using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.Projects.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Project.Controllers
{
    [Route("admin/project")]
    public class AdminProjectController : BaseEntityControllerAuthorize<ProjectDto, IProjectApplicationService>
    {
        public AdminProjectController(IProjectApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
