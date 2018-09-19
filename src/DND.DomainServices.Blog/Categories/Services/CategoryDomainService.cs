using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data;
using DND.Domain.Blog.Categories;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Categories.Services
{
    public class CategoryDomainService : DomainServiceEntityBase<ApplicationContext, Category>, ICategoryDomainService
    {
        public CategoryDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, Category>().GetFirstAsync(cancellationToken, c => c.UrlSlug.Equals(categorySlug)).ConfigureAwait(false);
            }
        }

        public async override Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, int? pageNo = null, int? pageSize = null, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<Category, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, Category>().GetAllAsync(cancellationToken, o => o.OrderBy(c => c.Name)).ConfigureAwait(false);
            }
        }

        public async override Task<Result<Category>> CreateAsync(CancellationToken cancellationToken, Category entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(cancellationToken, entity, createdBy).ConfigureAwait(false);
        }

    }
}