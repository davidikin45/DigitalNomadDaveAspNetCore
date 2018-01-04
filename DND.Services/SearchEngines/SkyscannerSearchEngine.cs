using DND.Services.SearchEngines.Interfaces;
using DND.Services.Skyscanner.Model;
using Newtonsoft.Json;
using Solution.Base.Helpers;
using Solution.Base.Infrastructure;
using Solution.Base.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


// Sesion open for 30 minutes - hour
namespace DND.Services.SearchEngines
{
    class SkyscannerSearchEngine : IFlightSearchEngine
    {
        ObjectCache cache = MemoryCache.Default;

        private static class Settings
        {
            public const string PollBrowseRoutesURL = "http://partners.api.skyscanner.net/apiservices/browseroutes/v1.0/{market}/{currency}/{locale}/{originPlace}/{destinationPlace}/{outboundPartialDate}/{inboundPartialDate}?apiKey={apiKey}";
            public const double PollBrowseRoutesCacheExpiration = 300;
            public const string PollBrowseDatesURL = "http://partners.api.skyscanner.net/apiservices/browsedates/v1.0/{market}/{currency}/{locale}/{originPlace}/{destinationPlace}/{outboundPartialDate}/{inboundPartialDate}?apiKey={apiKey}";
            public const double PollBrowseDatesCacheExpiration = 300;
            public const string PollBrowseGridURL = "http://partners.api.skyscanner.net/apiservices/browsegrid/v1.0/{market}/{currency}/{locale}/{originPlace}/{destinationPlace}/{outboundPartialDate}/{inboundPartialDate}?apiKey={apiKey}";
            public const double PollBrowseGridCacheExpiration = 300;

            public const string StartLivePricesURL = "http://partners.api.skyscanner.net/apiservices/pricing/v1.0?apiKey={apiKey}";
            public const double StartLivePricesCacheExpiration = 15;
            public const double PollLivePricesCacheExpiration = 15;

            public const string LocationAutoSuggestByQueryURL = "http://partners.api.skyscanner.net/apiservices/autosuggest/v1.0/{market}/{currency}/{locale}/?query={query}&apiKey={apiKey}";
            public const double LocationAutoSuggestByQueryCacheExpiration = 60;
            public const string LocationAutoSuggestByIDURL = "http://partners.api.skyscanner.net/apiservices/autosuggest/v1.0/{market}/{currency}/{locale}/?id={id}&apiKey={apiKey}";
            public const double LocationAutoSuggestByIDCacheExpiration = 90000;

            public const string CurrenciesServiceURL = "http://partners.api.skyscanner.net/apiservices/reference/v1.0/currencies?apiKey={apiKey}";
            public static SemaphoreSlim CurrenciesServiceLock = new SemaphoreSlim(1, 1);
            public const string CurrenciesServiceFileName = "currenciesService.json";
            public const double CurrenciesServiceCacheExpiration = 90000;

            public const string LocalesServiceURL = "http://partners.api.skyscanner.net/apiservices/reference/v1.0/locales?apiKey={apiKey}";
            public static SemaphoreSlim LocalesServiceLock = new SemaphoreSlim(1, 1);
            public const string LocalesServiceFileName = "localesService.json";
            public const double LocalesServiceCacheExpiration = 90000;

            public const string GeoServiceURL = "http://partners.api.skyscanner.net/apiservices/geo/v1.0?apikey={apiKey}";
            public static SemaphoreSlim GeoServiceLock = new SemaphoreSlim(1, 1);
            public const string GeoServiceFileName = "geoService.json";
            public const double GeoServiceCacheExpiration = 90000;

            public const string CountriesServiceURL = "http://partners.api.skyscanner.net/apiservices/reference/v1.0/countries/{locale}?apiKey={apiKey}";
            public static ConcurrentDictionary<string, SemaphoreSlim> CountriesServiceLock = new ConcurrentDictionary<string, SemaphoreSlim>();
            public const string CountriesServiceFileName = "countriesService_{locale}.json";
            public const double CountriesServiceCacheExpiration = 90000;

            public const int Exception304WaitTime = 1;
            public const int Exception304Retries = 100;
            public const int Exception429WaitTime = 60;
            public const int Exception429Retries = 20;
        }

