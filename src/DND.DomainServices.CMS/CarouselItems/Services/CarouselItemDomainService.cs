using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data;
using DND.Domain.CMS.CarouselItems;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.CarouselItems.Services
{
    public class CarouselItemDomainService : DomainServiceEntityBase<ApplicationContext, CarouselItem>, ICarouselItemDomainService
    {
        public CarouselItemDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}