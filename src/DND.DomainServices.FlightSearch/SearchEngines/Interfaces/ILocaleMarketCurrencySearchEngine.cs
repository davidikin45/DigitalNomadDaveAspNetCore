using DND.Domain.Skyscanner.Model;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.SearchEngines.Interfaces
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
