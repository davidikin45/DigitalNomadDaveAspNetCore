using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;

namespace DND.ApplicationServices
{
    public class TestimonialApplicationService : BaseEntityApplicationService<IApplicationDbContext, Testimonial, TestimonialDTO>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(ITestimonialDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
     
    }
}