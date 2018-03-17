using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines.Interfaces;
using DND.Services;
using DND.Services.Factories;
using Solution.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
//using static DND.Services.LocationService;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Skyscanner.Model
{
    public partial class LivePricesServiceResponse
    {
        const string DateFormatLong = "ddd d MMM yyyy";
        const string DateFormatShort = "ddd dd/MM";
        const string TimeFormat = "HH:mm";

        public async Task<LivePricesServiceResponse> FilterAsync(string currency, double? minPrice, double? maxPrice, CancellationToken cancellationToken)
        {
            return await FilterAsync(currency, minPrice, maxPrice, null, null, null, null, null, null, null, null, null, null, cancellationToken);
        }

        public async Task<LivePricesServiceResponse> FilterAsync(string currency, double? minPrice, double? maxPrice, int? OutboundMaxStops,
            DateTime? OutboundDepartureDateTimeFrom, DateTime? OutboundDepartureDateTimeTo, DateTime? OutboundArrivalDateTimeFrom, DateTime? OutboundArrivalDateTimeTo,
            int? InboundMaxStops,
            DateTime? InboundDepartureDateTimeFrom, DateTime? InboundDepartureDateTimeTo, DateTime? InboundArrivalDateTimeFrom, DateTime? InboundArrivalDateTimeTo, CancellationToken cancellationToken)
        {
            IFlightSearchEngine flightSearhEngine = new FlightSearchEngineFactory().GetFlightSearchEngine("skyscanner");

            foreach (Segment s in Segments)
            {
                s.CarrierObject = Carriers.Where(c => c.Id == s.Carrier).FirstOrDefault();
                s.CarrierName = s.CarrierObject.Name;
                s.CarrierImageUrl = s.CarrierObject.ImageUrl;

                s.OriginPlace = Places.Where(p => p.Id == s.OriginStation).FirstOrDefault();
                s.DestinationPlace = Places.Where(p => p.Id == s.DestinationStation).FirstOrDefault();

                if (s.DepartureDateTime != null)
                {
                    s.DepartureDateTimeConverted = DateTime.Parse(s.DepartureDateTime);
                }

                if (s.ArrivalDateTime != null)
                {
                    s.ArrivalDateTimeConverted = DateTime.Parse(s.ArrivalDateTime);
                }
            }

            Currency searchCurrency = null;
            var filterCurrencies = Currencies.Where(c => currency == null || currency == "" || c.Code == currency).ToList();

            if (filterCurrencies.Count() == 1)
            {
                searchCurrency = filterCurrencies.FirstOrDefault();
            }

            var filterItineries = Itineraries.Where(i =>
                (
                    (maxPrice == null || (i.PricingOptions.Any(po => po.Price <= maxPrice && po.Price != 0)))
                )
                &&
                 (
                    (minPrice == null || (i.PricingOptions.Any(po => po.Price >= minPrice && po.Price != 0)))
                )
                &&
                (
                    (OutboundMaxStops == null || i.OutboundLegId == null || (Legs.Where(l => l.Id == i.OutboundLegId).First().Stops.Count() <= OutboundMaxStops))
                )
                &&
                (
                    (InboundMaxStops == null || i.InboundLegId == null || (Legs.Where(l => l.Id == i.InboundLegId).First().Stops.Count() <= InboundMaxStops))
                )
                &&
                (
                   (OutboundDepartureDateTimeFrom == null || Segments.Where(s => Legs.Where(l => l.Id == i.OutboundLegId).First().SegmentIds.First() == s.Id).First().DepartureDateTimeConverted >= OutboundDepartureDateTimeFrom)
                && (OutboundDepartureDateTimeTo == null || Segments.Where(s => Legs.Where(l => l.Id == i.OutboundLegId).First().SegmentIds.First() == s.Id).First().DepartureDateTimeConverted <= OutboundDepartureDateTimeTo)
                && (OutboundArrivalDateTimeFrom == null || Segments.Where(s => Legs.Where(l => l.Id == i.OutboundLegId).First().SegmentIds.First() == s.Id).Last().ArrivalDateTimeConverted >= OutboundArrivalDateTimeFrom)
                && (OutboundArrivalDateTimeTo == null || Segments.Where(s => Legs.Where(l => l.Id == i.OutboundLegId).First().SegmentIds.First() == s.Id).Last().ArrivalDateTimeConverted <= OutboundArrivalDateTimeTo)
                && (InboundDepartureDateTimeFrom == null || Segments.Where(s => Legs.Where(l => l.Id == i.InboundLegId).First().SegmentIds.First() == s.Id).First().DepartureDateTimeConverted >= InboundDepartureDateTimeFrom)
                && (InboundDepartureDateTimeTo == null || Segments.Where(s => Legs.Where(l => l.Id == i.InboundLegId).First().SegmentIds.First() == s.Id).First().DepartureDateTimeConverted <= InboundDepartureDateTimeTo)
                && (InboundArrivalDateTimeFrom == null || Segments.Where(s => Legs.Where(l => l.Id == i.InboundLegId).First().SegmentIds.First() == s.Id).Last().ArrivalDateTimeConverted >= InboundArrivalDateTimeFrom)
                && (InboundArrivalDateTimeTo == null || Segments.Where(s => Legs.Where(l => l.Id == i.InboundLegId).First().SegmentIds.First() == s.Id).Last().ArrivalDateTimeConverted <= InboundArrivalDateTimeTo)
                )).ToList();

            foreach (Itinerary i in filterItineries)
            {
                i.PricingOptions.RemoveAll(po => !((po.Price <= maxPrice && po.Price != 0) || maxPrice == null) || !((po.Price >= minPrice && po.Price != 0) || minPrice == null));
            }

            var filterLegs = Legs.Where(l => filterItineries.Any(i => i.OutboundLegId == l.Id || i.InboundLegId == l.Id)).ToList();

            var filterSegments = Segments.Where(s => filterLegs.Any(fl => fl.SegmentIds.Contains(s.Id))).ToList();

            var filterLegPlaces = Places.Where(p => filterLegs.Any(l => l.OriginStation == p.Id || l.DestinationStation == p.Id)).ToList();

            var filterSegmentPlaces = Places.Where(p => filterSegments.Any(s => s.OriginStation == p.Id || s.DestinationStation == p.Id)).ToList();

            var filterPlaces = filterLegPlaces.Union(filterSegmentPlaces, new Solution.Base.Helpers.Comparer<Place>((p1, p2) => p1.Id == p2.Id)).ToList();

            List<Place> newPlaces = new List<Place>();
            foreach (Place fp in filterPlaces)
            {
                if (fp.ParentId != 0)
                {
                    fp.ParentPlace = Places.Where(p => p.Id == fp.ParentId).FirstOrDefault();
                    if (!newPlaces.Any(p => p.Id == fp.ParentId))
                    {
                        newPlaces.Add(fp.ParentPlace);
                    }

                    if (fp.ParentPlace.ParentId != 0 && fp.ParentPlace.ParentPlace == null)
                    {
                        fp.ParentPlace.ParentPlace = Places.Where(p => p.Id == fp.ParentPlace.ParentId).FirstOrDefault();
                        if (!newPlaces.Any(p => p.Id == fp.ParentPlace.ParentId))
                        {
                            newPlaces.Add(fp.ParentPlace.ParentPlace);
                        }
                    }
                }
            }

            filterPlaces = filterPlaces.Union(newPlaces, new Solution.Base.Helpers.Comparer<Place>((p1, p2) => p1.Id == p2.Id)).ToList();

            foreach (Segment s in filterSegments)
            {
                if (s.OriginPlace != null)
                {
                    s.OriginAirport = s.OriginPlace.Name;
                    s.OriginAirportCode = s.OriginPlace.Code;
                    var originAirport = new AirportService().GetByIATA(s.OriginPlace.Code);
                    if (originAirport != null)
                    {
                        s.OriginAirportLatitude = originAirport.LatitudeDecimalDegrees;
                        s.OriginAirportLongitude = originAirport.LongitudeDecimalDegrees;
                    }

                    s.OriginCity = s.OriginPlace.ParentPlace.Name;
                    s.OriginCountry = s.OriginPlace.ParentPlace.ParentPlace.Name;
                }
                if (s.DestinationPlace != null)
                {
                    s.DestinationAirport = s.DestinationPlace.Name;
                    s.DestinationAirportCode = s.DestinationPlace.Code;
                    var destinationAirport = new AirportService().GetByIATA(s.DestinationPlace.Code);
                    if (destinationAirport != null)
                    {
                        s.DestinationAirportLatitude = destinationAirport.LatitudeDecimalDegrees;
                        s.DestinationAirportLongitude = destinationAirport.LongitudeDecimalDegrees;
                    }

                    s.DestinationCity = s.DestinationPlace.ParentPlace.Name;
                    s.DestinationCountry = s.DestinationPlace.ParentPlace.ParentPlace.Name;
                }

                TimeSpan dt = TimeSpan.FromMinutes(s.Duration);
                s.DurationHourMinuteFormatted = ((dt.Days * 24) + dt.Hours).ToString() + "h " + dt.Minutes.ToString() + "m";
                s.DepartureDateFormattedLong = s.DepartureDateTimeConverted.ToString(DateFormatLong);
                s.DepartureDateFormattedShort = s.DepartureDateTimeConverted.ToString(DateFormatShort);
                s.DepartureTimeFormatted = s.DepartureDateTimeConverted.ToString(TimeFormat);
                s.ArrivalDateFormattedLong = s.ArrivalDateTimeConverted.ToString(DateFormatLong);
                s.ArrivalDateFormattedShort = s.ArrivalDateTimeConverted.ToString(DateFormatShort);
                s.ArrivalTimeFormatted = s.ArrivalDateTimeConverted.ToString(TimeFormat);

                s.OriginFormatted = s.OriginAirport + ", " + s.OriginAirportCode + " (" + s.OriginCountry + ")";
                s.DestinationFormatted = s.DestinationAirport + ", " + s.DestinationAirportCode + " (" + s.DestinationCountry + ")";
            }

            foreach (Leg l in filterLegs)
            {
                List<Carrier> legCarriers = new List<Carrier>();
                List<Segment> legSegments = new List<Segment>();
                foreach (int sId in l.SegmentIds)
                {
                    var segment = filterSegments.Where(s => s.Id == sId).FirstOrDefault();
                    legSegments.Add(segment);
                }

                foreach (int cId in l.Carriers)
                {
                    var carrier = Carriers.Where(c => c.Id == cId).FirstOrDefault();
                    legCarriers.Add(carrier);
                }
                l.CarrierObjects = legCarriers;
                l.Segments = legSegments;

                l.OriginPlace = filterPlaces.Where(p => p.Id == l.OriginStation).FirstOrDefault();
                l.DestinationPlace = filterPlaces.Where(p => p.Id == l.DestinationStation).FirstOrDefault();

                l.OriginAirport = l.OriginPlace.Name;
                l.OriginAirportCode = l.OriginPlace.Code;
                var originAirport = new AirportService().GetByIATA(l.OriginPlace.Code);
                if (originAirport != null)
                {
                    l.OriginAirportLatitude = originAirport.LatitudeDecimalDegrees;
                    l.OriginAirportLongitude = originAirport.LongitudeDecimalDegrees;
                }

                l.OriginCity = l.OriginPlace.ParentPlace.Name;
                l.OriginCountry = l.OriginPlace.ParentPlace.ParentPlace.Name;

                l.DestinationAirport = l.DestinationPlace.Name;
                l.DestinationAirportCode = l.DestinationPlace.Code;
                var destinationAirport = new AirportService().GetByIATA(l.DestinationPlace.Code);
                if (destinationAirport != null)
                {
                    l.DestinationAirportLatitude = destinationAirport.LatitudeDecimalDegrees;
                    l.DestinationAirportLongitude = destinationAirport.LongitudeDecimalDegrees;
                }
                l.DestinationCity = l.DestinationPlace.ParentPlace.Name;
                l.DestinationCountry = l.DestinationPlace.ParentPlace.ParentPlace.Name;
                l.StopCount = l.Stops.Count();

                l.DepartureDateTimeConverted = DateTime.Parse(l.Departure);
                l.ArrivalDateTimeConverted = DateTime.Parse(l.Arrival);
                TimeSpan dt = TimeSpan.FromMinutes(l.Duration);
                l.DurationHourMinuteFormatted = ((dt.Days * 24) + dt.Hours).ToString() + "h " + dt.Minutes.ToString() + "m";
                if (l.StopCount == 0)
                {
                    l.DurationHourMinuteFormatted = l.DurationHourMinuteFormatted + ", Direct";
                }

                l.DepartureDateFormattedLong = l.DepartureDateTimeConverted.ToString(DateFormatLong);
                l.DepartureDateFormattedShort = l.DepartureDateTimeConverted.ToString(DateFormatShort);
                l.DepartureTimeFormatted = l.DepartureDateTimeConverted.ToString(TimeFormat);
                l.ArrivalDateFormattedLong = l.ArrivalDateTimeConverted.ToString(DateFormatLong);
                l.ArrivalDateFormattedShort = l.ArrivalDateTimeConverted.ToString(DateFormatShort);
                l.ArrivalTimeFormatted = l.ArrivalDateTimeConverted.ToString(TimeFormat);

                l.OriginFormatted = l.OriginAirport + ", " + l.OriginAirportCode + " (" + l.OriginCountry + ")";
                l.DestinationFormatted = l.DestinationAirport + ", " + l.DestinationAirportCode + " (" + l.DestinationCountry + ")";
            }

            foreach (Itinerary i in filterItineries)
            {
                if (i.OutboundLegId != null && i.OutboundLegId != "")
                {
                    i.OutboundLeg = filterLegs.Where(l => l.Id == i.OutboundLegId).FirstOrDefault();

                    i.OutboundOriginPlace = filterPlaces.Where(p => p.Id == (Legs.Where(l => l.Id == i.OutboundLegId).FirstOrDefault().OriginStation)).FirstOrDefault();
                    i.OutboundDestinationPlace = filterPlaces.Where(p => p.Id == (Legs.Where(l => l.Id == i.OutboundLegId).FirstOrDefault().DestinationStation)).FirstOrDefault();

                    i.OutboundDepartureDateTime = filterLegs.Where(l => l.Id == i.OutboundLegId).FirstOrDefault().DepartureDateTimeConverted;
                    i.OutboundArrivalDateTime = filterLegs.Where(l => l.Id == i.OutboundLegId).FirstOrDefault().ArrivalDateTimeConverted;
                    i.OutboundStopCount = filterLegs.Where(l => l.Id == i.OutboundLegId).FirstOrDefault().Stops.Count();
                    i.OutboundCarriers = i.OutboundLeg.CarrierObjects;
                    i.OutboundDuration = i.OutboundLeg.Duration;
                    i.OutboundDurationHourMinuteFormatted = i.OutboundLeg.DurationHourMinuteFormatted;

                    i.OutboundDepartureDateFormattedLong = i.OutboundLeg.DepartureDateFormattedLong;
                    i.OutboundDepartureDateFormattedShort = i.OutboundLeg.DepartureDateFormattedShort;
                    i.OutboundDepartureTimeFormatted = i.OutboundLeg.DepartureTimeFormatted;

                    i.OutboundArrivalDateFormattedLong = i.OutboundLeg.ArrivalDateFormattedLong;
                    i.OutboundArrivalDateFormattedShort = i.OutboundLeg.ArrivalDateFormattedShort;
                    i.OutboundArrivalTimeFormatted = i.OutboundLeg.ArrivalTimeFormatted;

                    i.OutboundOriginFormatted = i.OutboundLeg.OriginFormatted;
                    i.OutboundDestinationFormatted = i.OutboundLeg.DestinationFormatted;
                }
                if (i.InboundLegId != null && i.InboundLegId != "")
                {
                    i.ReturnFlight = true;
                    i.InboundLeg = filterLegs.Where(l => l.Id == i.InboundLegId).FirstOrDefault();

                    i.InboundOriginPlace = filterPlaces.Where(p => p.Id == (Legs.Where(l => l.Id == i.InboundLegId).FirstOrDefault().OriginStation)).FirstOrDefault();
                    i.InboundDestinationPlace = filterPlaces.Where(p => p.Id == (Legs.Where(l => l.Id == i.InboundLegId).FirstOrDefault().DestinationStation)).FirstOrDefault();

                    i.InboundDepartureDateTime = filterLegs.Where(l => l.Id == i.InboundLegId).FirstOrDefault().DepartureDateTimeConverted;
                    i.InboundArrivalDateTime = filterLegs.Where(l => l.Id == i.InboundLegId).FirstOrDefault().ArrivalDateTimeConverted;
                    i.InboundStopCount = filterLegs.Where(l => l.Id == i.InboundLegId).FirstOrDefault().Stops.Count();
                    i.InboundCarriers = i.InboundLeg.CarrierObjects;
                    i.InboundDuration = i.InboundLeg.Duration;
                    i.InboundDurationHourMinuteFormatted = i.InboundLeg.DurationHourMinuteFormatted;

                    i.InboundDepartureDateFormattedLong = i.InboundLeg.DepartureDateFormattedLong;
                    i.InboundDepartureDateFormattedShort = i.InboundLeg.DepartureDateFormattedShort;
                    i.InboundDepartureTimeFormatted = i.InboundLeg.DepartureTimeFormatted;

                    i.InboundArrivalDateFormattedLong = i.InboundLeg.ArrivalDateFormattedLong;
                    i.InboundArrivalDateFormattedShort = i.InboundLeg.ArrivalDateFormattedShort;
                    i.InboundArrivalTimeFormatted = i.InboundLeg.ArrivalTimeFormatted;

                    i.InboundOriginFormatted = i.InboundLeg.OriginFormatted;
                    i.InboundDestinationFormatted = i.InboundLeg.DestinationFormatted;
                }

                foreach (PricingOption p in i.PricingOptions)
                {
                    p.PriceFormatted = p.Price.ToString();
                    p.PriceRounded = (int)Math.Round(p.Price, 0, MidpointRounding.AwayFromZero);
                    p.PriceRoundedFormatted = p.PriceRounded.ToString();

                    if (searchCurrency != null)
                    {

                        string space = "";
                        if (searchCurrency.SpaceBetweenAmountAndSymbol)
                        {
                            space = " ";
                        }

                        p.PriceFormatted = String.Format("{0:#" + searchCurrency.ThousandsSeparator + "##0" + searchCurrency.DecimalSeparator + "#0}", Math.Round(p.Price, searchCurrency.DecimalDigits));

                        if (searchCurrency.SymbolOnLeft)
                        {
                            p.PriceFormatted = searchCurrency.Symbol + space + p.PriceFormatted;
                        }
                        else
                        {

                            p.PriceFormatted = p.PriceFormatted + space + searchCurrency.Symbol;
                        }

                        p.PriceRounded = (int)Math.Round(p.Price, searchCurrency.RoundingCoefficient, MidpointRounding.AwayFromZero);
                        p.PriceRoundedFormatted = String.Format("{0:#" + searchCurrency.ThousandsSeparator + "##0" + searchCurrency.DecimalSeparator + "##}", p.PriceRounded);

                        if (searchCurrency.SymbolOnLeft)
                        {
                            p.PriceRoundedFormatted = searchCurrency.Symbol + space + p.PriceRoundedFormatted;
                        }
                        else
                        {
                            p.PriceRoundedFormatted = p.PriceRoundedFormatted + space + searchCurrency.Symbol;
                        }
                    }
                }

                var bestPricingOption = i.PricingOptions.OrderBy(po => po.Price).ThenBy(po => po.QuoteAgeInMinutes).FirstOrDefault();
                i.BestPrice = bestPricingOption.Price;
                i.BestPriceRounded = bestPricingOption.PriceRounded;
                i.BestPriceFormatted = bestPricingOption.PriceFormatted;
                i.BestPriceRoundedFormatted = bestPricingOption.PriceRoundedFormatted;

                i.DeeplinkUrl = bestPricingOption.DeeplinkUrl;

                var bestPricingAgent = Agents.Where(a => a.Id == bestPricingOption.Agents.FirstOrDefault()).FirstOrDefault();
                i.BestPriceAgent = bestPricingAgent;
                i.Agent = bestPricingAgent.Name;
                i.AgentImageUrl = bestPricingAgent.ImageUrl;

                i.OutboundOriginAirport = i.OutboundOriginPlace.Name;
                i.OutboundOriginAirportPlace = i.OutboundOriginPlace;

                i.OutboundOriginAirportCode = i.OutboundOriginPlace.Code;
                var outboundOriginAirport = await flightSearhEngine.GetAirportByIDAsync(i.OutboundOriginPlace.Code, cancellationToken);
                if (outboundOriginAirport != null)
                {
                    i.OutboundOriginAirportLatitude = outboundOriginAirport.Latitude;
                    i.OutboundOriginAirportLongitude = outboundOriginAirport.Longitude;
                }
                i.OutboundOriginCity = i.OutboundOriginPlace.ParentPlace.Name;
                i.OutboundOriginCityPlace = i.OutboundOriginPlace.ParentPlace;

                var outboundOriginCity = await flightSearhEngine.GetCityByIDAsync(i.OutboundOriginCityPlace.Code, cancellationToken);
                if (outboundOriginCity != null)
                {
                    i.OutboundOriginCityLatitude = outboundOriginCity.Latitude;
                    i.OutboundOriginCityLongitude = outboundOriginCity.Longitude;
                }

                i.OutboundOriginCountry = i.OutboundOriginPlace.ParentPlace.ParentPlace.Name;
                i.OutboundOriginCountryPlace = i.OutboundOriginPlace.ParentPlace.ParentPlace;

                i.OutboundDestinationAirport = i.OutboundDestinationPlace.Name;
                i.OutboundDestinationAirportPlace = i.OutboundDestinationPlace;
                i.OutboundDestinationAirportCode = i.OutboundDestinationPlace.Code;
                var outboundDestinationAirport = await flightSearhEngine.GetAirportByIDAsync(i.OutboundDestinationPlace.Code, cancellationToken);
                if (outboundDestinationAirport != null)
                {
                    i.OutboundDestinationAirportLatitude = outboundDestinationAirport.Latitude;
                    i.OutboundDestinationAirportLongitude = outboundDestinationAirport.Longitude;
                }
                i.OutboundDestinationCity = i.OutboundDestinationPlace.ParentPlace.Name;
                i.OutboundDestinationCityPlace = i.OutboundDestinationPlace.ParentPlace;

                var outboundDestinationCity = await flightSearhEngine.GetCityByIDAsync(i.OutboundDestinationCityPlace.Code, cancellationToken);
                if (outboundDestinationCity != null)
                {
                    i.OutboundDestinationCityLatitude = outboundDestinationCity.Latitude;
                    i.OutboundDestinationCityLatitude = outboundDestinationCity.Longitude;
                }

                i.OutboundDestinationCountry = i.OutboundDestinationPlace.ParentPlace.ParentPlace.Name;
                i.OutboundDestinationCountryPlace = i.OutboundDestinationPlace.ParentPlace.ParentPlace;


                if (i.InboundLegId != null && i.InboundLegId != "")
                {
                    i.InboundOriginAirport = i.InboundOriginPlace.Name;
                    i.InboundOriginAirportPlace = i.InboundOriginPlace;

                    i.InboundOriginAirportCode = i.InboundOriginPlace.Code;
                    var inboundOriginAirport = await flightSearhEngine.GetAirportByIDAsync(i.InboundOriginPlace.Code, cancellationToken);
                    if (inboundOriginAirport != null)
                    {
                        i.InboundOriginAirportLatitude = inboundOriginAirport.Latitude;
                        i.InboundOriginAirportLongitude = inboundOriginAirport.Longitude;
                    }
                    i.InboundOriginCity = i.InboundOriginPlace.ParentPlace.Name;
                    i.InboundOriginCityPlace = i.InboundOriginPlace.ParentPlace;


                    var inboundOriginCity = await flightSearhEngine.GetCityByIDAsync(i.InboundOriginCityPlace.Code, cancellationToken);
                    if (inboundOriginCity != null)
                    {
                        i.InboundOriginCityLatitude = inboundOriginCity.Latitude;
                        i.InboundOriginCityLongitude = inboundOriginCity.Longitude;
                    }

                    i.InboundOriginCountry = i.InboundOriginPlace.ParentPlace.ParentPlace.Name;
                    i.InboundOriginCountryPlace = i.InboundOriginPlace.ParentPlace.ParentPlace;

                    i.InboundDestinationAirport = i.InboundDestinationPlace.Name;
                    i.InboundDestinationAirportPlace = i.InboundDestinationPlace;
                    i.InboundDestinationAirportCode = i.InboundDestinationPlace.Code;
                    var inboundDestinationAirport = await flightSearhEngine.GetAirportByIDAsync(i.InboundDestinationPlace.Code, cancellationToken);
                    if (inboundDestinationAirport != null)
                    {
                        i.InboundDestinationAirportLatitude = inboundDestinationAirport.Latitude;
                        i.InboundDestinationAirportLongitude = inboundDestinationAirport.Longitude;
                    }
                    i.InboundDestinationCity = i.InboundDestinationPlace.ParentPlace.Name;
                    i.InboundDestinationCityPlace = i.InboundDestinationPlace.ParentPlace;

                    var inboundDestinationCity = await flightSearhEngine.GetCityByIDAsync(i.InboundDestinationCityPlace.Code, cancellationToken);
                    if (inboundDestinationCity != null)
                    {
                        i.InboundDestinationCityLatitude = inboundDestinationCity.Latitude;
                        i.InboundDestinationCityLongitude = inboundDestinationCity.Longitude;
                    }

                    i.InboundDestinationCountry = i.InboundDestinationPlace.ParentPlace.ParentPlace.Name;
                    i.InboundDestinationCountryPlace = i.InboundDestinationPlace.ParentPlace.ParentPlace;
                }
            }

            var filterLegCarriers = Carriers.Where(c => filterLegs.Any(l => l.Carriers.Contains(c.Id))).ToList();

            var filterSegmentCarriers = Carriers.Where(c => filterSegments.Any(s => s.Carrier == c.Id)).ToList();

            var filterCarriers = filterLegCarriers.Union(filterSegmentCarriers, new Solution.Base.Helpers.Comparer<Carrier>((c1, c2) => c1.Id == c2.Id)).ToList();

            var filterAgents = Agents.Where(a => filterItineries.Any(i => i.PricingOptions.Any(po => po.Agents.Contains(a.Id)))).ToList();

            LivePricesServiceResponse filterObject = new LivePricesServiceResponse();
            filterObject.SessionKey = SessionKey;
            filterObject.Query = Query;
            filterObject.Status = Status;
            filterObject.Itineraries = filterItineries;
            filterObject.Legs = filterLegs;
            filterObject.Segments = filterSegments;
            filterObject.Carriers = filterCarriers;
            filterObject.Agents = filterAgents;
            filterObject.Places = filterPlaces;
            filterObject.Currencies = filterCurrencies;

            return filterObject;
        }

    }
}
