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
using DND.Domain.Blog.Categories;

namespace DND.DomainServices.Categories.Services
{
    public class CategoryDomainService : BaseEntityDomainService<IApplicationDbContext, Category>, ICategoryDomainService
    {
        public CategoryDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Category>().GetFirstAsync(c => c.UrlSlug.Equals(categorySlug));
            }
        }

        public async override Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<Category, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Category>().GetAllAsync(o => o.OrderBy(c => c.Name));
            }
        }

        public async override Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, cancellationToken);
        }
    }
}