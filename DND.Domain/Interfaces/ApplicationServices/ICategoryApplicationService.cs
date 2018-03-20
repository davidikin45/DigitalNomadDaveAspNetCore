using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.Blog.Categories.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ICategoryApplicationService : IBaseEntityApplicationService<CategoryDto>
    {
        Task<CategoryDto> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
