using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Services.Skyscanner.Model
{
    public partial class Itinerary
    {

        public string OutboundLegId { get; set; }

        public string InboundLegId { get; set; }

        public List<PricingOption> PricingOptions { get; set; }

        public BookingDetailsLink BookingDetailsLink { get; set; }
    }
}
