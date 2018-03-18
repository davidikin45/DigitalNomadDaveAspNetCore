using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Enums;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.ViewModels
{
    public class FlightSearchClientRequestForm : BaseObjectValidatable, IMapTo<FlightSearchRequestDTO>, IHaveCustomMappings
    {
        [Required]
        public string Locale { get; set; }
        [Required]
        public string[] Market { get; set; }
        [Required]
        public string Currency { get; set; }
       

        public Boolean ReturnFlight { get; set; }

        [Required]
        public string OriginLocation { get; set; }

        [Required]
        public string DestinationLocation { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime OutboundDate { get; set; }

        [Display(Name = "Departure Time From")]
        public int? OutboundDepartureTimeFrom { get; set; }

        [Display(Name = "Departure Time To")]
        public int? OutboundDepartureTimeTo { get; set; }

        [Display(Name = "Arrival Time From")]
        public int? OutboundArrivalTimeFrom { get; set; }

        [Display(Name = "Arrival Time To")]
        public int? OutboundArrivalTimeTo { get; set; }

        [Display(Name = "Min Outbound Flight Duration")]
        public int? OutboundDurationMin { get; set; }

        [Display(Name = "Max Outbound Flight Duration")]
        public int? OutboundDurationMax { get; set; }

        [DataType(DataType.Date), Display(Name = "Return Date")]
        public DateTime? InboundDate { get; set; }

        [Display(Name = "Departure Time From")]
        public int? InboundDepartureTimeFrom { get; set; }

        [Display(Name = "Departure Time To")]
        public int? InboundDepartureTimeTo { get; set; }

        [Display(Name = "Arrival Time From")]
        public int? InboundArrivalTimeFrom { get; set; }

        [Display(Name = "Arrival Time To")]
        public int? InboundArrivalTimeTo { get; set; }

        [Display(Name = "Min Return Flight Duration")]
        public int? InboundDurationMin { get; set; }

        [Display(Name = "Max Return Flight Duration")]
        public int? InboundDurationMax { get; set; }

        [Required, Display(Name = "Adults")]
        public Adult Adults { get; set; }

        [Required, Display(Name = "Children")]
        public Children Children { get; set; }

        [Required, Display(Name = "Infants")]
        public Infant Infants { get; set; }

        [Required, Display(Name = "Flight Class"), EnumDatasource(Datasource = typeof(Enums.FlightClass))]
        public string FlightClass { get; set; }

        public Boolean DirectFlightsOnly { get; set; }

        [Display(Name = "Min Total Cost per Adult")]
        public double? PriceFilterPerAdultMin { get; set; }

        [Display(Name = "Max Total Cost per Adult")]
        public double? PriceFilterPerAdultMax { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }

        [EnumDatasource(Datasource = typeof(Enums.SortType))]
        public string SortType { get; set; }

        [EnumDatasource(Datasource = typeof(Enums.SortOrder))]
        public string SortOrder { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FlightSearchClientRequestForm, FlightSearchRequestDTO>()
            .ForMember(d => d.Markets, opt => opt.MapFrom(s => s.Market))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Currency))
            .ForMember(d => d.Locale, opt => opt.MapFrom(s => s.Locale))
            .ForMember(d => d.OriginLocation, opt => opt.MapFrom(s => s.OriginLocation))
            .ForMember(d => d.DestinationLocation, opt => opt.MapFrom(s => s.DestinationLocation))
            .ForMember(d => d.OutboundDate, opt => opt.MapFrom(s => s.OutboundDate))
            .ForMember(d => d.InboundDate, opt => opt.MapFrom(s => s.InboundDate))
            .ForMember(d => d.Adults, opt => opt.MapFrom(s => int.Parse(s.Adults.GetEnumDescription())))
            .ForMember(d => d.Children, opt => opt.MapFrom(s => int.Parse(s.Children.GetEnumDescription())))
            .ForMember(d => d.Infants, opt => opt.MapFrom(s => int.Parse(s.Infants.GetEnumDescription())))
            .ForMember(d => d.PriceMinFilter, opt => opt.MapFrom(s => s.PriceFilterPerAdultMin))
            .ForMember(d => d.PriceMaxFilter, opt => opt.MapFrom(s => s.PriceFilterPerAdultMax))
            .ForMember(d => d.MaxStopsFilter, opt => opt.MapFrom(s => s.DirectFlightsOnly ? 0 : new Nullable<int>() ))
            .ForMember(d => d.FlightClass, opt => opt.MapFrom(s => (FlightClass)Enum.Parse(typeof(FlightClass), s.FlightClass)))
            .ForMember(d => d.Skip, opt => opt.MapFrom(s => s.Skip))
            .ForMember(d => d.Take, opt => opt.MapFrom(s => s.Take))
            .ForMember(d => d.SortType, opt => opt.MapFrom(s => (SortType)Enum.Parse(typeof(SortType), s.SortType)))
            .ForMember(d => d.SortOrder, opt => opt.MapFrom(s => (SortOrder)Enum.Parse(typeof(SortOrder), s.SortOrder)))
            .ForMember(d => d.OutboundDepartureTimeFromFilter, opt => opt.MapFrom(s => s.OutboundDepartureTimeFrom.HasValue ? s.OutboundDate.AddMinutes(s.OutboundDepartureTimeFrom.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.OutboundDepartureTimeToFilter, opt => opt.MapFrom(s => s.OutboundDepartureTimeTo.HasValue ? s.OutboundDate.AddMinutes(s.OutboundDepartureTimeTo.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.OutboundArrivalTimeFromFilter, opt => opt.MapFrom(s => s.OutboundArrivalTimeFrom.HasValue ? s.OutboundDate.AddMinutes(s.OutboundArrivalTimeFrom.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.OutboundArrivalTimeToFilter, opt => opt.MapFrom(s => s.OutboundArrivalTimeTo.HasValue ? s.OutboundDate.AddMinutes(s.OutboundArrivalTimeTo.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.InboundDepartureTimeFromFilter, opt => opt.MapFrom(s => s.InboundDepartureTimeFrom.HasValue ? s.InboundDate.Value.AddMinutes(s.InboundDepartureTimeFrom.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.InboundDepartureTimeToFilter, opt => opt.MapFrom(s => s.InboundDepartureTimeTo.HasValue ? s.InboundDate.Value.AddMinutes(s.InboundDepartureTimeTo.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.InboundArrivalTimeFromFilter, opt => opt.MapFrom(s => s.InboundArrivalTimeFrom.HasValue ? s.InboundDate.Value.AddMinutes(s.InboundArrivalTimeFrom.Value) : new Nullable<DateTime>()))
            .ForMember(d => d.InboundArrivalTimeToFilter, opt => opt.MapFrom(s => s.InboundArrivalTimeTo.HasValue ? s.InboundDate.Value.AddMinutes(s.InboundArrivalTimeTo.Value) : new Nullable<DateTime>()))
                ;

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}