using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Domain.DTOs;
using DND.Domain.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Implementation.Validation;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Services
{
    public class ContentHtmlService : BaseEntityService<IBaseDbContext, ContentHtml, ContentHtmlDTO>, IContentHtmlService
    {
        public ContentHtmlService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public override Task DeleteAsync(ContentHtmlDTO dto, CancellationToken cancellationToken)
        {
            if(dto.PreventDelete)
            {
               throw new ServiceValidationErrors(new GeneralError("This CMS content cannot be deleted"));
            }

            return base.DeleteAsync(dto, cancellationToken);
        }
    }
}
