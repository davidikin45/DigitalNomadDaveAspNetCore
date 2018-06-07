using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Categories;
using DND.Interfaces.Blog.Data;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Categories.Services
{
    public class CategoryDomainService : BaseEntityDomainService<IBlogDbContext, Category>, ICategoryDomainService
    {
        public CategoryDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IBlogDbContext, Category>().GetFirstAsync(c => c.UrlSlug.Equals(categorySlug)).ConfigureAwait(false);
            }
        }

        public async override Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<Category, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IBlogDbContext, Category>().GetAllAsync(o => o.OrderBy(c => c.Name)).ConfigureAwait(false);
            }
        }

        public async override Task<Result<Category>> CreateAsync(Category entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, createdBy, cancellationToken).ConfigureAwait(false);
        }

    }
}