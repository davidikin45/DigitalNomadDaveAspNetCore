using DND.Domain.Enums;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class FlightSearchRequestDTO : BaseEntity<int>
    {
        public string Locale { get; set; }
        public string[] Markets { get; set; }
        public string Currency { get; set; }

        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }

        public DateTime OutboundDate { get; set; }
        public DateTime? OutboundDepartureTimeFromFilter { get; set; }
        public DateTime? OutboundDepartureTimeToFilter { get; set; }
        public DateTime? OutboundArrivalTimeFromFilter { get; set; }
        public DateTime? OutboundArrivalTimeToFilter { get; set; }
        public int? OutboundDurationMinFilter { get; set; }
        public int? OutboundDurationMaxFilter { get; set; }

        public DateTime? InboundDate { get; set; }
        public DateTime? InboundDepartureTimeFromFilter { get; set; }
        public DateTime? InboundDepartureTimeToFilter { get; set; }
        public DateTime? InboundArrivalTimeFromFilter { get; set; }
        public DateTime? InboundArrivalTimeToFilter { get; set; }
        public int? InboundDurationMinFilter { get; set; }
        public int? InboundDurationMaxFilter { get; set; }

        public FlightSearchType FlightSearchType { get; set; }
        public FlightClass FlightClass { get; set; }

        public Adult Adults { get; set; }
        public Children Children { get; set; }
        public Infant Infants { get; set; }

        public double? PriceMinFilter { get; set; }
        public double? PriceMaxFilter { get; set; }

        public int? MaxStopsFilter { get; set; }

        public SortType SortType { get; set; }
        public SortOrder SortOrder { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Locale))
                yield return new ValidationResult("Locale Required", new string[] { this.Name(x => x.Locale) });

           // yield break;
        }
    }
}