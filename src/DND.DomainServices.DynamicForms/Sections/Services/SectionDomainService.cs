using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Sections;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.Sections.Services
{
    public class SectionDomainService : BaseEntityDomainService<IDynamicFormsDbContext, Section>, ISectionDomainService
    {
        public SectionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
