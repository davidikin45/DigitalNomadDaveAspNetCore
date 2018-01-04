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
    public interface IFlightSearchEngine
    {
        Task<BrowseRoutesServiceResponse> BrowseRoutesSearchAsync(string country, string currency, string locale,
         string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime? outboundPartialDate, DateTime? inboundPartialDate, CancellationToken cancellationToken);
   
        Task<BrowseDatesServiceResponse> BrowseDatesSearchAsync(string country, string currency, string locale,
         string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, CancellationToken cancellationToken);

        Task<BrowseGridServiceResponse> BrowseGridSearchAsync(string country, string currency, string locale,
         string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, CancellationToken cancellationToken);

        Task<LivePricesServiceResponse> LivePriceSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode,
        string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants, string cabinClass, int? maxStopsFilter, CancellationToken cancellationToken);

        Task<string> StartLivePriceSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode,
        string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants, string cabinClass, CancellationToken cancellationToken);

        Task<LivePricesServiceResponse> PollLivePriceSearchAsync(string pollURL, string cacheKey, Boolean untilComplete, int adults, int children, int infants, int? maxStopsFilter, CancellationToken cancellationToken);

        Task<LocationServiceResponse> SearchLocationByQueryAsync(string country, string currency, string locale, string query, CancellationToken cancellationToken);
        Task<Place> GetLocationByIDAsync(string country, string currency, string locale, string id, CancellationToken cancellationToken);

        Task<CurrenciesServiceResponse> GetCurrenciesAsync(CancellationToken cancellationToken);
        Task<Currency> GetCurrencyByIDAsync(string id, CancellationToken cancellationToken);

        Task<LocalesServiceResponse> GetLocalesAsync(CancellationToken cancellationToken);
        Task<Locale> GetLocaleByIDAsync(string id, CancellationToken cancellationToken);

        Task<GeoServiceResponse> GetGeoLocationsAsync(CancellationToken cancellationToken);
        Task<Airport> GetAirportByIDAsync(string id, CancellationToken cancellationToken);
        Task<City> GetCityByIDAsync(string id, CancellationToken cancellationToken);
        Task<Country> GetCountryByIDAsync(string id, CancellationToken cancellationToken);
        Task<Region> GetRegionByIDAsync(string id, CancellationToken cancellationToken);
        Task<Continent> GetContinentByIDAsync(string id, CancellationToken cancellationToken);

        Task<CountriesServiceResponse> GetCountriesByLocaleAsync(string locale, CancellationToken cancellationToken);
    }
}
