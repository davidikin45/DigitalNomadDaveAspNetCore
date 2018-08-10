using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.LookupTables;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.LookupTables.Services
{
    public class LookupTableDomainService : BaseEntityDomainService<IDynamicFormsDbContext, LookupTable>, ILookupTableDomainService
    {
        public LookupTableDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
