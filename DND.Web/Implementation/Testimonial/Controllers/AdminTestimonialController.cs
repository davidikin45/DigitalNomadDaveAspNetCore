using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Testimonial.Controllers
{
    [Route("admin/testimonial")]
    public class AdminTestimonialController : BaseEntityControllerAuthorize<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDto, ITestimonialApplicationService>
    {
        public AdminTestimonialController(ITestimonialApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
