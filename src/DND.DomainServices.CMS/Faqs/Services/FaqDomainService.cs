using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data;
using DND.Domain.CMS.Faqs;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.Faqs.Services
{
    public class FaqDomainService : DomainServiceEntityBase<ApplicationContext, Faq>, IFaqDomainService
    {
        public FaqDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
