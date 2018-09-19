using DND.Common.Domain.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.FlightSearch.Search.Dtos
{
    public class LocationAutoSuggestResponseDto : DtoBase<int>
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
