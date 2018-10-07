using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.CMS.CarouselItems;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.CarouselItems.Services
{
    public class CarouselItemApplicationService : ApplicationServiceEntityBase<CarouselItem, CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto, ICarouselItemDomainService>, ICarouselItemApplicationService
    {
        public CarouselItemApplicationService(ICarouselItemDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<CarouselItemDto>> hubContext)
        : base("cms.carousel-items.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
     
    }
}