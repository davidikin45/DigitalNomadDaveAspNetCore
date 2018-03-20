using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.Projects;
using DND.Domain.CMS.Projects.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;

namespace DND.ApplicationServices.CMS.Projects.Services
{
    public class ProjectApplicationService : BaseEntityApplicationService<IApplicationDbContext, Project, ProjectDto, IProjectDomainService>, IProjectApplicationService
    {
        public ProjectApplicationService(IProjectDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}