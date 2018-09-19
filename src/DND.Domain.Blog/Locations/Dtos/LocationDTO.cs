using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Domain.Blog.Locations.Enums;
using DND.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Globalization;

namespace DND.Domain.Blog.Locations.Dtos
{
    public class LocationDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        [Required]
        public string Name { get; set; }
        public LocationType LocationType { get; set; }
        [Render(ShowForGrid = false)]
        public string DescriptionBody { get; set; }
        public string TimeRequired { get; set; }
        public string Cost { get; set; }
        [UIHint("String")]
        public string LinkText { get; set; }
        public string Link { get; set; }
        public Boolean ShowOnTravelMap { get; set; }
        public Boolean CurrentLocation { get; set; }
        public Boolean NextLocation { get; set; }

        [Render(AllowSortForGrid = false)]
        //[Required]
        [FolderDropdown(Folders.Gallery, true)]
        public string Album { get; set; }

        //[Dropdown(typeof(User), nameof(User.Name))]
        //public string UserId { get; set; }
        public string PlaceId { get; set; }
        public DbGeography GPSLocation { get; set; }

        [StringLength(200, ErrorMessage = "UrlSlug: length should not exceed 200 characters")]
        public string UrlSlug { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        public LocationDto()
        {
            LocationType = LocationType.City;
            DateCreated = DateTime.Now;
        }

        public Boolean HasGPSCoordinates()
        {
            return GPSLocation != null && GPSLocation.Latitude.HasValue && GPSLocation.Longitude.HasValue;
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<LocationDto, Location>()
             .ForMember(bo => bo.DateModified, dto => dto.Ignore())
            .ForMember(bo => bo.DateCreated, dto => dto.Ignore())
            .ForMember(bo => bo.Latitude, dto => dto.MapFrom(s => s.GPSLocation != null ? s.GPSLocation.Latitude : null))
           .ForMember(bo => bo.Longitude, dto => dto.MapFrom(s => s.GPSLocation != null ? s.GPSLocation.Longitude : null));

            configuration.CreateMap<Location, LocationDto>()
             .ForMember(dto => dto.GPSLocation, bo => bo.MapFrom(s => s.Latitude.HasValue && s.Longitude.HasValue ?
             DbGeography.FromText(string.Format(CultureInfo.InvariantCulture, "POINT({1} {0})", s.Latitude.ToString(), s.Longitude.ToString()), DbGeography.DefaultCoordinateSystemId) : default(DbGeography)));
        }
    }
}