        private string[] _apiKeys;
        private string _dataPath;

        public SkyscannerSearchEngine(string dataPath, params string[] apiKeys)
        {
            _apiKeys = apiKeys;
            _dataPath = dataPath;
        }

        private string ApiKey()
        {
            Random RandString = new Random();
            var apiKey = _apiKeys[RandString.Next(0, _apiKeys.Length)];
            return apiKey;
        }

        private string DefaultApiKey()
        {
            var apiKey = _apiKeys[0];
            return apiKey;
        }

        public async Task<BrowseRoutesServiceResponse> BrowseRoutesSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime? outboundPartialDate, DateTime? inboundPartialDate, CancellationToken cancellationToken)
        {
            string outboundPartialDateString = "";
            if (outboundPartialDate.HasValue)
            {
                outboundPartialDateString = outboundPartialDate.Value.ToString("yyyy-MM-dd");
            }
            string inboundPartialDateString = "";
            if (inboundPartialDate.HasValue)
            {
                inboundPartialDateString = inboundPartialDate.Value.ToString("yyyy-MM-dd");
            }

            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("BrowseRoutesSearchAsync");
            cacheKeyList.Add(country);
            cacheKeyList.Add(currency);
            cacheKeyList.Add(locale);
            cacheKeyList.Add(originPlaceSkyscannerCode);
            cacheKeyList.Add(destinationPlaceSkyscannerCode);
            cacheKeyList.Add(outboundPartialDateString);
            cacheKeyList.Add(inboundPartialDateString);

            string cacheKey = string.Join(":", cacheKeyList);

            BrowseRoutesServiceResponse data = (BrowseRoutesServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {
                var variables = new Dictionary<string,
                 string>();
                variables.Add("{apiKey}", ApiKey());
                variables.Add("{market}", country);
                variables.Add("{currency}", currency);
                variables.Add("{locale}", locale);
                // If you exceed the rate limit, it will reset after 60 seconds.
                //Sessions expire after 30 minutes.
                //https://github.com/Skyscanner/api-documentation/blob/master/live_flights_pricing/README.md
                //37.678,-122.452-latlong
                //188.39.95.93-ip
                variables.Add("{originPlace}", originPlaceSkyscannerCode);
                variables.Add("{destinationPlace}", destinationPlaceSkyscannerCode);
                if (outboundPartialDate.HasValue)
                {
                    variables.Add("{outboundPartialDate}", outboundPartialDate.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    variables.Add("{outboundPartialDate}", "anytime");
                }

                if (inboundPartialDate.HasValue)
                {
                    variables.Add("{inboundPartialDate}", inboundPartialDate.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    variables.Add("{inboundPartialDate}", "");
                }

                var URL = Settings.PollBrowseRoutesURL.ReplaceFromDictionary(variables);
                Func<Exception, Boolean> exception429 = ex => ex is SimpleHttpResponseException && (((int)((SimpleHttpResponseException)ex).StatusCode) == 429);
                var response = await TaskHelper.DoAsync(() => WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken), new AsyncRetryPolicy(TimeSpan.FromSeconds(Settings.Exception429WaitTime), Settings.Exception429Retries, exception429), cancellationToken);
                data = JsonConvert.DeserializeObject<BrowseRoutesServiceResponse>(response.Item1.BytesToString());

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.PollBrowseRoutesCacheExpiration));
                cache.Add(cacheKey, data, policy);
            }

            return data;
        }

