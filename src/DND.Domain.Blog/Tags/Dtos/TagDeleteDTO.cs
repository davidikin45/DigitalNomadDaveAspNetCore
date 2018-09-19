using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Tags.Dtos
{
    public class TagDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Tag>, IMapTo<Tag>
    {

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
