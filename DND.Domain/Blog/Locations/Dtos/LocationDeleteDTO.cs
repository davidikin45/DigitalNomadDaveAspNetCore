using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Constants;
using DND.Domain.FlightSearch.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

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