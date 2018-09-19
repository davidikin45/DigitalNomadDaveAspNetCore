using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.CarouselItems.Dtos
{
    public class CarouselItemDeleteDto : DtoAggregateRootBase<int>, IMapFrom<CarouselItem>, IMapTo<CarouselItem>
    {

        public CarouselItemDeleteDto()
        {

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}