using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.ModelImport
{
    public class Airports
    {
        public class Airport
        {
            public string ICAO { get; set; }
            public string IATA { get; set; }
            public string AirportName { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public int LatitudeDegrees { get; set; }
            public int LatitudeMinutes { get; set; }
            public int LatitudeSeconds { get; set; }
            public string LatitudeDirection { get; set; }
            public double LatitudeDecimalDegrees { get; set; }
            public int LongitudeDegrees { get; set; }
            public int LongitudeMinutes { get; set; }
            public int LongitudeSeconds { get; set; }
            public string LongitudeDirection { get; set; }
            public double LongitudeDecimalDegrees { get; set; }
            public int Altitude { get; set; }
        }

        public class RootObject
        {
            public List<Airport> Airports { get; set; }
        }
    }
}
