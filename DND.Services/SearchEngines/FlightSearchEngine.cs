using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using DND.Services.FlightSearch.BusinessObjects;
using DND.Services.Skyscanner.Model;
using Solution.Base.Helpers;
using DND.Services.SearchEngines;
using DND.Services.Factories;
using Solution.Base.Infrastructure;
using DND.Services.SearchEngines.Interfaces;
using Solution.Base.Implementation.Validation;

namespace DND.Services.SearchEngines
{
    public class FlightSearchEngine
    {
        public class Options
        {
            public int AnywhereCountryLimit = 10;

        }

        private IFlightSearchEngine _flightSearchEngine;
        private Options _options; 

        public FlightSearchEngine(string id)
        {
            _flightSearchEngine = new FlightSearchEngineFactory().GetFlightSearchEngine(id);
            _options = new Options();
        }

        #region Search
        public async Task<ClientSearchSession> SearchAsync(string apiKey, string[] countries, string currency, string locale, string originPlaceSkyscannerCode,
         string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants,
          double? priceMinFilter, double? priceMaxFilter, int? maxStopsFilter, string cabinClass, string sortType, string sortOrder,
         DateTime? outboundDepartureDateTimeFromFilter, DateTime? outboundDepartureDateTimeToFilter, DateTime? outboundArrivalDateTimeFromFilter, DateTime? outboundArrivalDateTimeToFilter,
         DateTime? inboundDepartureDateTimeFromFilter, DateTime? inboundDepartureDateTimeToFilter, DateTime? inboundArrivalDateTimeFromFilter, DateTime? inboundArrivalDateTimeToFilter, CancellationToken cancellationToken)
        {
            await _flightSearchEngine.GetLocalesAsync(cancellationToken);
            int passengers = adults + children + infants;

            List<ServerSearchSessionResult> searchResults = new List<ServerSearchSessionResult>();

            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken);
            var tasks = new List<Task<IEnumerable<ServerSearchSessionResult>>>();

            foreach (string country in countries)
            {
                tasks.Add(StartSearchAsync(country, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, adults, 0, 0, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, cts.Token));
            }

            IEnumerable<IEnumerable<ServerSearchSessionResult>> tempResults = await TaskHelper.WhenAllOrException(tasks.ToArray(), cts);

            foreach (List<ServerSearchSessionResult> list in tempResults)
            {
                foreach (ServerSearchSessionResult result in list)
                {
                    searchResults.Add(result);
                }
            }

            double? priceMinFilterAllPassengers = null;
            if (priceMinFilter.HasValue)
            {
                priceMinFilterAllPassengers = priceMinFilter * passengers;
            }

            double? priceMaxFilterAllPassengers = null;
            if (priceMaxFilter.HasValue)
            {
                priceMaxFilterAllPassengers = priceMaxFilter * passengers;
            }

            ServerSearchSession data = ServerSearchSession.CreateServerSearchSession(countries, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, adults, 0, 0, priceMinFilterAllPassengers, priceMaxFilterAllPassengers, 3, cabinClass, searchResults);

            ClientSearchSession clientSearchSession = data.CreateClientSearchSession();
            await clientSearchSession.FilterAsync(sortType, sortOrder, adults, children, infants, outboundDepartureDateTimeFromFilter, outboundDepartureDateTimeToFilter, outboundArrivalDateTimeFromFilter, outboundArrivalDateTimeToFilter,
                   inboundDepartureDateTimeFromFilter, inboundDepartureDateTimeToFilter, inboundArrivalDateTimeFromFilter, inboundArrivalDateTimeToFilter, priceMinFilterAllPassengers, priceMaxFilterAllPassengers, maxStopsFilter, cancellationToken);

            return clientSearchSession;
        }

        private async Task<IEnumerable<ServerSearchSessionResult>> StartSearchAsync(string country, string currency, string locale, string originPlaceSkyscannerCode,
         string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants,
         double? priceMinFilter, double? priceMaxFilter, int? maxStopsFilter, string cabinClass, CancellationToken cancellationToken)
        {
            //ConcurrentBag<ServerSearchSessionResult> searchResults2 = new ConcurrentBag<ServerSearchSessionResult>();

            IEnumerable<ServerSearchSessionResult> searchResults = await StartSearchInnerAsync(country, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter,
             maxStopsFilter, cabinClass, false, cancellationToken);

            return searchResults;
        }

