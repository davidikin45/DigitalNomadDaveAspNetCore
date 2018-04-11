using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Testimonial.Api
{
    [ApiVersion("1.0")]
    [Route("api/testimonials")]
    public class TestimonialsController : BaseEntityWebApiControllerAuthorize<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDto, ITestimonialApplicationService>
    {
        public TestimonialsController(ITestimonialApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
