using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.MailingLists.Services
{
    public class MailingListApplicationService : ApplicationServiceEntityBase<MailingList, MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListDomainService>, IMailingListApplicationService
    {
        public MailingListApplicationService(IMailingListDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<MailingListDto>> hubContext)
        : base("cms.mailing-list.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
