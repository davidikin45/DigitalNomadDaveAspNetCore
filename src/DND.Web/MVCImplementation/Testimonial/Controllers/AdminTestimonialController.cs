using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Testimonial.Controllers
{
    [Route("admin/testimonial")]
    public class AdminTestimonialController : BaseEntityControllerAuthorize<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialApplicationService>
    {
        public AdminTestimonialController(ITestimonialApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
