using DND.Domain.CMS.Testimonials.Dtos;
using System.Collections.Generic;

namespace DND.Web.Mvc.Testimonial.Models
{
    public class TestimonialsViewModel
    {
        public IList<TestimonialDto> Testimonials { get; set; }
    }
}
