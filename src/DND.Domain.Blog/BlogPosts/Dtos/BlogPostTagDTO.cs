using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Domain.Blog.Tags;
using DND.Domain.Blog.Tags.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostTagDto : DtoBase<int>, IMapFrom<BlogPostTag>, IMapTo<BlogPostTag>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required]
        [Dropdown(typeof(Tag), nameof(DND.Domain.Blog.Tags.Tag.Name))]
        public int TagId { get; set; }

        [HiddenInput()]
        public int BlogPostId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public TagDto Tag { get; set; }

        [Render(ShowForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = false, ShowForEdit = true)]
        [Repeater("{" + nameof(BlogPostLocationDto.LocationId) + "}")]
        public List<BlogPostLocationDto> Locations { get; set; } = new List<BlogPostLocationDto>();

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
