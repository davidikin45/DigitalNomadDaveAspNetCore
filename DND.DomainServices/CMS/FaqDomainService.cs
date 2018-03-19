using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;

namespace DND.ApplicationServices
{
    public class FaqDomainService : BaseEntityDomainService<IBaseDbContext, Faq>, IFaqDomainService
    {
        public FaqDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
