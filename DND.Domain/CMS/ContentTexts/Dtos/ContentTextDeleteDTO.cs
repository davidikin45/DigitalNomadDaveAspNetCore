using DND.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Common.Implementation.Dtos;

namespace DND.Domain.CMS.ContentTexts.Dtos
{
    public class ContentTextDeleteDto : BaseDtoAggregateRoot<string>, IMapTo<ContentText>, IMapFrom<ContentText>
    {

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
