using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.Services
{
    public class CarouselItemService : BaseEntityService<IApplicationDbContext, CarouselItem, CarouselItemDTO>, ICarouselItemService
    {
        public CarouselItemService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }
     
    }
}