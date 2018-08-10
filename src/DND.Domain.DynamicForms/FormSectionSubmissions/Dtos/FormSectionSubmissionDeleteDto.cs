using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSectionSubmissions.Dtos
{
    public class FormSectionSubmissionDeleteDto : BaseDtoAggregateRoot<int>,  IMapFrom<FormSectionSubmission>, IMapTo<FormSectionSubmission>
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
