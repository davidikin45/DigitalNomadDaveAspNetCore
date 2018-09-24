using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.CMS.Projects.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.Project.Controllers
{
    [Route("admin/cms/projects")]
    public class AdminProjectsController : MvcControllerEntityAuthorizeBase<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IProjectApplicationService>
    {
        public AdminProjectsController(IProjectApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
