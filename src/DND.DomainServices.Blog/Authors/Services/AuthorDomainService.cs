using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Authors;
using DND.Interfaces.Blog.Data;
using DND.Interfaces.Blog.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Authors.Services
{
    public class AuthorDomainService : BaseEntityDomainService<IBlogDbContext, Author>, IAuthorDomainService
    {
        public AuthorDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IBlogDbContext, Author>().GetFirstAsync(c => c.UrlSlug.Equals(authorSlug));
            }
        }

        public async override Task<Result> UpdateAsync(Author entity, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.UpdateAsync(entity, updatedBy, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<Result<Author>> CreateAsync(Author entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, createdBy, cancellationToken).ConfigureAwait(false);
        }
    }
}