        private async Task<List<ServerSearchSessionResult>> StartSearchInnerAsync(string country, string currency, string locale, string originPlaceSkyscannerCode,
         string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants,
          double? priceMinFilter, double? priceMaxFilter, int? maxStopsFilter, string cabinClass, Boolean livePriceDirect, CancellationToken cancellationToken)
        {
            //List<LivePrices.RootObject> results = new List<LivePrices.RootObject>();
            //ConcurrentBag<string> pollURLs = new ConcurrentBag<string>();

            Boolean direct = false;
            if (maxStopsFilter.HasValue && maxStopsFilter.Value == 0)
            {
                //direct = true;
            }

            if (originPlaceSkyscannerCode != "anywhere" && !originPlaceSkyscannerCode.EndsWith("-sky") && originPlaceSkyscannerCode != "nearest" && !originPlaceSkyscannerCode.Contains("-ip"))
            {
                originPlaceSkyscannerCode = originPlaceSkyscannerCode + "-sky";
            }

            if (destinationPlaceSkyscannerCode != "anywhere" && !destinationPlaceSkyscannerCode.EndsWith("-sky") && destinationPlaceSkyscannerCode != "nearest" && !destinationPlaceSkyscannerCode.Contains("-ip"))
            {
                destinationPlaceSkyscannerCode = destinationPlaceSkyscannerCode + "-sky";
            }

            if (originPlaceSkyscannerCode == "nearest")
            {
                originPlaceSkyscannerCode = RequestHelper.GetIPAddress() + "-ip";
            }

            if (destinationPlaceSkyscannerCode == "nearest")
            {
                destinationPlaceSkyscannerCode = RequestHelper.GetIPAddress() + "-ip";
            }


            ConcurrentBag<ServerSearchSessionResult> searchResults = new ConcurrentBag<ServerSearchSessionResult>();
            List<Task<List<ServerSearchSessionResult>>> tasks = new List<Task<List<ServerSearchSessionResult>>>();
            //routes - All flights at any time of week - Origin/Destination/Carrier. Route Origin is always what was entered.
            //quotes - All available flights

            //Airport > Country = Airports
            //City > Country = Airports
            //Country > Country = Airports
            //Airport > Anywhere = Countries
            //City > Anywhere = Countries
            //Country > Anywhere = Countries
            if (destinationPlaceSkyscannerCode == "anywhere")
            {
                var dataOriginal = await _flightSearchEngine.BrowseRoutesSearchAsync(country, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, cancellationToken);

                var dataFilter = dataOriginal.Filter(priceMinFilter, priceMaxFilter, direct);

                var countryRoutes = dataFilter.Routes;
                var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken);
                var t = new List<Task<List<ServerSearchSessionResult>>>();

                // Get Route Destinations - From Airport
                //Expand

                //if (_options.AnywhereCountryLimit > 0 && countryRoutes.Count > _options.AnywhereCountryLimit)
                //{
                //    throw new ServiceValidationErrors(new GeneralError("This search is going to take too long. Restrict price range."));
                //}
               
                foreach (Route route in countryRoutes)
                {
                    var destinationCountry = dataOriginal.Places.Where(r => r.PlaceId == route.DestinationId.ToString()).First();
                    if (destinationCountry.SkyscannerCode != "CE")
                    {
                        t.Add(StartSearchInnerAsync(country, currency, locale, originPlaceSkyscannerCode, destinationCountry.SkyscannerCode + "-sky", outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, false, cts.Token));
                    }
                }

                var results = await TaskHelper.WhenAllOrException(t.ToArray(), cts);
                foreach (List<ServerSearchSessionResult> result in results)
                {
                    searchResults.AddRange(result);
                }

            }
            else if (originPlaceSkyscannerCode == "anywhere")
            {

                var dataOriginal = await _flightSearchEngine.BrowseRoutesSearchAsync(country, currency, locale, destinationPlaceSkyscannerCode, originPlaceSkyscannerCode, null, null, cancellationToken);
                //This is a filter based on the reverse route. This will give performance
                dataOriginal = dataOriginal.Filter(priceMinFilter, priceMaxFilter, direct);

                var countryRoutes = dataOriginal.Routes;

                var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken);
                var t = new List<Task<List<ServerSearchSessionResult>>>();

