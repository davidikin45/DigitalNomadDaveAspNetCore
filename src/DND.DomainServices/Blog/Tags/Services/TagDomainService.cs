using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Domain.Blog.Tags;
using DND.Common.Implementation.Validation;
using System.ComponentModel.DataAnnotations;
using DND.Common.Enums;

namespace DND.DomainServices.Tags.Services
{
    public class TagDomainService : BaseEntityDomainService<IApplicationDbContext, Tag>, ITagDomainService
    {
        public TagDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Tag> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext,Tag>().GetFirstAsync(t => t.UrlSlug.Equals(tagSlug)).ConfigureAwait(false);
            }
        }

        public async override Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<Tag, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, Tag>().GetAllAsync(o => o.OrderBy(c => c.Name)).ConfigureAwait(false);
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