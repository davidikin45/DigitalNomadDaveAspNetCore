using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public partial class Itinerary
    {

        public Agent BestPriceAgent { get; set; }

        public Double BestPrice { get; set; }

        public int BestPriceRounded { get; set; }

        public string BestPriceFormatted { get; set; }

        public string BestPriceRoundedFormatted { get; set; }

        public String Agent { get; set; }

        public String AgentImageUrl { get; set; }

        public string DeeplinkUrl { get; set; }


        public Boolean ReturnFlight { get; set; }


        public Leg OutboundLeg { get; set; }


        public Leg InboundLeg { get; set; }



        public String OutboundOriginFormatted { get; set; }


        public Place OutboundOriginPlace { get; set; }

        public String OutboundOriginAirportCode { get; set; }


        public double OutboundOriginAirportLatitude { get; set; }


        public double OutboundOriginAirportLongitude { get; set; }


        public String OutboundOriginAirport { get; set; }


        public String OutboundOriginCity { get; set; }

        public double OutboundOriginCityLatitude { get; set; }

        public double OutboundOriginCityLongitude { get; set; }


        public String OutboundOriginCountry { get; set; }


        public Place OutboundOriginAirportPlace { get; set; }


        public Place OutboundOriginCityPlace { get; set; }


        public Place OutboundOriginCountryPlace { get; set; }



        public String OutboundDestinationFormatted { get; set; }


        public Place OutboundDestinationPlace { get; set; }


        public String OutboundDestinationAirportCode { get; set; }

        public double OutboundDestinationAirportLatitude { get; set; }

        public double OutboundDestinationAirportLongitude { get; set; }


        public String OutboundDestinationAirport { get; set; }


        public String OutboundDestinationCity { get; set; }

        public double OutboundDestinationCityLatitude { get; set; }

        public double OutboundDestinationCityLongitude { get; set; }

        public String OutboundDestinationCountry { get; set; }


        public Place OutboundDestinationAirportPlace { get; set; }


        public Place OutboundDestinationCityPlace { get; set; }



        public Place OutboundDestinationCountryPlace { get; set; }




        public DateTime OutboundDepartureDateTime { get; set; }


        public String OutboundDepartureDateFormattedLong { get; set; }


        public String OutboundDepartureDateFormattedShort { get; set; }


        public String OutboundDepartureTimeFormatted { get; set; }


        public DateTime OutboundArrivalDateTime { get; set; }

        public String OutboundArrivalDateFormatted { get; set; }


        public String OutboundArrivalDateFormattedLong { get; set; }


        public String OutboundArrivalDateFormattedShort { get; set; }


        public String OutboundArrivalTimeFormatted { get; set; }

        public int OutboundDuration { get; set; }

        public string OutboundDurationHourMinuteFormatted { get; set; }

        public int OutboundStopCount { get; set; }


        public List<Carrier> OutboundCarriers { get; set; }



        public String InboundOriginFormatted { get; set; }


        public Place InboundOriginPlace { get; set; }

        public String InboundOriginAirportCode { get; set; }


        public double InboundOriginAirportLatitude { get; set; }


        public double InboundOriginAirportLongitude { get; set; }


        public String InboundOriginAirport { get; set; }


        public String InboundOriginCity { get; set; }

        public double InboundOriginCityLatitude { get; set; }


        public double InboundOriginCityLongitude { get; set; }


        public String InboundOriginCountry { get; set; }


        public Place InboundOriginAirportPlace { get; set; }


        public Place InboundOriginCityPlace { get; set; }


        public Place InboundOriginCountryPlace { get; set; }



        public String InboundDestinationFormatted { get; set; }


        public Place InboundDestinationPlace { get; set; }


        public String InboundDestinationAirportCode { get; set; }

        public double InboundDestinationAirportLatitude { get; set; }

        public double InboundDestinationAirportLongitude { get; set; }


        public String InboundDestinationAirport { get; set; }


        public String InboundDestinationCity { get; set; }

        public double InboundDestinationCityLatitude { get; set; }

        public double InboundDestinationCityLongitude { get; set; }

        public String InboundDestinationCountry { get; set; }


        public Place InboundDestinationAirportPlace { get; set; }


        public Place InboundDestinationCityPlace { get; set; }


        public Place InboundDestinationCountryPlace { get; set; }



        public DateTime InboundDepartureDateTime { get; set; }

        public String InboundDepartureDateFormattedLong { get; set; }

        public String InboundDepartureDateFormattedShort { get; set; }


        public String InboundDepartureTimeFormatted { get; set; }

        public DateTime InboundArrivalDateTime { get; set; }


        public String InboundArrivalDateFormattedLong { get; set; }


        public String InboundArrivalDateFormattedShort { get; set; }


        public String InboundArrivalTimeFormatted { get; set; }



        public int InboundDuration { get; set; }

        public string InboundDurationHourMinuteFormatted { get; set; }

        public int InboundStopCount { get; set; }


        public List<Carrier> InboundCarriers { get; set; }
    }
}
