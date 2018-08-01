using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.Faqs;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Faqs.Services
{
    public class FaqApplicationService : BaseEntityApplicationService<Faq, FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqDomainService>, IFaqApplicationService
    {
        public FaqApplicationService(IFaqDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<FaqDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
    }
}
