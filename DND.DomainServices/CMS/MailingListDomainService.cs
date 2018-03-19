using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;

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
