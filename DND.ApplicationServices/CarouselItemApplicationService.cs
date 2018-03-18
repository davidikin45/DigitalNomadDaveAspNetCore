using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;

namespace DND.ApplicationServices
{
    public class CarouselItemApplicationService : BaseEntityApplicationService<IApplicationDbContext, CarouselItem, CarouselItemDTO, ICarouselItemDomainService>, ICarouselItemApplicationService
    {
        public CarouselItemApplicationService(ICarouselItemDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}