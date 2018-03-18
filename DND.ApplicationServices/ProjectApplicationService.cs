using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;

namespace DND.ApplicationServices
{
    public class ProjectApplicationService : BaseEntityApplicationService<IApplicationDbContext, Project, ProjectDTO, IProjectDomainService>, IProjectApplicationService
    {
        public ProjectApplicationService(IProjectDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}