using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public partial class Leg
    {


        public String OriginFormatted { get; set; }


        public Place OriginPlace { get; set; }


        public String OriginAirportCode { get; set; }



        public double OriginAirportLatitude { get; set; }



        public double OriginAirportLongitude { get; set; }


        public String OriginAirport { get; set; }


        public String OriginCity { get; set; }


        public String OriginCountry { get; set; }



        public DateTime DepartureDateTimeConverted { get; set; }


        public String DepartureDateFormattedLong { get; set; }


        public String DepartureDateFormattedShort { get; set; }


        public String DepartureTimeFormatted { get; set; }


        public String DestinationFormatted { get; set; }


        public Place DestinationPlace { get; set; }


        public String DestinationAirportCode { get; set; }


        public double DestinationAirportLatitude { get; set; }


        public double DestinationAirportLongitude { get; set; }


        public String DestinationAirport { get; set; }


        public String DestinationCity { get; set; }


        public String DestinationCountry { get; set; }


        public DateTime ArrivalDateTimeConverted { get; set; }


        public String ArrivalDateFormattedLong { get; set; }


        public String ArrivalDateFormattedShort { get; set; }


        public String ArrivalTimeFormatted { get; set; }


        public string DurationHourMinuteFormatted { get; set; }



        public int StopCount { get; set; }


        public List<Carrier> CarrierObjects { get; set; }


        public List<Segment> Segments { get; set; }
    }
}
