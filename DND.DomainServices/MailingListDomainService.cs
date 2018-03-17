using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.ApplicationServices
{
    public class MailingListDomainService : BaseEntityDomainService<IBaseDbContext, MailingList>, IMailingListDomainService
    {
        public MailingListDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
