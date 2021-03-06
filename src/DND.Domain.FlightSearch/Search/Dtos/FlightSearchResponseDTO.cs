﻿using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.FlightSearch.Search.Dtos
{
    public class FlightSearchResponseDto : DtoBase<int>
    {

        public FlightSearchResponseDto(IEnumerable<ItineraryDto> source, int skip, int take,int totalCount)
        {
            TotalCount = totalCount;
            Itineraries = new LoadMoreList<ItineraryDto>(source, skip, take);
        }

        public int FilteredCount { get { return Itineraries.TotalCount; } }
        public int TotalCount { get; private set; }

        public bool HasMore { get { return Itineraries.HasMore; } }

        public LoadMoreList<ItineraryDto> Itineraries { get; set; }

        public List<LocationResponseDto> OutboundOriginAirports { get; set; }
        public List<LocationResponseDto> OutboundDestinationAirports { get; set; }
        public List<LocationResponseDto> InboundOriginAirports { get; set; }
        public List<LocationResponseDto> InboundDestinationAirports { get; set; }

        public FlightSearchRequestDto Request { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
