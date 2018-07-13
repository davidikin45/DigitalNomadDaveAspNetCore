using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Locations.Enums;
using DND.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;

namespace DND.Domain.Blog.Locations.Dtos
{
    public class LocationDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
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
            .ForMember(bo => bo.DateCreated, dto => dto.Ignore());

            configuration.CreateMap<Location, LocationDto>();
        }
    }
}