using DND.Services.FlightSearch.BusinessObjects;
using DND.Services.Skyscanner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Services.SearchEngines.Interfaces
{
    public interface ILocaleMarketCurrencySearchEngine
    {
        Task<LocalesServiceResponse> GetLocalesAsync(CancellationToken cancellationToken);
        Task<Locale> GetLocaleByIDAsync(string id, CancellationToken cancellationToken);

        Task<CountriesServiceResponse> GetMarketsByLocaleAsync(string locale, CancellationToken cancellationToken);
        Task<Country> GetMarketByIDAsync(string id, CancellationToken cancellationToken);

        Task<CurrenciesServiceResponse> GetCurrenciesAsync(CancellationToken cancellationToken);
        Task<Currency> GetCurrencyByIDAsync(string id, CancellationToken cancellationToken);
    }
}
