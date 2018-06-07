using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Categories.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Categories.Services
{
    public class CategoryApplicationService : BaseEntityApplicationService<Category, CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryDomainService>, ICategoryApplicationService
    {

        protected virtual ICategoryDomainService CategoryDomainService { get; }

        public CategoryApplicationService(ICategoryDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {
            CategoryDomainService = domainService;
        }

        public async Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            var bo = await CategoryDomainService.GetCategoryAsync(categorySlug, cancellationToken);
            return Mapper.Map<CategoryDto>(bo);
        }

        public async override Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<CategoryDto>, IOrderedQueryable<CategoryDto>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<CategoryDto, object>>[] includeProperties)
        {
            var categories = await CategoryDomainService.GetAllAsync(cancellationToken);
            return categories.ToList().Select(Mapper.Map<Category, CategoryDto>);
        }
    }
}