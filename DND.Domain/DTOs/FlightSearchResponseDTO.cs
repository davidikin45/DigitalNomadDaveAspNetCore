using DND.Base.Implementation.Models;
using DND.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.DTOs
{
   public class FlightSearchResponseDTO : BaseEntity<int>
    {

        public FlightSearchResponseDTO(IEnumerable<ItineraryDTO> source, int skip, int take,int totalCount)
        {
            TotalCount = totalCount;
            Itineraries = new LoadMoreList<ItineraryDTO>(source, skip, take);
        }

        public int FilteredCount { get { return Itineraries.TotalCount; } }
        public int TotalCount { get; private set; }

        public bool HasMore { get { return Itineraries.HasMore; } }

        public LoadMoreList<ItineraryDTO> Itineraries { get; set; }

        public List<LocationResponseDTO> OutboundOriginAirports { get; set; }
        public List<LocationResponseDTO> OutboundDestinationAirports { get; set; }
        public List<LocationResponseDTO> InboundOriginAirports { get; set; }
        public List<LocationResponseDTO> InboundDestinationAirports { get; set; }

        public FlightSearchRequestDTO Request { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
