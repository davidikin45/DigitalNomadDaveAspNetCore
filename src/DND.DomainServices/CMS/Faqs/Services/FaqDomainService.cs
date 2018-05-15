using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.Faqs;

namespace DND.DomainServices.CMS.Faqs.Services
{
    public class FaqDomainService : BaseEntityDomainService<IBaseDbContext, Faq>, IFaqDomainService
    {
        public FaqDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
