using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.Blog.Categories.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface ICategoryApplicationService : IApplicationServiceEntity<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto>
    {
        Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
