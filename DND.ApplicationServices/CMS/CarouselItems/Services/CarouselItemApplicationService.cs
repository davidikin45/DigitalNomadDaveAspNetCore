using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.CarouselItems;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;

namespace DND.ApplicationServices.CMS.CarouselItems.Services
{
    public class CarouselItemApplicationService : BaseEntityApplicationService<IApplicationDbContext, CarouselItem, CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto, ICarouselItemDomainService>, ICarouselItemApplicationService
    {
        public CarouselItemApplicationService(ICarouselItemDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}