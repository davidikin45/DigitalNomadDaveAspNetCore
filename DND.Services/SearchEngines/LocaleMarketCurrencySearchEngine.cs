using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DND.Services.Skyscanner.Model;
using Solution.Base.Helpers;
using System.Threading.Tasks;
using DND.Services.SearchEngines;
using DND.Services.Factories;
using DND.Services.SearchEngines.Interfaces;
using System.Threading;

namespace DND.Services.SearchEngines
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
