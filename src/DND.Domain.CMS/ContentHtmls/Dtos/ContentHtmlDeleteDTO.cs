using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentHtmls.Dtos
{
    public class ContentHtmlDeleteDto : DtoAggregateRootBase<string>, IMapTo<ContentHtml>, IMapFrom<ContentHtml>
    {

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
