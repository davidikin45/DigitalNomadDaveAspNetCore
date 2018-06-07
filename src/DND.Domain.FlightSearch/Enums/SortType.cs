using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.FlightSearch.Enums
{
    public enum SortType
    {
        [Display(Name = "Price Per Passenger"), Description("Price")]
        Price,
        [Display(Name = "Outbound Departure Time"), Description("OutboundDepartureTime")]
        OutboundDeparture,
        [Display(Name = "Outbound Arrival Time"), Description("OutboundArrivalTime")]
        OutboundArrival,
        [Display(Name = "Outbound Duration"), Description("OutboundDuration")]
        OutboundDuration,
        [Display(Name = "Outbound Stops"), Description("OutboundStops")]
        OutboundStopsAsc,
        [Display(Name = "Inbound Departure Time"), Description("InboundDepartureTime")]
        InboundDeparture,
        [Display(Name = "Inbound Arrival Time"), Description("InboundArrivalTime")]
        InboundArrival,
        [Display(Name = "Inbound Duration"), Description("InboundDuration")]
        InboundDuration,
        [Display(Name = "Inbound Stops"), Description("InboundStops")]
        InboundStops
    }
}
