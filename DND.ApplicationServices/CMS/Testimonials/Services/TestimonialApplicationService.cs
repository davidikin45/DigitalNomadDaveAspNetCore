using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.Testimonials;
using DND.Domain.CMS.Testimonials.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;

namespace DND.ApplicationServices.CMS.Testimonials.Services
{
    public class TestimonialApplicationService : BaseEntityApplicationService<IApplicationDbContext, Testimonial, TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, ITestimonialDomainService>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(ITestimonialDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}