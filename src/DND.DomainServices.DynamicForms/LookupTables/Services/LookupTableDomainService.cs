using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data.DynamicForms;
using DND.Domain.DynamicForms.LookupTables;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.LookupTables.Services
{
    public class LookupTableDomainService : DomainServiceEntityBase<DynamicFormsContext, LookupTable>, ILookupTableDomainService
    {
        public LookupTableDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
