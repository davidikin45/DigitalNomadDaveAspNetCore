using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data;
using DND.Domain.CMS.Testimonials;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.Testimonials.Services
{
    public class TestimonialDomainService : DomainServiceEntityBase<ApplicationContext, Testimonial>, ITestimonialDomainService
    {
        public TestimonialDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}