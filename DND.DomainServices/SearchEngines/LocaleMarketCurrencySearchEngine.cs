using DND.Domain.Skyscanner.Model;
using DND.DomainServices.FlightSearch.Search.Services;
using DND.DomainServices.SearchEngines.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.SearchEngines
{

    public class LocaleMarketCurrencySearchEngine : ILocaleMarketCurrencySearchEngine
    {
        private IFlightSearchEngine _flightSearchEngine;
        public LocaleMarketCurrencySearchEngine(string id)
        {
            _flightSearchEngine = new FlightSearchEngineFactory().GetFlightSearchEngine(id);
        }

        public async Task<LocalesServiceResponse> GetLocalesAsync(CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetLocalesAsync(cancellationToken);
        }

        public async Task<Locale> GetLocaleByIDAsync(string id, CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetLocaleByIDAsync(id, cancellationToken);
        }

        public async Task<CountriesServiceResponse> GetMarketsByLocaleAsync(string locale, CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetCountriesByLocaleAsync(locale, cancellationToken);
        }

        public async Task<Country> GetMarketByIDAsync(string id, CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetCountryByIDAsync(id, cancellationToken);
        }

        public async Task<CurrenciesServiceResponse> GetCurrenciesAsync(CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetCurrenciesAsync(cancellationToken);
        }

        public async Task<Currency> GetCurrencyByIDAsync(string id, CancellationToken cancellationToken)
        {
            return await _flightSearchEngine.GetCurrencyByIDAsync(id, cancellationToken);
        }
    }
}
