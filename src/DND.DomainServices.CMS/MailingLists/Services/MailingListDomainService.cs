using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data;
using DND.Domain.CMS.MailingLists;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.MailingLists.Services
{
    public class MailingListDomainService : DomainServiceEntityBase<ApplicationContext, MailingList>, IMailingListDomainService
    {
        public MailingListDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}
