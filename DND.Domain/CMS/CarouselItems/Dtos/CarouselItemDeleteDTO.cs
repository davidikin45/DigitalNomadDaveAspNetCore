using DND.Domain.Constants;
using DND.Domain.Models;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Common.Implementation.Dtos;

namespace DND.Domain.CMS.CarouselItems.Dtos
{
    public class CarouselItemDeleteDto : BaseDtoAggregateRoot<int>, IMapFrom<CarouselItem>, IMapTo<CarouselItem>
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