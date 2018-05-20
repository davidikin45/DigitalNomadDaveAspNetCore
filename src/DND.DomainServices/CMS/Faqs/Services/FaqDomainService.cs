using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.Faqs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Common.Enums;

namespace DND.DomainServices.CMS.Faqs.Services
{
    public class FaqDomainService : BaseEntityDomainService<IBaseDbContext, Faq>, IFaqDomainService
    {
        public FaqDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(Faq entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}
