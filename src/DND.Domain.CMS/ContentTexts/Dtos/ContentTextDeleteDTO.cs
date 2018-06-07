using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentTexts.Dtos
{
    public class ContentTextDeleteDto : BaseDtoAggregateRoot<string>, IMapTo<ContentText>, IMapFrom<ContentText>
    {

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
