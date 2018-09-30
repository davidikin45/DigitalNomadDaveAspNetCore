using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.Testimonial.Controllers
{
    [Route("admin/cms/testimonials")]
    public class AdminTestimonialsController : MvcControllerEntityAuthorizeBase<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialApplicationService>
    {
        public AdminTestimonialsController(ITestimonialApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
