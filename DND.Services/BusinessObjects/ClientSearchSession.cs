using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DND.Services.Skyscanner.Model;
using System.Threading;
using Solution.Base.ModelMetadataCustom;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;

namespace DND.Services.FlightSearch.BusinessObjects
{
    public class ClientSearchSession
    {
        [JsonIgnoreAttribute]
        public List<Tuple<string, string, string>> PollOriginDestinationURLs { get; set; }
        [JsonIgnoreAttribute]
        public List<LivePricesServiceResponse> PollLivePriceSearchResults { get; set; }
        [JsonIgnoreAttribute]
        public string PollCacheKey { get; private set; }
        [JsonIgnoreAttribute]
        public string FilterKey { get; private set; }

        public string ClientSearchID { get; private set; }

        public int ResultCount { get; private set; }
        public string SortType { get; private set; }
        public string SortOrder { get; private set; }

        public string[] Country { get; private set; }
        public string Currency { get; private set; }
        public string Locale { get; private set; }
        public string OriginPlaceSkyscannerCode { get; private set; }
        public string DestinationPlaceSkyscannerCode { get; private set; }
        public DateTime OutboundPartialDate { get; private set; }
        public DateTime? InboundPartialDate { get; private set; }
        public double? PriceMinFilter { get; private set; }
        public double? PriceMaxFilter { get; private set; }
        public String CabinClass { get; private set; }

        public int Adults { get; private set; }
        public int Children { get; private set; }
        public int Infants { get; private set; }
        public int? MaxStopsFilter { get; private set; }

        public DateTime? OutboundDepartureDateTimeFromFilter { get; private set; }
        public DateTime? OutboundDepartureDateTimeToFilter { get; private set; }
        public DateTime? OutboundArrivalDateTimeFromFilter { get; private set; }
        public DateTime? OutboundArrivalDateTimeToFilter { get; private set; }
        public DateTime? InboundDepartureDateTimeFromFilter { get; private set; }
        public DateTime? InboundDepartureDateTimeToFilter { get; private set; }
        public DateTime? InboundArrivalDateTimeFromFilter { get; private set; }
        public DateTime? InboundArrivalDateTimeToFilter { get; private set; }

        public int TotalResults { get; private set; }
        public List<Itinerary> FilteredItineraries { get; set; }

        public ClientSearchSession(string[] country,
           string currency, string locale, string originPlaceSkyscannerCode,
       string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, string cabinClass)
        {
            ClientSearchID = Guid.NewGuid().ToString();
            Country = country;
            Currency = currency;
            Locale = locale;
            OriginPlaceSkyscannerCode = originPlaceSkyscannerCode;
            DestinationPlaceSkyscannerCode = destinationPlaceSkyscannerCode;
            OutboundPartialDate = outboundPartialDate;
            InboundPartialDate = inboundPartialDate;
            CabinClass = cabinClass;
            FilteredItineraries = new List<Itinerary>();
            PollOriginDestinationURLs = new List<Tuple<string, string, string>>();
        }

