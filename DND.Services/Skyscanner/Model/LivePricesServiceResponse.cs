using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DND.Services.Skyscanner.Model
{
    public partial class LivePricesServiceResponse
    {
        public string SessionKey { get; set; }
        public Query Query { get; set; }
        public string Status { get; set; }
        public List<Itinerary> Itineraries { get; set; }
        public List<Leg> Legs { get; set; }
        public List<Segment> Segments { get; set; }
        public List<Carrier> Carriers { get; set; }
        public List<Agent> Agents { get; set; }
        public List<Place> Places { get; set; }
        public List<Currency> Currencies { get; set; }
    }
}
