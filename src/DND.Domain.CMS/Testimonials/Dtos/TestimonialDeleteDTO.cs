using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.Testimonials.Dtos
{
    public class TestimonialDeleteDto : BaseDtoAggregateRoot<int>, IMapFrom<Testimonial>, IMapTo<Testimonial>
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
