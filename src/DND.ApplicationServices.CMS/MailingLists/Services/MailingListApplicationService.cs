using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;

namespace DND.ApplicationServices.CMS.MailingLists.Services
{
    public class MailingListApplicationService : BaseEntityApplicationService<MailingList, MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListDomainService>, IMailingListApplicationService
    {
        public MailingListApplicationService(IMailingListDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
