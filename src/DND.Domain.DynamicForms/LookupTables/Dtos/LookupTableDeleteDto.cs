using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.LookupTables.Dtos
{
    public class LookupTableDeleteDto : BaseDtoAggregateRoot<int>,  IMapFrom<LookupTable>, IMapTo<LookupTable>
    {
        public LookupTableDeleteDto()
        {
        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

    }
}
