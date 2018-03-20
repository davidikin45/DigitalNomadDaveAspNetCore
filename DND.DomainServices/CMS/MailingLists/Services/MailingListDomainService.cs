using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.MailingLists;

namespace DND.DomainServices.CMS.MailingLists.Services
{
    public class MailingListDomainService : BaseEntityDomainService<IBaseDbContext, MailingList>, IMailingListDomainService
    {
        public MailingListDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
