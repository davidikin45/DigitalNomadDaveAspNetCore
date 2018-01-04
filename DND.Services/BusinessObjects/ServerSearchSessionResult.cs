using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DND.Services.Skyscanner.Model;

namespace DND.Services.FlightSearch.BusinessObjects
{
    public class ServerSearchSessionResult
    {
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Locale { get; set; }
        public string OriginPlaceSkyscannerCode { get; set; }
        public string DestinationPlaceSkyscannerCode { get; set; }
        public DateTime OutboundPartialDate { get; set; }
        public DateTime? InboundPartialDate { get; set; } 
        public double? PriceFilter { get; set; }
        public String CabinClass { get; set; }

        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public int? MaxStopsFilter { get; set; }

        public string PollURL { get; set; }
        public LivePricesServiceResponse Results { get; set; }

        public ServerSearchSessionResult(string country, string currency, string locale, string originPlaceSkyscannerCode,
        string destinationPlaceSkyscannerCode, DateTime outboundPartialDate, DateTime? inboundPartialDate, int adults, int children, int infants,
        double? priceFilter, int? maxStopsFilter, string cabinClass)
        {
            Country = country;
            Currency = currency;
            Locale = locale;
            OriginPlaceSkyscannerCode = originPlaceSkyscannerCode;
            DestinationPlaceSkyscannerCode = destinationPlaceSkyscannerCode;
            OutboundPartialDate = outboundPartialDate;
            InboundPartialDate = inboundPartialDate;
            Adults = adults;
            Children = children;
            Infants = infants;
            MaxStopsFilter = maxStopsFilter;
            PriceFilter = priceFilter;
            CabinClass = cabinClass;
        }  
     
        //public string PollKey{
        //    get { 

        //            string outboundPartialDateString = OutboundPartialDate.ToString("yyyy-MM-dd");      
        //            string inboundPartialDateString = "";
        //            if (InboundPartialDate.HasValue)
        //            {
        //              inboundPartialDateString = InboundPartialDate.Value.ToString("yyyy-MM-dd");
        //            }

        //            List<object> keySearchFilter = new List<object>();
        //            keySearchFilter.Add(Country);
        //            keySearchFilter.Add(Currency);
        //            keySearchFilter.Add(Locale);
        //            keySearchFilter.Add(OriginPlaceSkyscannerCode);
        //            keySearchFilter.Add(DestinationPlaceSkyscannerCode);
        //            keySearchFilter.Add(outboundPartialDateString);
        //            keySearchFilter.Add(inboundPartialDateString);
        //            keySearchFilter.Add(Adults); //Adults
        //            keySearchFilter.Add(Children); //Children
        //            keySearchFilter.Add(Infants); //Infants
        //            keySearchFilter.Add(MaxStopsFilter); //Stops
        //            keySearchFilter.Add(CabinClass);

        //            string keySearchFilterString = string.Join(":", keySearchFilter);    

        //            return keySearchFilterString;
        //    }
        //}
           
    }
}
