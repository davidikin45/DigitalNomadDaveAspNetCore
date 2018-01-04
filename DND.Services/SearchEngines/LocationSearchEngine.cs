using DND.Services.Factories;
using DND.Services.SearchEngines.Interfaces;
using DND.Services.Skyscanner.Model;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Services.SearchEngines
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
            return await _flightSearchEngine.SearchLocationByQueryAsync(country, currency, locale, query, cancellationToken);          
       }

        public async Task<Place> GetByIDAsync(string country, string currency, string locale, string id, CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetLocationByIDAsync(country, currency, locale, id, cancellationToken);
        }
    }
}
