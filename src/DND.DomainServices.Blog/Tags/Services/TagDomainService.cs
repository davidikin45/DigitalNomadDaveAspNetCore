using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Tags;
using DND.Interfaces.Blog.Data;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Tags.Services
{
    public class TagDomainService : BaseEntityDomainService<IBlogDbContext, Tag>, ITagDomainService
    {
        public TagDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Tag> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IBlogDbContext, Tag>().GetFirstAsync(t => t.UrlSlug.Equals(tagSlug)).ConfigureAwait(false);
            }
        }

        public async override Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<Tag, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IBlogDbContext, Tag>().GetAllAsync(o => o.OrderBy(c => c.Name)).ConfigureAwait(false);
            }
        }

        public async override Task<Result> UpdateAsync(Tag entity, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.UpdateAsync(entity, updatedBy, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<Result<Tag>> CreateAsync(Tag entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, createdBy, cancellationToken).ConfigureAwait(false);
        }
    }
}