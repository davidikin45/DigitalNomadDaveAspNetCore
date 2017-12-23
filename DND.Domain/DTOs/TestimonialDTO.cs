using DND.Base.Implementation.Models;
using DND.Base.Interfaces.Automapper;
using DND.Base.ModelMetadata;
using DND.Domain.Constants;
using DND.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class TestimonialDTO : BaseEntity<int>, IMapFrom<Testimonial>, IMapTo<Testimonial>
    {
        [Required, StringLength(100)]
        public string Source { get; set; }

        [Required, StringLength(5000)]
        public string QuoteText { get; set; }

        [Render(AllowSortForGrid = false)]
        [FileDropdown(Folders.Testimonials, true)]
        public string File { get; set; }

        [Render(ShowForEdit = true, ShowForCreate = false, ShowForGrid = true)]
        public DateTime DateCreated { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
