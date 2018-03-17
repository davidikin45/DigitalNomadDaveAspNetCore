using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.ApplicationServices
{
    public class ProjectDomainService : BaseEntityDomainService<IApplicationDbContext, Project>, IProjectDomainService
    {
        public ProjectDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {
        }    
    }
}