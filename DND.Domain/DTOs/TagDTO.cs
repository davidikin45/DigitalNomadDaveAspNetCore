using DND.Domain.Models;
using Solution.Base.Implementation.Models;
using Solution.Base.Interfaces.Automapper;
using Solution.Base.ModelMetadataCustom;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class TagDTO : BaseEntity<int>, IMapFrom<Tag>, IMapTo<Tag>
    {

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        [Render(ShowForCreate = false,ShowForEdit = false, ShowForGrid = false, ShowForDisplay = false)]
        public int Count { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
