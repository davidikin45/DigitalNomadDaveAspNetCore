using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Domain.DynamicForms.Sections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Sections.Dtos
{
    public class SectionDeleteDto : BaseDtoAggregateRoot<int>,  IMapFrom<Section>, IMapTo<Section>
    {
        public SectionDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
