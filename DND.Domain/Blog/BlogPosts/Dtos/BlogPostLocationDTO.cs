using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Domain.Blog.Locations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostLocationDto : BaseDto, IMapFrom<Location>, IMapTo<BlogPostLocation>
    {
        [Required]
        public int LocationId { get; set; }

        public int BlogPostId { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
