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
    public class FaqDTO : BaseEntity<int>, IMapTo<Faq>, IMapFrom<Faq>
    {
        [Required]
        public string Question { get; set; }

        [Required]
    
        [MultilineText(HTML = true)]
        public string Answer { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
