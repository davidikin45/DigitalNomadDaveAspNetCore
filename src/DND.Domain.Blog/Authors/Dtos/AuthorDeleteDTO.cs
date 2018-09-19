using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Authors.Dtos
{
    public class AuthorDeleteDto : DtoAggregateRootBase<int> , IMapFrom<Author>, IMapTo<Author>
    {
        public AuthorDeleteDto()
		{

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}