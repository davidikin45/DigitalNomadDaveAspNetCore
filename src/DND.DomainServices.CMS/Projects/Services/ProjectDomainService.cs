using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data;
using DND.Domain.CMS.Projects;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.Projects.Services
{
    public class ProjectDomainService : DomainServiceEntityBase<ApplicationContext, Project>, IProjectDomainService
    {
        public ProjectDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {
        }

    }
}