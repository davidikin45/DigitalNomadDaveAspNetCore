using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data;
using DND.Domain.Blog.Authors;
using DND.Interfaces.Blog.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Authors.Services
{
    public class AuthorDomainService : DomainServiceEntityBase<ApplicationContext, Author>, IAuthorDomainService
    {
        public AuthorDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, Author>().GetFirstAsync(cancellationToken, c => c.UrlSlug.Equals(authorSlug));
            }
        }

        public async override Task<Result> UpdateAsync(CancellationToken cancellationToken, Author entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.UpdateAsync(cancellationToken, entity, updatedBy).ConfigureAwait(false);
        }

        public async override Task<Result<Author>> CreateAsync(CancellationToken cancellationToken, Author entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(cancellationToken, entity, createdBy).ConfigureAwait(false);
        }
    }
}