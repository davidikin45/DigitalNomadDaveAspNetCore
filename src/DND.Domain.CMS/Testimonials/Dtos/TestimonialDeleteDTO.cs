using DND.Common.Domain.Dtos;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.Testimonials.Dtos
{
    public class TestimonialDeleteDto : DtoAggregateRootBase<int>, IMapFrom<Testimonial>, IMapTo<Testimonial>
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
