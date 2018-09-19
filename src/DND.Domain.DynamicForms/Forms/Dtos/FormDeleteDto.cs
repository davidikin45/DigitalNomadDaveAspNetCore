using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Forms.Dtos
{
    public class FormDeleteDto : DtoAggregateRootBase<int>,  IMapFrom<Form>, IMapTo<Form>
    {
        public FormDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
