using DND.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Models.TestimonialViewModels
{
    public class TestimonialsViewModel
    {
        public IList<TestimonialDTO> Testimonials { get; set; }
    }
}
