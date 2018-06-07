using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.Faqs.Dtos
{
    public class FaqDeleteDto : BaseDtoAggregateRoot<int>, IMapTo<Faq>, IMapFrom<Faq>
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
