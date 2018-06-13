﻿using DND.Common.Extensions;
using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines;
using DND.Interfaces.FlightSearch.DomainServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.FlightSearch.Search.Services
{
    public class FlightSearchDomainService : BaseDomainService, IFlightSearchDomainService
    {
        public FlightSearchDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
            : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ValidationErrors(new GeneralError("Invaid Request"));

            if (!request.IsValid())
                throw new ObjectValidationErrors(request.Validate());

            var flightSearchEngine = new FlightSearchEngine("skyscanner");


            var origin = request.OriginLocation;
            var destination = request.DestinationLocation;

            cancellationToken.ThrowIfCancellationRequested();
            var result = await flightSearchEngine.SearchAsync(
                 Settings.Skyscanner.APIKey,
                 request.Markets,
                 request.Currency,
                 request.Locale,
                 origin,
                 destination,
                 request.OutboundDate,
                 request.InboundDate,
                 int.Parse(request.Adults.GetEnumDescription()),
                 int.Parse(request.Children.GetEnumDescription()),
                 int.Parse(request.Infants.GetEnumDescription()),
                 request.PriceMinFilter,
                 request.PriceMaxFilter,
                 request.MaxStopsFilter,
                 request.FlightClass.GetEnumDescription(),
                 request.SortType.GetEnumDescription(),
                 request.SortOrder.GetEnumDescription(),
                 request.OutboundDepartureTimeFromFilter,
                 request.OutboundDepartureTimeToFilter,
                 request.OutboundArrivalTimeFromFilter,
                 request.OutboundArrivalTimeToFilter,
                 request.InboundDepartureTimeFromFilter,
                 request.InboundDepartureTimeToFilter,
                 request.InboundArrivalTimeFromFilter,
                 request.InboundArrivalTimeToFilter,
                 cancellationToken).ConfigureAwait(false);


            var ItineraryDtos = new List<ItineraryDto>();
            foreach (Itinerary i in result.FilteredItineraries)
            {
                var itinerary = new ItineraryDto().CopyFrom(i);
                ItineraryDtos.Add(itinerary);
            }

            var response = new FlightSearchResponseDto(ItineraryDtos, request.Skip ?? 0, request.Take ?? 10, result.TotalResults);

            response.Request = request;

            return response;
        }

        public async Task<LocationAutoSuggestResponseDto> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ValidationErrors(new GeneralError("Invaid Request"));

            if (!request.IsValid())
                throw new ObjectValidationErrors(request.Validate());

            var response = new LocationAutoSuggestResponseDto();

            Boolean anywhereAdded = false;
            Boolean nearestAdded = false;

            if (!string.IsNullOrEmpty(request.Search))
            {
                if ("anywhere".Contains(request.Search.ToLower()))
                {
                    anywhereAdded = true;
                    response.Locations.Add(new LocationResponsedto { PlaceId = "anywhere", PlaceName = "Anywhere" });
                }

                if ("nearest current".Contains(request.Search.ToLower()))
                {
                    nearestAdded = true;
                    response.Locations.Add(new LocationResponsedto { PlaceId = "nearest", PlaceName = "Nearest Airport" });
                }


                var locationSearchEngine = new LocationSearchEngine("skyscanner");

                cancellationToken.ThrowIfCancellationRequested();
                var results = await locationSearchEngine.SearchByQueryAsync(request.Market[0], request.Currency, request.Locale, request.Search, cancellationToken).ConfigureAwait(false);

                foreach (Place p in results.Places)
                {
                    var location = new LocationResponsedto { PlaceId = p.PlaceId.Replace("-sky", ""), PlaceName = p.PlaceName, CityId = p.CityId.Replace("-sky", ""), CountryId = p.CountryId.Replace("-sky", ""), CountryName = p.CountryName, RegionId = p.RegionId.Replace("-sky", "") };
                    response.Locations.Add(location);
                }
            }

            if (!anywhereAdded)
            {
                response.Locations.Add(new LocationResponsedto { PlaceId = "anywhere", PlaceName = "Anywhere" });
            }

            if (!nearestAdded)
            {
                response.Locations.Add(new LocationResponsedto { PlaceId = "nearest", PlaceName = "Nearest Airport" });
            }

            return response;
        }

        public async Task<LocationResponsedto> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ValidationErrors(new GeneralError("Invaid Request"));

            if (!request.IsValid())
                throw new ObjectValidationErrors(request.Validate());

            var locationSearchEngine = new LocationSearchEngine("skyscanner");

            var p = await locationSearchEngine.GetByIDAsync(request.Market[0], request.Currency, request.Locale, request.Id, cancellationToken).ConfigureAwait(false);

            var location = new LocationResponsedto { PlaceId = p.PlaceId.Replace("-sky", ""), PlaceName = p.PlaceName, CityId = p.CityId.Replace("-sky", ""), CountryId = p.CountryId.Replace("-sky", ""), CountryName = p.CountryName, RegionId = "" };

            return location;
        }
    }
}