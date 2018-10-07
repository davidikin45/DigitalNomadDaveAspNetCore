using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.CMS.Faqs;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Faqs.Services
{
    public class FaqApplicationService : ApplicationServiceEntityBase<Faq, FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqDomainService>, IFaqApplicationService
    {
        public FaqApplicationService(IFaqDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<FaqDto>> hubContext)
        : base("cms.faqs.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
