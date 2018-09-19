using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.MailingLists.Services
{
    public class MailingListApplicationService : ApplicationServiceEntityBase<MailingList, MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListDomainService>, IMailingListApplicationService
    {
        public MailingListApplicationService(IMailingListDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<MailingListDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
    }
}
