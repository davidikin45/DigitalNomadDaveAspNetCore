using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.MailingLists;
using System.Threading.Tasks;
using System.Collections.Generic;
using DND.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace DND.DomainServices.CMS.MailingLists.Services
{
    public class MailingListDomainService : BaseEntityDomainService<IBaseDbContext, MailingList>, IMailingListDomainService
    {
        public MailingListDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(MailingList entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}
