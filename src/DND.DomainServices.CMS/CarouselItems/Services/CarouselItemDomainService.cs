using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.CarouselItems;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.CarouselItems.Services
{
    public class CarouselItemDomainService : BaseEntityDomainService<ICMSDbContext, CarouselItem>, ICarouselItemDomainService
    {
        public CarouselItemDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}