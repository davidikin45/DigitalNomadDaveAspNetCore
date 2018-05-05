using DND.Domain.CMS.Testimonials.Dtos;
using System.Collections.Generic;

namespace DND.Web.MVCImplementation.Testimonial.Models
{
    public class TestimonialsViewModel
    {
        public IList<TestimonialDto> Testimonials { get; set; }
    }
}
