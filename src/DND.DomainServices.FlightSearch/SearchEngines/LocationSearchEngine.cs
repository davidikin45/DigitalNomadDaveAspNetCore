﻿using DND.Domain.Skyscanner.Model;
using DND.DomainServices.FlightSearch.Search.Services;
using DND.DomainServices.SearchEngines.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.SearchEngines
{

    public class LocationSearchEngine : ILocationSearchEngine
    {
        private IFlightSearchEngine _flightSearchEngine;
        public LocationSearchEngine(string id)
        {
            _flightSearchEngine = new FlightSearchEngineFactory().GetFlightSearchEngine(id);
        }

        public async Task<LocationServiceResponse> SearchByQueryAsync(string country, string currency, string locale, string query, CancellationToken cancellationToken)
       {
            return await _flightSearchEngine.SearchLocationByQueryAsync(country, currency, locale, query, cancellationToken).ConfigureAwait(false);          
       }

        public async Task<Place> GetByIDAsync(string country, string currency, string locale, string id, CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetLocationByIDAsync(country, currency, locale, id, cancellationToken).ConfigureAwait(false);
        }
    }
}
