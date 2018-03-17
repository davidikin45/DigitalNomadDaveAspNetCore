using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;

namespace DND.ApplicationServices
{
    public class CarouselItemApplicationService : BaseEntityApplicationService<IApplicationDbContext, CarouselItem, CarouselItemDTO>, ICarouselItemApplicationService
    {
        public CarouselItemApplicationService(ICarouselItemDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}