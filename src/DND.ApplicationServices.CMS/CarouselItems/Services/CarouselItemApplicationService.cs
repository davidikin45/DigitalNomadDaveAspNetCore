using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.CMS.CarouselItems;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.CarouselItems.Services
{
    public class CarouselItemApplicationService : BaseEntityApplicationService<CarouselItem, CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto, ICarouselItemDomainService>, ICarouselItemApplicationService
    {
        public CarouselItemApplicationService(ICarouselItemDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<CarouselItemDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
     
    }
}