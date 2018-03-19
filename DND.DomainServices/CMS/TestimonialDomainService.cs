using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;

namespace DND.ApplicationServices
{
    public class TestimonialDomainService : BaseEntityDomainService<IApplicationDbContext, Testimonial>, ITestimonialDomainService
    {
        public TestimonialDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
     
    }
}