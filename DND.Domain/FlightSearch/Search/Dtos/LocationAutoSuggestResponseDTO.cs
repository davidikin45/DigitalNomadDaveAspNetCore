using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.FlightSearch.Search.Dtos
{
    public class LocationAutoSuggestResponseDto : BaseEntity<int>
    {
        public LocationAutoSuggestResponseDto()
        {
            Locations = new List<LocationResponsedto>();
        }

        public IList<LocationResponsedto> Locations { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
