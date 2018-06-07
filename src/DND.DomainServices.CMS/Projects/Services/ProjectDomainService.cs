using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.Projects;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.Projects.Services
{
    public class ProjectDomainService : BaseEntityDomainService<ICMSDbContext, Project>, IProjectDomainService
    {
        public ProjectDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {
        }

    }
}