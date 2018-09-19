using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data;
using DND.Domain.Blog.Tags;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Tags.Services
{
    public class TagDomainService : DomainServiceEntityBase<ApplicationContext, Tag>, ITagDomainService
    {
        public TagDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Tag> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, Tag>().GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(tagSlug)).ConfigureAwait(false);
            }
        }

        public async override Task<IEnumerable<Tag>> GetAsync(CancellationToken cancellationToken, Expression<Func<Tag, bool>> filter = null, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy = null, int? pageNo = null, int? pageSize = null, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<Tag, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, Tag>().GetAllAsync(cancellationToken, o => o.OrderBy(c => c.Name)).ConfigureAwait(false);
            }
        }

        public async override Task<Result> UpdateAsync(CancellationToken cancellationToken, Tag entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.UpdateAsync(cancellationToken, entity, updatedBy).ConfigureAwait(false);
        }

        public async override Task<Result<Tag>> CreateAsync(CancellationToken cancellationToken, Tag entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(cancellationToken, entity, createdBy).ConfigureAwait(false);
        }
    }
}