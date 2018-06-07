using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.Testimonials;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;

namespace DND.ApplicationServices.CMS.Testimonials.Services
{
    public class TestimonialApplicationService : BaseEntityApplicationService<Testimonial, TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialDomainService>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(ITestimonialDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}