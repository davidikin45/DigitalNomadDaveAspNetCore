using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.Testimonials;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.Testimonials.Services
{
    public class TestimonialDomainService : BaseEntityDomainService<ICMSDbContext, Testimonial>, ITestimonialDomainService
    {
        public TestimonialDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}