using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.Projects;
using DND.Domain.CMS.Projects.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;

namespace DND.ApplicationServices.CMS.Projects.Services
{
    public class ProjectApplicationService : BaseEntityApplicationService<Project, ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto, IProjectDomainService>, IProjectApplicationService
    {
        public ProjectApplicationService(IProjectDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}