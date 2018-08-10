using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSubmissions.Dtos
{
    public class FormSubmissionDeleteDto : BaseDtoAggregateRoot<Guid>,  IMapFrom<FormSubmission>, IMapTo<FormSubmission>
    {
        public FormSubmissionDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
