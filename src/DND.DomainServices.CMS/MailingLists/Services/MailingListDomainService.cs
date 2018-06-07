using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.MailingLists;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.MailingLists.Services
{
    public class MailingListDomainService : BaseEntityDomainService<ICMSDbContext, MailingList>, IMailingListDomainService
    {
        public MailingListDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}
