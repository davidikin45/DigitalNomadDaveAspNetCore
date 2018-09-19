using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.MailingLists.Dtos
{
    public class MailingListDeleteDto : DtoAggregateRootBase<int>, IMapTo<MailingList>, IMapFrom<MailingList>
    {
        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
