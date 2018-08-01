using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.ContentTexts;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.ContentTexts.Services
{
    public class ContentTextApplicationService : BaseEntityApplicationService<ContentText, ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IContentTextDomainService>, IContentTextApplicationService
    {
        public ContentTextApplicationService(IContentTextDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<ContentTextDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
    }
}
