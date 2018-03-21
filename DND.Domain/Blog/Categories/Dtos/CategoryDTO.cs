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
    public class CategoryDto : BaseDto<int>, IMapFrom<Category>, IMapTo<Category>
    {

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        [Render(ShowForCreate = false, ShowForEdit = false, ShowForGrid = false, ShowForDisplay = false)]
        public int Count { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
