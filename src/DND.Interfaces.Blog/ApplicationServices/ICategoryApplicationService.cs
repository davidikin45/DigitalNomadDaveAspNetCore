using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.Blog.Categories.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface ICategoryApplicationService : IBaseEntityApplicationService<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto>
    {
        Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
