using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solution.Base.Helpers;

namespace DND.Services.Skyscanner.Model
{
    public partial class BrowseRoutesServiceResponse
    {
        public BrowseRoutesServiceResponse Filter(double? minPrice, double? maxPrice, Boolean direct = false)
        {
           // var filterQuotes = Quotes.Where(q => ((maxPrice == null || (q.MinPrice <= maxPrice && q.MinPrice != 0)) && (minPrice == null || (q.MinPrice >= minPrice && q.MinPrice != 0)))).ToList();


            var filterQuotes = Quotes.Where(q => ((maxPrice == null || (q.MinPrice <= maxPrice && q.MinPrice != 0)) && (direct == false || q.Direct == direct))).ToList();

            var filterRoutes = Routes.Where(r => r.QuoteIds == null || filterQuotes.Any(q => r.QuoteIds != null && r.QuoteIds.Contains(q.QuoteId))).ToList();

            foreach (Route r in filterRoutes)
            {
                if (r.QuoteIds != null)
                {
                    r.QuoteIds.RemoveAll(q => !filterQuotes.Any(fq => fq.QuoteId == q));
                }
            }


            var filterRoutePlaces = Places.Where(p => filterRoutes.Any(fr => fr.OriginId.ToString() == p.PlaceId || fr.DestinationId.ToString() == p.PlaceId)).ToList();
            var filterQuotePlaces = Places.Where(p => filterQuotes.Any(fq => (fq.InboundLeg != null && fq.InboundLeg.OriginId.ToString() == p.PlaceId) || (fq.InboundLeg != null && fq.InboundLeg.DestinationId.ToString() == p.PlaceId) ||
                (fq.OutboundLeg != null && fq.OutboundLeg.OriginId.ToString() == p.PlaceId) || (fq.OutboundLeg != null && fq.OutboundLeg.DestinationId.ToString() == p.PlaceId))).ToList();

            var filterPlaces = filterRoutePlaces.Union(filterQuotePlaces, new Solution.Base.Helpers.Comparer<Place>((p1, p2) => p1.PlaceId == p2.PlaceId)).ToList();

            var filterCarriers = Carriers.Where(c => filterQuotes.Any(fq => (fq.InboundLeg != null && fq.InboundLeg.CarrierIds.Contains(c.CarrierId)) || (fq.OutboundLeg != null && fq.OutboundLeg.CarrierIds.Contains(c.CarrierId)))).ToList();

            BrowseRoutesServiceResponse filterObject = new BrowseRoutesServiceResponse();
            filterObject.Routes = filterRoutes;
            filterObject.Quotes = filterQuotes;
            filterObject.Places = filterPlaces;
            filterObject.Carriers = filterCarriers;
            filterObject.Currencies = Currencies;

            return filterObject;
        }

        public List<string> OriginAirports()
        {
            var airportPlaces = Places.Where(p => Routes.Where(r => r.OriginId.ToString() == p.PlaceId).ToList().Count() > 0 && p.Type == "Station").ToList();
            var result = airportPlaces.Select(p => p.SkyscannerCode).Distinct().ToList();
            return result;
        }

        public List<string> DestinationCitiesByOriginAirport(string originAirportId)
        {
            //var destinations = Quotes.Where(q => q.OutboundLeg != null).Where(q => Places.Where(p => p.PlaceId == q.OutboundLeg.OriginId).First().SkyscannerCode == originAirportId).Select(r => r.OutboundLeg.DestinationId).Distinct().ToList();
            //var airportPlaces = Places.Where(p => destinations.Contains(p.PlaceId)).ToList();
            //var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            //return result;

            var airportPlaces = Places.Where(p => Routes.Where(r => r.DestinationId.ToString() == p.PlaceId && Places.Where(o => r.OriginId.ToString() == o.PlaceId).FirstOrDefault().SkyscannerCode == originAirportId).ToList().Count() > 0 && p.Type != "Country").ToList();
            var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            return result;
        }

        public List<string> OriginCitiesByDestinationAirport(string destinationAirportId)
        {
            //var destinations = Quotes.Where(q => q.OutboundLeg != null).Where(q => Places.Where(p => p.PlaceId == q.OutboundLeg.OriginId).First().SkyscannerCode == originAirportId).Select(r => r.OutboundLeg.DestinationId).Distinct().ToList();
            //var airportPlaces = Places.Where(p => destinations.Contains(p.PlaceId)).ToList();
            //var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            //return result;

            var airportPlaces = Places.Where(p => Routes.Where(r => r.OriginId.ToString() == p.PlaceId && Places.Where(d => r.DestinationId.ToString() == d.PlaceId).FirstOrDefault().SkyscannerCode == destinationAirportId).ToList().Count() > 0 && p.Type != "Country").ToList();
            var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            return result;
        }

        public List<string> OriginCities()
        {

            //var origins = Quotes.Where(q => q.OutboundLeg != null).Select(r => r.OutboundLeg.OriginId).Distinct().ToList();
            //var airportPlaces = Places.Where(p => origins.Contains(p.PlaceId)).ToList();
            //var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            //return result;

            var airportPlaces = Places.Where(p => Routes.Where(r => r.OriginId.ToString() == p.PlaceId).ToList().Count() > 0 && p.Type != "Country").ToList();
            var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            return result;

        }

        public List<string> DestinationCitiesByOriginCity(string originCityId)
        {
            //var destinations = Quotes.Where(q => q.OutboundLeg != null).Where(q => Places.Where(p => p.PlaceId == q.OutboundLeg.OriginId).First().CityId == originCityId).Select(r => r.OutboundLeg.DestinationId).Distinct().ToList();
            //var airportPlaces = Places.Where(p => destinations.Contains(p.PlaceId)).ToList();
            //var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            //return result;

            var airportPlaces = Places.Where(p => Routes.Where(r => r.DestinationId.ToString() == p.PlaceId && Places.Where(o => r.OriginId.ToString() == o.PlaceId).FirstOrDefault().CityId == originCityId).ToList().Count() > 0 && p.Type != "Country").ToList();
            var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            return result;
        }

        public List<string> DestinationCities()
        {

            //var destinations = Quotes.Where(q => q.OutboundLeg != null).Select(r => r.OutboundLeg.DestinationId).Distinct().ToList();
            //var airportPlaces = Places.Where(p => destinations.Contains(p.PlaceId)).ToList();
            //var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            //return result;

            var airportPlaces = Places.Where(p => Routes.Where(r => r.DestinationId.ToString() == p.PlaceId).ToList().Count() > 0 && p.Type != "Country").ToList();
            var result = airportPlaces.Select(p => p.CityId).Distinct().ToList();
            return result;

        }

        public List<string> DestinationAirports()
        {

            //var destinations = Quotes.Where(q => q.OutboundLeg != null).Select(r => r.OutboundLeg.DestinationId).Distinct().ToList();
            //var airportPlaces = Places.Where(p => destinations.Contains(p.PlaceId)).ToList();
            //var result = airportPlaces.Select(p => p.SkyscannerCode).Distinct().ToList();
            //return result;

            var airportPlaces = Places.Where(p => Routes.Where(r => r.DestinationId.ToString() == p.PlaceId).ToList().Count() > 0 && p.Type == "Station").ToList();
            var result = airportPlaces.Select(p => p.SkyscannerCode).Distinct().ToList();
            return result;

        }

    }
}
