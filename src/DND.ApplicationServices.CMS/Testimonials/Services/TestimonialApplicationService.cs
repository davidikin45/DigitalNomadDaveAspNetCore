using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.CMS.Testimonials;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Testimonials.Services
{
    public class TestimonialApplicationService : ApplicationServiceEntityBase<Testimonial, TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialDomainService>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(ITestimonialDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<TestimonialDto>> hubContext)
        : base("cms.testimonials.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
     
    }
}