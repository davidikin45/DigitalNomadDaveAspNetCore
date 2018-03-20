using DND.Common.Implementation.Models;
using DND.Domain.Blog.Tags;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPostTag : BaseEntityAuditable<int>
    {
        [Required]
        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        [Required]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public BlogPostTag()
		{
			
		}

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}