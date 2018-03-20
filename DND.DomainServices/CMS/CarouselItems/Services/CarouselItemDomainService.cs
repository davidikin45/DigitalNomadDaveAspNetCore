using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.CarouselItems;

namespace DND.DomainServices.CMS.CarouselItems.Services
{
    public class CarouselItemDomainService : BaseEntityDomainService<IApplicationDbContext, CarouselItem>, ICarouselItemDomainService
    {
        public CarouselItemDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
     
    }
}