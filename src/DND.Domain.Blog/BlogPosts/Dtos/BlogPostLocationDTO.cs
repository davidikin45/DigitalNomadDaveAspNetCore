using AutoMapper;
using AutoMapper.EquivalencyExpression;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostLocationDto : BaseDto<int>, IMapFrom<BlogPostLocation>, IMapTo<BlogPostLocation>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required]
        [Dropdown(typeof(Location), "{" + nameof(DND.Domain.Blog.Locations.Location.LocationTypeString) + "} - {" + nameof(DND.Domain.Blog.Locations.Location.Name) + "}")]
        public int LocationId { get; set; }

        [HiddenInput()]
        public int BlogPostId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public LocationDto Location { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