                // Get Route Destinations
                //Expand

                foreach (Route countryRoute in countryRoutes)
                {
                    //For anywhere the destination will be Country 
                    var destinationCountry = dataOriginal.Places.Where(r => r.PlaceId == countryRoute.DestinationId.ToString()).First();
                    if (destinationCountry.SkyscannerCode != "CE")
                    {
                        t.Add(StartSearchInnerAsync(country, currency, locale, destinationCountry.SkyscannerCode + "-sky", destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, false, cts.Token));
                    }

                }

                var results = await TaskHelper.WhenAllOrException(t.ToArray(), cts);
                foreach (List<ServerSearchSessionResult> result in results)
                {
                    searchResults.AddRange(result);
                }
            }
            else if (!livePriceDirect)
            {
                BrowseRoutesServiceResponse dataOriginal = await _flightSearchEngine.BrowseRoutesSearchAsync(country, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, cancellationToken);
                BrowseRoutesServiceResponse dataFilter = dataOriginal.Filter(priceMinFilter, priceMaxFilter, direct);


                //To or From Country
                if (dataOriginal.Routes.Count() > 0)
                {
                    var cityRoutes = dataFilter.Routes;
                    if (cityRoutes.Count > 0)
                    {

                        var origin = dataFilter.Places.Where(r => r.PlaceId == cityRoutes[0].OriginId.ToString()).First();
                        var destination = dataFilter.Places.Where(r => r.PlaceId == cityRoutes[0].DestinationId.ToString()).First();

                        if (((origin.Type == "Country") && (destination.Type == "Station")) || ((origin.Type == "City") && (destination.Type == "Station")) || ((origin.Type == "Station") && (destination.Type == "City")) ||
                            ((origin.Type == "Station") && (destination.Type == "Station") && (originPlaceSkyscannerCode != origin.SkyscannerCode + "-sky") && !originPlaceSkyscannerCode.Contains("-ip") && (destinationPlaceSkyscannerCode != destination.SkyscannerCode + "-sky") && !destinationPlaceSkyscannerCode.Contains("-ip")))
                        {

                            //var originCountry = origin;
                            //var data = await _flightSearchEngine.BrowseRoutesSearchAsync(country, currency, locale, originCountry.SkyscannerCode, destination.SkyscannerCode + "-sky", outboundPartialDate, inboundPartialDate);
                            //var dataFilterCities = data.Filter(priceMinFilter, priceMaxFilter);
                            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken);
                            var t = new List<Task<List<ServerSearchSessionResult>>>();

                            if (dataFilter.OriginCities().Count > 0)
                            {
                                foreach (string originCityLoop in dataFilter.OriginCities())
                                {
                                    var originCity = originCityLoop + "-sky";
                                    if (dataFilter.DestinationCitiesByOriginCity(originCityLoop).Count > 0)
                                    {
                                        foreach (string destinationCityLoop in dataFilter.DestinationCitiesByOriginCity(originCityLoop))
                                        {
                                            var destiantionCity = destinationCityLoop + "-sky";
                                            t.Add(StartSearchInnerAsync(country, currency, locale, originCity, destiantionCity, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, true, cts.Token));
                                        }
                                    }
                                }
                            }

                            var results = await TaskHelper.WhenAllOrException(t.ToArray(), cts);
                            foreach (List<ServerSearchSessionResult> result in results)
                            {
                                searchResults.AddRange(result);
                            }
                        }
                        else if (originPlaceSkyscannerCode == origin.SkyscannerCode + "-sky" || originPlaceSkyscannerCode.Contains("-ip"))
                        {
                            var originAirport = origin.SkyscannerCode + "-sky";
                            //searchResults.AddRange(await StartSearchInnerAsync(country, currency, locale, originAirport, destinationCityTemp.SkyscannerCode, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, true));
                            //var data = await _flightSearchEngine.BrowseRoutesSearchAsync(country, currency, locale, originAirport, destination.SkyscannerCode, outboundPartialDate, inboundPartialDate);
                            //var dataFilterCities = data.Filter(priceMinFilter, priceMaxFilter);
                            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken);
                            var t = new List<Task<List<ServerSearchSessionResult>>>();

                            if (dataFilter.DestinationCitiesByOriginAirport(origin.SkyscannerCode).Count > 0)
                            {
                                foreach (string destinationCityLoop in dataFilter.DestinationCitiesByOriginAirport(origin.SkyscannerCode))
                                {
                                    var destinationCity = destinationCityLoop + "-sky";
                                    t.Add(StartSearchInnerAsync(country, currency, locale, originAirport, destinationCity, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, true, cts.Token));
                                }
                            }

                            var results = await TaskHelper.WhenAllOrException(t.ToArray(), cts);
                            foreach (List<ServerSearchSessionResult> result in results)
                            {
                                searchResults.AddRange(result);
                            }

                        }
                        else if (destinationPlaceSkyscannerCode == destination.SkyscannerCode + "-sky" || destinationPlaceSkyscannerCode.Contains("-ip"))
                        {
                            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken);
                            var t = new List<Task<List<ServerSearchSessionResult>>>();


                            if (dataFilter.OriginCitiesByDestinationAirport(destination.SkyscannerCode).Count > 0)
                            {

                                var destinationAirport = destination.SkyscannerCode + "-sky";

                                foreach (string originCityLoop in dataFilter.OriginCitiesByDestinationAirport(destination.SkyscannerCode))
                                {
                                    var originCity = originCityLoop + "-sky";
                                    t.Add(StartSearchInnerAsync(country, currency, locale, originCity, destinationAirport, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, true, cts.Token));
                                }
                            }

                            var results = await TaskHelper.WhenAllOrException(t.ToArray(), cts);
                            foreach (List<ServerSearchSessionResult> result in results)
                            {
                                searchResults.AddRange(result);
                            }
                        }
                    }
                }
                else if (dataOriginal.Places.Count() > 0)
                {
                    var originAirportOrCity = originPlaceSkyscannerCode;
                    var destinationAirportOrCity = destinationPlaceSkyscannerCode;

                    searchResults.AddRange(await StartSearchInnerAsync(country, currency, locale, originAirportOrCity, destinationAirportOrCity, outboundPartialDate, inboundPartialDate, adults, children, infants, priceMinFilter, priceMaxFilter, maxStopsFilter, cabinClass, true, cancellationToken));
                }
            }
            else if (livePriceDirect)
            {
                var originAirportOrCity = originPlaceSkyscannerCode;
                var destinationAirportOrCity = destinationPlaceSkyscannerCode;

                BrowseRoutesServiceResponse cacheResults = await _flightSearchEngine.BrowseRoutesSearchAsync(country, currency, locale, originAirportOrCity, destinationAirportOrCity, outboundPartialDate, inboundPartialDate, cancellationToken);
                cacheResults = cacheResults.Filter(priceMinFilter, priceMaxFilter, direct);

               
                    if (cacheResults.Quotes.Count > 0 || cacheResults.Routes.Count > 0)
                    {
                        var result = new ServerSearchSessionResult(country, currency, locale, originAirportOrCity, destinationAirportOrCity, outboundPartialDate, inboundPartialDate, adults, children, infants,
                   priceMaxFilter, maxStopsFilter, cabinClass);

                        //**** This is where the magic happens!
                        result.Results = await _flightSearchEngine.LivePriceSearchAsync(country, currency, locale, originAirportOrCity, destinationAirportOrCity, outboundPartialDate, inboundPartialDate, adults, children, infants, cabinClass, null, cancellationToken);


                        searchResults.Add(result);
                    }

                

            }

            return searchResults.ToList();

        }
        #endregion

    }
}