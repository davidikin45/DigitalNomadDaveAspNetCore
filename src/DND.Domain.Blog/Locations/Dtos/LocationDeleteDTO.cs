using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Locations.Dtos
{
    public class LocationDeleteDto : BaseDtoAggregateRoot<int> , IMapFrom<Location>, IMapTo<Location>
    {
        public LocationDeleteDto()
		{

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}