using DND.Domain.Models;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Common.Implementation.Dtos;

namespace DND.Domain.Blog.Categories.Dtos
{
    public class CategoryDeleteDto : BaseDtoAggregateRoot<int>, IMapFrom<Category>, IMapTo<Category>
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
