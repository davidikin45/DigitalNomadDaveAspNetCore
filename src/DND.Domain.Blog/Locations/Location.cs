using DND.Common.Domain;
using DND.Common.Extensions;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.Blog.Locations.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.Blog.Locations
{
    public class Location : EntityAggregateRootAuditableBase<int>
    {
		public string Name { get; set; }

        //https://stackoverflow.com/questions/47721246/ef-core-2-0-enums-stored-as-string
        public LocationType LocationType { get; set; } = LocationType.City;

        public string LocationTypeDescription => LocationType.GetDescription();

        public string DescriptionBody { get; set; }
        public string TimeRequired { get; set; }
        public string Cost { get; set; }
        public string LinkText { get; set; }
        public string Link { get; set; }
        public Boolean ShowOnTravelMap { get; set; }
        public Boolean CurrentLocation { get; set; }
        public Boolean NextLocation { get; set; }

        public string PlaceId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string Album { get; set; }

        public string UrlSlug { get; set; }

        //public string SkyscannerCode { get; set; }

        public Location()
		{
			
		}

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return await Task.FromResult(errors);
        }
    }
}