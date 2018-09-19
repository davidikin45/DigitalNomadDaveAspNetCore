using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.FormSubmissions.Dtos
{
    public class FormSubmissionDeleteDto : DtoAggregateRootBase<Guid>,  IMapFrom<FormSubmission>, IMapTo<FormSubmission>
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
