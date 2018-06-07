using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentHtmls.Dtos
{
    public class ContentHtmlDeleteDto : BaseDtoAggregateRoot<string>, IMapTo<ContentHtml>, IMapFrom<ContentHtml>
    {

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