        public async Task<BrowseDatesServiceResponse> BrowseDatesSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, CancellationToken cancellationToken)
        {
            string outboundPartialDateString = outboundPartialDate.ToString("yyyy-MM-dd");
            string inboundPartialDateString = "";
            if (inboundPartialDate.HasValue)
            {
                inboundPartialDateString = inboundPartialDate.Value.ToString("yyyy-MM-dd");
            }

            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("BrowseDatesSearchAsync");
            cacheKeyList.Add(country);
            cacheKeyList.Add(currency);
            cacheKeyList.Add(locale);
            cacheKeyList.Add(originPlaceSkyscannerCode);
            cacheKeyList.Add(destinationPlaceSkyscannerCode);
            cacheKeyList.Add(outboundPartialDateString);
            cacheKeyList.Add(inboundPartialDateString);

            string cacheKey = string.Join(":", cacheKeyList);

            BrowseDatesServiceResponse data = (BrowseDatesServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {

                var variables = new Dictionary<string, string>();
                variables.Add("{apiKey}", ApiKey());
                variables.Add("{market}", country);
                variables.Add("{currency}", currency);
                variables.Add("{locale}", locale);
                variables.Add("{originPlace}", originPlaceSkyscannerCode);
                variables.Add("{destinationPlace}", destinationPlaceSkyscannerCode);
                variables.Add("{outboundPartialDate}", outboundPartialDate.ToString("yyyy-MM-dd"));

                if (inboundPartialDate.HasValue)
                {
                    variables.Add("{inboundPartialDate}", inboundPartialDate.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    variables.Add("{inboundPartialDate}", "");
                }

                var URL = Settings.PollBrowseDatesURL.ReplaceFromDictionary(variables);
                Func<Exception, Boolean> exception429 = ex => ex is SimpleHttpResponseException && (((int)((SimpleHttpResponseException)ex).StatusCode) == 429);
                var response = await TaskHelper.DoAsync(() => WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken), new AsyncRetryPolicy(TimeSpan.FromSeconds(Settings.Exception429WaitTime), Settings.Exception429Retries, exception429), cancellationToken);
                data = JsonConvert.DeserializeObject<BrowseDatesServiceResponse>(response.Item1.BytesToString());

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.PollBrowseDatesCacheExpiration));
                cache.Add(cacheKey, data, policy);
            }

            return data;
        }

        public async Task<BrowseGridServiceResponse> BrowseGridSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, CancellationToken cancellationToken)
        {
            string outboundPartialDateString = outboundPartialDate.ToString("yyyy-MM");
            string inboundPartialDateString = "";
            if (inboundPartialDate.HasValue)
            {
                inboundPartialDateString = inboundPartialDate.Value.ToString("yyyy-MM");
            }

            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("BrowseGridSearchAsync");
            cacheKeyList.Add(country);
            cacheKeyList.Add(currency);
            cacheKeyList.Add(locale);
            cacheKeyList.Add(originPlaceSkyscannerCode);
            cacheKeyList.Add(destinationPlaceSkyscannerCode);
            cacheKeyList.Add(outboundPartialDateString);
            cacheKeyList.Add(inboundPartialDateString);

            string cacheKey = string.Join(":", cacheKeyList);

            BrowseGridServiceResponse data = (BrowseGridServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {

                var variables = new Dictionary<string, string>();
                variables.Add("{apiKey}", ApiKey());
                variables.Add("{market}", country);
                variables.Add("{currency}", currency);
                variables.Add("{locale}", locale);
                variables.Add("{originPlace}", originPlaceSkyscannerCode);
                variables.Add("{destinationPlace}", destinationPlaceSkyscannerCode);
                variables.Add("{outboundPartialDate}", outboundPartialDate.ToString("yyyy-MM"));

                if (inboundPartialDate.HasValue)
                {
                    variables.Add("{inboundPartialDate}", inboundPartialDate.Value.ToString("yyyy-MM"));
                }
                else
                {
                    variables.Add("{inboundPartialDate}", "");
                }

                var URL = Settings.PollBrowseGridURL.ReplaceFromDictionary(variables);
                Func<Exception, Boolean> exception429 = ex => ex is SimpleHttpResponseException && (((int)((SimpleHttpResponseException)ex).StatusCode) == 429);
                var response = await TaskHelper.DoAsync(() => WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken), new AsyncRetryPolicy(TimeSpan.FromSeconds(Settings.Exception429WaitTime), Settings.Exception429Retries, exception429), cancellationToken);
                data = JsonConvert.DeserializeObject<BrowseGridServiceResponse>(response.Item1.BytesToString());

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.PollBrowseGridCacheExpiration));
                cache.Add(cacheKey, data, policy);
            }

            return data;
        }

        public async Task<LivePricesServiceResponse> LivePriceSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants, string cabinClass, int? maxStopsFilter, CancellationToken cancellationToken)
        {
            //maxStops is actually stops filter

            string outboundPartialDateString = outboundPartialDate.ToString("yyyy-MM-dd");
            string inboundPartialDateString = "";
            if (inboundPartialDate.HasValue)
            {
                inboundPartialDateString = inboundPartialDate.Value.ToString("yyyy-MM-dd");
            }

            string stopsFilterString = "";
            if (maxStopsFilter.HasValue)
            {
                stopsFilterString = maxStopsFilter.Value.ToString();
            }

            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("PollLivePriceSearchAsync");
            cacheKeyList.Add(country);
            cacheKeyList.Add(currency);
            cacheKeyList.Add(locale);
            cacheKeyList.Add(originPlaceSkyscannerCode);
            cacheKeyList.Add(destinationPlaceSkyscannerCode);
            cacheKeyList.Add(outboundPartialDateString);
            cacheKeyList.Add(inboundPartialDateString);
            cacheKeyList.Add(adults); //Adults
            cacheKeyList.Add(children); //Children
            cacheKeyList.Add(infants); //Infants
            cacheKeyList.Add(stopsFilterString); //Stops
            cacheKeyList.Add(cabinClass);

            string cacheKey = string.Join(":", cacheKeyList);


            var data = (LivePricesServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {
                var url = await StartLivePriceSearchAsync(country, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, adults, children, infants, cabinClass, cancellationToken);
                await Task.Delay(1000);
                return await PollLivePriceSearchAsync(url, cacheKey, true, adults, children, infants, maxStopsFilter, cancellationToken);

            }
            else
            {
                return data;
            }
        }

        public async Task<string> StartLivePriceSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode, string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants, string cabinClass, CancellationToken cancellationToken)
        {
            try
            {
                string outboundPartialDateString = outboundPartialDate.ToString("yyyy-MM-dd");
                string inboundPartialDateString = "";
                if (inboundPartialDate.HasValue)
                {
                    inboundPartialDateString = inboundPartialDate.Value.ToString("yyyy-MM-dd");
                }

                List<object> cacheKeyList = new List<object>();
                cacheKeyList.Add("StartLivePriceSearchAsync");
                cacheKeyList.Add(country);
                cacheKeyList.Add(currency);
                cacheKeyList.Add(locale);
                cacheKeyList.Add("sky"); //Iata, GeoNameCode, GeoNameId, Rnid*, Sky, Latlong
                cacheKeyList.Add("on");
                cacheKeyList.Add(originPlaceSkyscannerCode);
                cacheKeyList.Add(destinationPlaceSkyscannerCode);
                cacheKeyList.Add(outboundPartialDateString);
                cacheKeyList.Add(inboundPartialDateString);
                cacheKeyList.Add(adults.ToString());
                cacheKeyList.Add(children.ToString());
                cacheKeyList.Add(infants.ToString());
                cacheKeyList.Add(cabinClass);

                string cacheKey = string.Join(":", cacheKeyList);

                string pollURL = (string)cache.Get(cacheKey);
                if (pollURL == null)
                {

                    var apiKey = DefaultApiKey();

                    var variablesURL = new Dictionary<string,
                     string>();
                    variablesURL.Add("{apiKey}", apiKey);
                    var URL = Settings.StartLivePricesURL.ReplaceFromDictionary(variablesURL);

                    var variables = new Dictionary<string,
                     string>();
                    variables.Add("country", country);
                    variables.Add("currency", currency);
                    variables.Add("locale", locale);
                    variables.Add("locationschema", "Sky"); //Iata, GeoNameCode, GeoNameId, Rnid*, Sky, Latlong
                    variables.Add("apikey", apiKey);
                    variables.Add("grouppricing", "true"); // doesn't work
                                                           //variables.Add("grouppricing", "off");
                    variables.Add("originplace", originPlaceSkyscannerCode);
                    variables.Add("destinationplace", destinationPlaceSkyscannerCode);
                    variables.Add("outbounddate", outboundPartialDate.ToString("yyyy-MM-dd"));
                    if (inboundPartialDate.HasValue)
                    {
                        variables.Add("inbounddate", inboundPartialDate.Value.ToString("yyyy-MM-dd"));
                    }
                    variables.Add("adults", adults.ToString());
                    variables.Add("children", children.ToString());
                    variables.Add("infants", infants.ToString());
                    variables.Add("cabinclass", cabinClass);


                    Func<Exception, Boolean> exception429 = ex => ex is SimpleHttpResponseException && (((int)((SimpleHttpResponseException)ex).StatusCode) == 429);
                    var response = await TaskHelper.DoAsync(() => WebRequestHelper.PostFormAsync(URL, variables, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken), new AsyncRetryPolicy(TimeSpan.FromSeconds(Settings.Exception429WaitTime), Settings.Exception429Retries, exception429), cancellationToken);

                    pollURL = response.Item2.Location.AbsoluteUri + "?";

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.StartLivePricesCacheExpiration));
                    cache.Add(cacheKey, pollURL, policy);
                }

                return pollURL;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<LivePricesServiceResponse> PollLivePriceSearchAsync(string pollURL, string cacheKey, bool untilComplete, int adults, int children, int infants, int? maxStopsFilter, CancellationToken cancellationToken)
        {
            string responseString = "";

            try
            {
                LivePricesServiceResponse data = (LivePricesServiceResponse)cache.Get(cacheKey);
                if (data == null)
                {
                    var variables = new Dictionary<string,
                     string>();
                    variables.Add("apikey", DefaultApiKey());
                    variables.Add("adults", adults.ToString());
                    variables.Add("children", children.ToString());
                    variables.Add("infants", infants.ToString());

                    //can we change passengers and stops when polling?

                    //Return Schema
                    variables.Add("locationschema", "Iata"); //Iata, GeoNameCode, GeoNameId, Rnid*, Sky, Latlong

                    if (maxStopsFilter.HasValue)
                    {
                        //maxStops is actually stops filter
                        variables.Add("stops", maxStopsFilter.ToString());
                    }

                    //carrier, duration, outboundarrivetime, outbounddeparttime, inboundarrivetime, inbounddeparttime, price*
                    //variables.Add("sorttype", "price");
                    //asc, desc
                    //variables.Add("sortorder", "desc");
                    variables.Add("includeQuery", "true");

                    //variables.Add("includecarriers", "");
                    //variables.Add("excludecarriers", "");

                    var newPollURL = pollURL + variables.ToQueryString();

                    var complete = false;
                    while (!complete)
                    {
                        Func<Exception, Boolean> exception304 = ex => ex is SimpleHttpResponseException && (((int)((SimpleHttpResponseException)ex).StatusCode) == 304);
                        Func<Exception, Boolean> exception429 = ex => ex is SimpleHttpResponseException && (((int)((SimpleHttpResponseException)ex).StatusCode) == 429);
                        var response = await TaskHelper.DoAsync(() => TaskHelper.DoAsync(() => WebRequestHelper.GetAsync(newPollURL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken), new AsyncRetryPolicy(TimeSpan.FromSeconds(Settings.Exception304WaitTime), Settings.Exception304Retries, exception304), cancellationToken), new AsyncRetryPolicy(TimeSpan.FromSeconds(Settings.Exception429WaitTime), Settings.Exception429Retries, exception429), cancellationToken);
                        responseString = response.Item1.BytesToString();
                        data = JsonConvert.DeserializeObject<LivePricesServiceResponse>(responseString);

                        if (data != null && ((data.Status == "UpdatesComplete") || (data.Status == "UpdatesPending" && !untilComplete)))
                        {
                            complete = true;
                        }
                    }

                    if (data.Places == null)
                    {
                        data.Places = new List<Place>();
                    }

                    if (data.Currencies == null)
                    {
                        data.Currencies = new List<Currency>();
                    }

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.PollLivePricesCacheExpiration));
                    cache.Add(cacheKey, data, policy);
                }

                return data;
            }
            catch(Newtonsoft.Json.JsonReaderException)
            {
                throw;
            }
            catch(Exception ex)
            {             
                throw;
            }
        }

        public async Task<LocationServiceResponse> SearchLocationByQueryAsync(string country, string currency, string locale, string query, CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("SearchLocationByQueryAsync");
            cacheKeyList.Add(country);
            cacheKeyList.Add(currency);
            cacheKeyList.Add(locale);
            cacheKeyList.Add(query);

            string cacheKey = string.Join(":", cacheKeyList);

            LocationServiceResponse data = (LocationServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {

                var variables = new Dictionary<string, string>();
                variables.Add("{apiKey}", ApiKey());
                variables.Add("{market}", country);
                variables.Add("{currency}", currency);
                variables.Add("{locale}", locale);
                variables.Add("{query}", query);

                var URL = Settings.LocationAutoSuggestByQueryURL.ReplaceFromDictionary(variables);
                var response = await WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken);
                var responseString = response.Item1.BytesToString();
                data = JsonConvert.DeserializeObject<LocationServiceResponse>(responseString);

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.LocationAutoSuggestByQueryCacheExpiration));
                cache.Add(cacheKey, data, policy);
            }

            return data;
        }

        public async Task<Place> GetLocationByIDAsync(string country, string currency, string locale, string id, CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GetLocationByIDAsync");
            cacheKeyList.Add(country);
            cacheKeyList.Add(currency);
            cacheKeyList.Add(locale);
            cacheKeyList.Add(id);

            string cacheKey = string.Join(":", cacheKeyList);

            Place data = (Place)cache.Get(cacheKey);
            if (data == null && !cache.Contains(cacheKey))
            {
                var variables = new Dictionary<string, string>();
                variables.Add("{apiKey}", ApiKey());
                variables.Add("{market}", country);
                variables.Add("{currency}", currency);
                variables.Add("{locale}", locale);
                variables.Add("{id}", id);

                var URL = Settings.LocationAutoSuggestByIDURL.ReplaceFromDictionary(variables);
                var response = await WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken);
                var responseString = response.Item1.BytesToString();
                var returnData = JsonConvert.DeserializeObject<LocationServiceResponse>(responseString);
                data = returnData.Places.FirstOrDefault();

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.LocationAutoSuggestByIDCacheExpiration));
                cache.Add(cacheKey, data, policy);
            }

            return data;
        }

        public async Task<CurrenciesServiceResponse> GetCurrenciesAsync(CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GetCurrenciesAsync");

            string cacheKey = string.Join(":", cacheKeyList);

            CurrenciesServiceResponse data = (CurrenciesServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {
                await Settings.CurrenciesServiceLock.WaitAsync();
                try
                {
                    data = (CurrenciesServiceResponse)cache.Get(cacheKey);
                    if (data == null)
                    {
                        string responseString = "";

                        if (_dataPath != "" && File.Exists(_dataPath + Settings.CurrenciesServiceFileName))
                        {
                            var path = _dataPath + Settings.CurrenciesServiceFileName;
                            responseString = File.ReadAllText(path);
                        }
                        else
                        {
                            var variables = new Dictionary<string, string>();
                            variables.Add("{apiKey}", ApiKey());

                            var URL = Settings.CurrenciesServiceURL.ReplaceFromDictionary(variables);
                            var response = await WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken);
                            responseString = response.Item1.BytesToString();

                            try
                            {
                                if (_dataPath != "")
                                {
                                    File.WriteAllText(_dataPath + Settings.CurrenciesServiceFileName, responseString);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        data = JsonConvert.DeserializeObject<CurrenciesServiceResponse>(responseString);

                        cacheCurrencies(data, cancellationToken);
                    }
                }
                finally
                {
                    Settings.CurrenciesServiceLock.Release();
                }

            }

            return data;
        }

        private void cacheCurrencies(CurrenciesServiceResponse data, CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GetCurrenciesAsync");

            string cacheKey = string.Join(":", cacheKeyList);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.CurrenciesServiceCacheExpiration));
            cache.Add(cacheKey, data, policy);

            foreach (Currency currency in data.Currencies)
            {
                cache.Add("currency:" + currency.Code, currency, policy);

            }

        }

        public async Task<Currency> GetCurrencyByIDAsync(string id, CancellationToken cancellationToken)
        {
            await GetCurrenciesAsync(cancellationToken);
            Currency data = (Currency)cache.Get("currency:" + id);
            return data;
        }

        public async Task<LocalesServiceResponse> GetLocalesAsync(CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GetLocalesAsync");

            string cacheKey = string.Join(":", cacheKeyList);

            LocalesServiceResponse data = (LocalesServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {
                await Settings.LocalesServiceLock.WaitAsync();
                try
                {
                    data = (LocalesServiceResponse)cache.Get(cacheKey);
                    if (data == null)
                    {
                        string responseString = "";

                        if (_dataPath != "" && File.Exists(_dataPath + Settings.LocalesServiceFileName))
                        {
                            var path = _dataPath + Settings.LocalesServiceFileName;
                            responseString = File.ReadAllText(path);
                        }
                        else
                        {
                            var variables = new Dictionary<string, string>();
                            variables.Add("{apiKey}", ApiKey());

                            var URL = Settings.LocalesServiceURL.ReplaceFromDictionary(variables);
                            var response = await WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken);
                            responseString = response.Item1.BytesToString();

                            try
                            {
                                if (_dataPath != "")
                                {
                                    File.WriteAllText(_dataPath + Settings.LocalesServiceFileName, responseString);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        data = JsonConvert.DeserializeObject<LocalesServiceResponse>(responseString);
                        await cacheLocales(data, cancellationToken);            
                    }
                
                }
                finally
                {
                    Settings.LocalesServiceLock.Release();
                }

            }

            return data;
        }

        private async Task cacheLocales(LocalesServiceResponse data, CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GetLocalesAsync");

            string cacheKey = string.Join(":", cacheKeyList);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.LocalesServiceCacheExpiration));
            cache.Add(cacheKey, data, policy);

            //cache currencies
            await GetCurrenciesAsync(cancellationToken);

            foreach (Locale locale in data.Locales)
            {
                //cache countries
                await GetCountriesByLocaleAsync(locale.Code, cancellationToken);
                cache.Add("locale:" + locale.Code, locale, policy);

            }
        }

        public async Task<Locale> GetLocaleByIDAsync(string id, CancellationToken cancellationToken)
        {
            await GetLocalesAsync(cancellationToken);
            Locale data = (Locale)cache.Get("locale:" + id);
            return data;
        }

        public async Task<GeoServiceResponse> GetGeoLocationsAsync(CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GeoLocationsAsync");

            string cacheKey = string.Join(":", cacheKeyList);

            GeoServiceResponse data = (GeoServiceResponse)cache.Get(cacheKey);
            if (data == null)
            {
                await Settings.GeoServiceLock.WaitAsync();
                try
                {
                    data = (GeoServiceResponse)cache.Get(cacheKey);
                    if (data == null)
                    {

                        string responseString = "";

                        if (_dataPath != "" && File.Exists(_dataPath + Settings.GeoServiceFileName))
                        {
                            var path = _dataPath + Settings.GeoServiceFileName;
                            responseString = File.ReadAllText(path);
                        }
                        else
                        {
                            var variables = new Dictionary<string, string>();
                            variables.Add("{apiKey}", ApiKey());

                            var URL = Settings.GeoServiceURL.ReplaceFromDictionary(variables);
                            var response = await WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken);
                            responseString = response.Item1.BytesToString();

                            try
                            {
                                if (_dataPath != "")
                                {
                                    File.WriteAllText(_dataPath + Settings.GeoServiceFileName, responseString);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        data = JsonConvert.DeserializeObject<GeoServiceResponse>(responseString);

                        CacheGeoLocations(data, cancellationToken);
                    }
                }
                finally
                {
                    Settings.GeoServiceLock.Release();
                }

            }

            return data;
        }

        private void CacheGeoLocations(GeoServiceResponse data, CancellationToken cancellationToken)
        {

            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GeoLocationsAsync");

            string cacheKey = string.Join(":", cacheKeyList);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.GeoServiceCacheExpiration));
            cache.Add(cacheKey, data, policy);

            foreach (Continent continent in data.Continents)
            {
                cache.Add("continent:" + continent.Id, continent, policy);
                foreach (Country country in continent.Countries)
                {
                    cache.Add("country:" + country.Id, country, policy);
                    foreach (City city in country.Cities)
                    {
                        cache.Add("city:" + city.IataCode, city, policy);
                        foreach (Airport airport in city.Airports)
                        {
                            cache.Add("airport:" + airport.Id, airport, policy);

                        }
                    }
                }
            }
        }

        public async Task<Airport> GetAirportByIDAsync(string id, CancellationToken cancellationToken)
        {
            await GetGeoLocationsAsync(cancellationToken);
            Airport data = (Airport)cache.Get("airport:" + id);
            return data;
        }

        public async Task<City> GetCityByIDAsync(string id, CancellationToken cancellationToken)
        {
            await GetGeoLocationsAsync(cancellationToken);
            City data = (City)cache.Get("city:" + id);
            return data;

        }
        public async Task<Country> GetCountryByIDAsync(string id, CancellationToken cancellationToken)
        {
            await GetGeoLocationsAsync(cancellationToken);
            Country data = (Country)cache.Get("country:" + id);
            return data;
        }

        public async Task<Region> GetRegionByIDAsync(string id, CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<Continent> GetContinentByIDAsync(string id, CancellationToken cancellationToken)
        {
            await GetGeoLocationsAsync(cancellationToken);
            Continent data = (Continent)cache.Get("continent:" + id);
            return data;
        }

        //Markets!
        public async Task<CountriesServiceResponse> GetCountriesByLocaleAsync(string locale, CancellationToken cancellationToken)
        {
            List<object> cacheKeyList = new List<object>();
            cacheKeyList.Add("GetCountriesByLocaleAsync");
            cacheKeyList.Add(locale);

            string cacheKey = string.Join(":", cacheKeyList);

            CountriesServiceResponse data = (CountriesServiceResponse)cache.Get(cacheKey);
            if (data == null && !cache.Contains(cacheKey))
            {
                if (!Settings.CountriesServiceLock.ContainsKey(locale))
                {
                    Settings.CountriesServiceLock.TryAdd(locale, new SemaphoreSlim(1, 1));
                }

                await Settings.CountriesServiceLock[locale].WaitAsync();
                try
                {
                    data = (CountriesServiceResponse)cache.Get(cacheKey);
                    if (data == null && !cache.Contains(cacheKey))
                    {

                        string responseString = "";

                        var variablesPath = new Dictionary<string, string>();
                        variablesPath.Add("{locale}", locale);

                        if (_dataPath != "" && File.Exists(_dataPath + Settings.CountriesServiceFileName.ReplaceFromDictionary(variablesPath)))
                        {
                            var path = _dataPath + Settings.CountriesServiceFileName.ReplaceFromDictionary(variablesPath);
                            responseString = File.ReadAllText(path);
                        }
                        else
                        {

                            var variables = new Dictionary<string, string>();
                            variables.Add("{apiKey}", ApiKey());
                            variables.Add("{locale}", locale);

                            var URL = Settings.CountriesServiceURL.ReplaceFromDictionary(variables);
                            var response = await WebRequestHelper.GetAsync(URL, WebRequestHelper.ResponseAcceptType.JSON, cancellationToken);
                            responseString = response.Item1.BytesToString();

                            try
                            {
                                if (_dataPath != "")
                                {
                                    File.WriteAllText(_dataPath+Settings.CountriesServiceFileName.ReplaceFromDictionary(variablesPath), responseString);
                                }
                            }
                            catch(Exception ex)
                            {
                                
                            }
                            
                        }

                        data = JsonConvert.DeserializeObject<CountriesServiceResponse>(responseString);

                        for (int i = 0; i < data.Countries.Count(); i++)
                        {
                            var country = data.Countries[i];
                            var geoCountry = await GetCountryByIDAsync(country.Code, cancellationToken);
                            if (geoCountry != null)
                            {
                                country.CurrencyId = geoCountry.CurrencyId;
                            }
                            else
                            {
                                Boolean noCurrency = true;
                            }
                        }

                        CacheItemPolicy policy = new CacheItemPolicy();
                        policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(Settings.CountriesServiceCacheExpiration));
                        cache.Add(cacheKey, data, policy);
                    }

                }
                finally
                {
                    Settings.CountriesServiceLock[locale].Release();
                }


            }

            return data;
        }





    }
}
