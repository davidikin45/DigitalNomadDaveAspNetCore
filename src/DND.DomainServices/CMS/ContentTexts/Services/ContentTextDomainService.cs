using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.ContentTexts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DND.Common.Enums;

namespace DND.DomainServices.CMS.ContentTexts.Services
{
    public class ContentTextDomainService : BaseEntityDomainService<IBaseDbContext, ContentText>, IContentTextDomainService
    {
        public ContentTextDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(ContentText entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}
