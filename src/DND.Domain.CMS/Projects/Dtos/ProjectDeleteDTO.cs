using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.Projects.Dtos
{
    public class ProjectDeleteDto : BaseDtoAggregateRoot<int>, IMapFrom<Project>, IMapTo<Project>
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
