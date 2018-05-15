using DND.Common.Implementation.Dtos;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.Testimonials.Dtos
{
    public class TestimonialDto : BaseDtoAggregateRoot<int>, IMapFrom<Testimonial>, IMapTo<Testimonial>
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
