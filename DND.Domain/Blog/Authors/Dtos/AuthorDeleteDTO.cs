using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Authors.Dtos
{
    public class AuthorDeleteDto : BaseDtoAggregateRoot<int> , IMapFrom<Author>, IMapTo<Author>
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