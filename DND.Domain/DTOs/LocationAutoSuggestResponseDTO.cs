using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class LocationAutoSuggestResponseDTO : BaseEntity<int>
    {
        public LocationAutoSuggestResponseDTO()
        {
            Locations = new List<LocationResponseDTO>();
        }

        public IList<LocationResponseDTO> Locations { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
