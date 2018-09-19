using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Dtos
{
    public class QuestionDeleteDto : DtoAggregateRootBase<int>,  IMapFrom<Question>, IMapTo<Question>
    {
        public QuestionDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
