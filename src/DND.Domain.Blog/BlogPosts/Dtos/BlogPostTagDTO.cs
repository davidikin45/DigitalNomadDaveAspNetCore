using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Blog.Tags;
using DND.Domain.Blog.Tags.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostTagDto : BaseDto<int>, IMapFrom<BlogPostTag>, IMapTo<BlogPostTag>, IHaveCustomMappings
    {
        [Required]
        [Dropdown(typeof(Tag), nameof(DND.Domain.Blog.Tags.Tag.Name))]
        public int TagId { get; set; }

        public int BlogPostId { get; set; }

        [Render(ShowForGrid = false, ShowForDisplay = false)]
        public TagDto Tag { get; set; }

        //public string Name { get; set; }
        //public string UrlSlug { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            //configuration.CreateMap<BlogPostTag, int>()
            //.ConstructUsing(s => s.Tag.Id);
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
