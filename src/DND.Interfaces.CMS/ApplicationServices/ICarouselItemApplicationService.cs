using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.CarouselItems.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface ICarouselItemApplicationService : IApplicationServiceEntity<CarouselItemDto, CarouselItemDto, CarouselItemDto, CarouselItemDeleteDto>
    {

    }
}
