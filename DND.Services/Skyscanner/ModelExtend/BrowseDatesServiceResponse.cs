using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DND.Services.Skyscanner.Model
{
    public partial class BrowseDatesServiceResponse
    {
        public BrowseDatesServiceResponse Filter(double? price)
        {
            var filterQuotes = Quotes.Where(q => (price == null || (q.MinPrice <= price && q.MinPrice != 0))).ToList();


            var filterQuotePlaces = Places.Where(p => filterQuotes.Any(fq => (fq.InboundLeg != null && fq.InboundLeg.OriginId.ToString() == p.PlaceId) || (fq.InboundLeg != null && fq.InboundLeg.DestinationId.ToString() == p.PlaceId) ||
                (fq.OutboundLeg != null && fq.OutboundLeg.OriginId.ToString() == p.PlaceId) || (fq.OutboundLeg != null && fq.OutboundLeg.DestinationId.ToString() == p.PlaceId))).ToList();

            var filterPlaces = filterQuotePlaces;

            var filterCarriers = Carriers.Where(c => filterQuotes.Any(fq => (fq.InboundLeg != null && fq.InboundLeg.CarrierIds.Contains(c.CarrierId)) || (fq.OutboundLeg != null && fq.OutboundLeg.CarrierIds.Contains(c.CarrierId)))).ToList();

            BrowseDatesServiceResponse filterObject = new BrowseDatesServiceResponse();
            filterObject.Quotes = filterQuotes;
            filterObject.Places = filterPlaces;
            filterObject.Carriers = filterCarriers;
            filterObject.Currencies = Currencies;

            return filterObject;
        }
    }
}
