using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using DND.Services.Skyscanner.Model;

namespace DND.Services.FlightSearch.BusinessObjects
{
    public class ServerSearchSession
    {
        public string SearchID { get; private set; }

        public string[] Countries { get; private set; }
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

        public List<ServerSearchSessionResult> ResultDetails { get; private set; }

        public static ServerSearchSession CreateServerSearchSession(string[] countries, string currency, string locale, string originPlaceSkyscannerCode,
        string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants,
        double? priceMinFilter, double? priceMaxFilter, int? maxStopsFilter, string cabinClass, List<ServerSearchSessionResult> resultDetails)
        {
            return new ServerSearchSession(countries, currency, locale, originPlaceSkyscannerCode, destinationPlaceSkyscannerCode, outboundPartialDate, inboundPartialDate, 1, 0, 0, priceMinFilter, priceMaxFilter,
                       maxStopsFilter, cabinClass, resultDetails);
        }


        private ServerSearchSession(string[] countries, string currency, string locale, string originPlaceSkyscannerCode,
        string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants,
        double? priceMinFilter, double? priceMaxFilter, int? maxStopsFilter, string cabinClass, List<ServerSearchSessionResult> resultDetails)
        {
            SearchID = Guid.NewGuid().ToString();
            Countries = countries;
            Currency = currency;
            Locale = locale;
            OriginPlaceSkyscannerCode = originPlaceSkyscannerCode;
            DestinationPlaceSkyscannerCode = destinationPlaceSkyscannerCode;
            OutboundPartialDate = outboundPartialDate;
            InboundPartialDate = inboundPartialDate;

            Adults = adults;
            Children = children;
            Infants = infants;
            CabinClass = cabinClass;

            MaxStopsFilter = maxStopsFilter;
            PriceMinFilter = priceMinFilter;
            PriceMaxFilter = priceMaxFilter;

            ResultDetails = resultDetails;
        }

        public ClientSearchSession CreateClientSearchSession()
        {
            ClientSearchSession clientSearchSession = new ClientSearchSession(Countries, Currency, Locale, OriginPlaceSkyscannerCode, DestinationPlaceSkyscannerCode, OutboundPartialDate, InboundPartialDate, CabinClass);

            List<LivePricesServiceResponse> results = new List<LivePricesServiceResponse>();

            foreach (ServerSearchSessionResult result in ResultDetails)
            {
                results.Add(result.Results);
            }

            clientSearchSession.PollLivePriceSearchResults = results;

            //foreach (ServerSearchSessionResult r in ResultDetails)
            //{
            //    Tuple<string, string, string> result = new Tuple<string, string, string>(r.OriginPlaceSkyscannerCode, r.DestinationPlaceSkyscannerCode, r.PollURL);
            //    clientSearchSession.PollOriginDestinationURLs.Add(result);              
            //}

            return clientSearchSession;
        }

        //public async Task StartLivePriceSearch()
        //{
        //    var service = new FlightSearchEngine("skyscanner");
        //    await service.StartLivePriceSearchAsync(ResultDetails, false);
        //}
    }
}
