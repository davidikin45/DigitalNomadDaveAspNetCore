using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Domain.Blog.Tags;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostTagDto : BaseDto, IMapFrom<Tag>, IMapTo<BlogPostTag>
    {
        [Required]
        public int TagId { get; set; }

        public int BlogPostId { get; set; }

        public string Name { get; set; }
        public string UrlSlug { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
