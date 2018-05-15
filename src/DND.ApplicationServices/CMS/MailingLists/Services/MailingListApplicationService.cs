using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Interfaces.Persistance;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;

namespace DND.ApplicationServices.CMS.MailingLists.Services
{
    public class MailingListApplicationService : BaseEntityApplicationService<IBaseDbContext, MailingList, MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListDomainService>, IMailingListApplicationService
    {
        public MailingListApplicationService(IMailingListDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
