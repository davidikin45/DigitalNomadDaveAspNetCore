using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostLocationDto : BaseDto<int>, IMapTo<BlogPostLocation>, IMapFrom<BlogPostLocation>, IHaveCustomMappings
    {
        [Required]
        public int LocationId { get; set; }

        public int BlogPostId { get; set; }

        public LocationDto Location { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
           //configuration.CreateMap<BlogPostLocation, int>()
           //.ConstructUsing(s => s.Location.Id);
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