        public async Task FilterAsync(string sortType, string sortOrder, int adults, int children, int infants, DateTime? outboundDepartureDateTimeFromFilter, DateTime? outboundDepartureDateTimeToFilter,
           DateTime? outboundArrivalDateTimeFromFilter, DateTime? outboundArrivalDateTimeToFilter, DateTime? inboundDepartureDateTimeFromFilter, DateTime? inboundDepartureDateTimeToFilter,
           DateTime? inboundArrivalDateTimeFromFilter, DateTime? inboundArrivalDateTimeToFilter, double? priceMinFilterPerAdultPassenger, double? priceMaxFilterPerAdultPassenger, int? stopsFilter, CancellationToken cancellationToken)
        {


            SortType = sortType;
            SortOrder = sortOrder;

            Adults = adults;
            Children = children;
            Infants = infants;
            OutboundDepartureDateTimeFromFilter = outboundDepartureDateTimeFromFilter;
            OutboundDepartureDateTimeToFilter = outboundDepartureDateTimeToFilter;
            OutboundArrivalDateTimeFromFilter = outboundArrivalDateTimeFromFilter;
            OutboundArrivalDateTimeToFilter = outboundArrivalDateTimeToFilter;
            InboundDepartureDateTimeFromFilter = inboundDepartureDateTimeFromFilter;
            InboundDepartureDateTimeToFilter = inboundDepartureDateTimeToFilter;
            InboundArrivalDateTimeFromFilter = inboundArrivalDateTimeFromFilter;
            InboundArrivalDateTimeToFilter = inboundArrivalDateTimeToFilter;
            PriceMinFilter = priceMinFilterPerAdultPassenger;
            PriceMaxFilter = priceMaxFilterPerAdultPassenger;
            MaxStopsFilter = stopsFilter;

            TotalResults = 0;
            var tasks = new List<Task<LivePricesServiceResponse>>();

            foreach (LivePricesServiceResponse r in PollLivePriceSearchResults)
            {
                TotalResults = TotalResults + r.Itineraries.Count;
                tasks.Add(r.FilterAsync(Currency, priceMinFilterPerAdultPassenger, priceMaxFilterPerAdultPassenger, stopsFilter, outboundDepartureDateTimeFromFilter, outboundDepartureDateTimeToFilter, outboundArrivalDateTimeFromFilter, outboundArrivalDateTimeToFilter,
                   stopsFilter, inboundDepartureDateTimeFromFilter, inboundDepartureDateTimeToFilter, inboundArrivalDateTimeFromFilter, inboundArrivalDateTimeToFilter, cancellationToken)
                );
            }

            IEnumerable<LivePricesServiceResponse> tempResults = await Task.WhenAll(tasks.ToArray());

            var filteredResults = new List<LivePricesServiceResponse>();
            foreach (LivePricesServiceResponse livePriceResult in tempResults)
            {
                if (livePriceResult.Itineraries.Count > 0)
                {
                    filteredResults.Add(livePriceResult);
                }
            }

            ResultCount = filteredResults.Sum(r => r.Itineraries.Count);

            foreach (LivePricesServiceResponse result in filteredResults)
            {
                foreach (Itinerary i in result.Itineraries)
                {
                    FilteredItineraries.Add(i);
                }
            }

            int ascDesc = 1; //asc

            if (SortOrder != null && SortOrder.ToLower() == OrderByType.Descending)
            {
                ascDesc = -1; //desc
            }

            switch (SortType.ToLower())
            {
                case "price":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.BestPrice.CompareTo(b.BestPrice));
                    break;
                case "outbounddeparturetime":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.OutboundDepartureDateTime.CompareTo(b.OutboundDepartureDateTime));
                    break;
                case "outboundarrivaltime":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.OutboundArrivalDateTime.CompareTo(b.OutboundArrivalDateTime));
                    break;
                case "outboundduration":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.OutboundDuration.CompareTo(b.OutboundDuration));
                    break;
                case "outboundstops":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.OutboundStopCount.CompareTo(b.OutboundStopCount));
                    break;
                case "inbounddeparturetime":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.InboundDepartureDateTime.CompareTo(b.InboundDepartureDateTime));
                    break;
                case "inboundarrivaltime":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.InboundArrivalDateTime.CompareTo(b.InboundArrivalDateTime));
                    break;
                case "inboundduration":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.InboundDuration.CompareTo(b.InboundDuration));
                    break;
                case "inboundstops":
                    FilteredItineraries.Sort((a, b) => ascDesc * a.InboundStopCount.CompareTo(b.InboundStopCount));
                    break;
                default:
                    FilteredItineraries.Sort((a, b) => ascDesc * a.BestPrice.CompareTo(b.BestPrice));
                    break;

            }


        }
    }
}
