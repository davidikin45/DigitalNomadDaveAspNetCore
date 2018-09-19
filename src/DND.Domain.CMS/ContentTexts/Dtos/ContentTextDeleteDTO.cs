using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentTexts.Dtos
{
    public class ContentTextDeleteDto : DtoAggregateRootBase<string>, IMapTo<ContentText>, IMapFrom<ContentText>
    {

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
