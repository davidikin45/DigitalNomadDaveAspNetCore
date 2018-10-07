using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.Testimonial.Api
{
    [ApiVersion("1.0")]
    [Route("api/cms/testimonials")]
    public class TestimonialsController : ApiControllerEntityAuthorizeBase<TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialApplicationService>
    {
        public TestimonialsController(ITestimonialApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IAuthorizationService authorizationService)
            : base("cms.testimonials.", service, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService)
        {

        }
    }
}
