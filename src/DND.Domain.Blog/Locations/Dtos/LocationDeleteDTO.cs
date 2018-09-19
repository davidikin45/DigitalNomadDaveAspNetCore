using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Locations.Dtos
{
    public class LocationDeleteDto : DtoAggregateRootBase<int> , IMapFrom<Location>, IMapTo<Location>
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