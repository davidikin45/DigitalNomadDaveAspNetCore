using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.BlogPosts.Dtos
{
    public class BlogPostDeleteDto : BaseDtoAggregateRoot<int>,  IMapFrom<BlogPost>, IMapTo<BlogPost>
    {

        public BlogPostDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
