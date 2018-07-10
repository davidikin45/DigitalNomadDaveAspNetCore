using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.FlightSearch.Search.Dtos
{
    public class LocationAutoSuggestResponseDto : BaseDto<int>
    {
        public LocationAutoSuggestResponseDto()
        {
            Locations = new List<LocationResponseDto>();
        }

        public IList<LocationResponseDto> Locations { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
