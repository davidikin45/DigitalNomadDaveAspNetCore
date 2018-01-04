using DND.Domain.DTOs;
using Solution.Base.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.Services
{
    public interface ICategoryService : IBaseEntityService<CategoryDTO>
    {
        Task<CategoryDTO> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
