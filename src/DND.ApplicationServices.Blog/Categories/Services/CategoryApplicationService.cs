using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Categories.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Categories.Services
{
    public class CategoryApplicationService : ApplicationServiceEntityBase<Category, CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryDomainService>, ICategoryApplicationService
    {

        protected virtual ICategoryDomainService CategoryDomainService { get; }

        public CategoryApplicationService(ICategoryDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<CategoryDto>> hubContext)
        : base("blog.categories.", domainService, mapper, authorizationService, userService, hubContext)
        {
            CategoryDomainService = domainService;
        }

        public async Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            var bo = await CategoryDomainService.GetCategoryAsync(categorySlug, cancellationToken);
            return Mapper.Map<CategoryDto>(bo);
        }
    }
}