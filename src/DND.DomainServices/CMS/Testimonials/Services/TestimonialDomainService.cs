using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.Testimonials;
using System.Threading.Tasks;
using DND.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.DomainServices.CMS.Testimonials.Services
{
    public class TestimonialDomainService : BaseEntityDomainService<IApplicationDbContext, Testimonial>, ITestimonialDomainService
    {
        public TestimonialDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}