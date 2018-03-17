using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.ApplicationServices
{
    public class CarouselItemDomainService : BaseEntityDomainService<IApplicationDbContext, CarouselItem>, ICarouselItemDomainService
    {
        public CarouselItemDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
     
    }
}