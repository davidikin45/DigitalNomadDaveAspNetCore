using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSectionSubmissions.Dtos
{
    public class FormSectionSubmissionDeleteDto : DtoAggregateRootBase<int>,  IMapFrom<FormSectionSubmission>, IMapTo<FormSectionSubmission>
    {
        public FormSectionSubmissionDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
