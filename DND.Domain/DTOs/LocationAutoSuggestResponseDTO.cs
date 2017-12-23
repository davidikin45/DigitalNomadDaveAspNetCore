using DND.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
