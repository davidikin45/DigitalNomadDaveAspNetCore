using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.Projects;
using DND.Domain.CMS.Projects.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Projects.Services
{
    public class ProjectApplicationService : ApplicationServiceEntityBase<Project, ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IProjectDomainService>, IProjectApplicationService
    {
        public ProjectApplicationService(IProjectDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<ProjectDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
     
    }
}