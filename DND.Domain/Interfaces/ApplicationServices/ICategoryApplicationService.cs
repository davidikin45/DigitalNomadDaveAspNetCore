using DND.Domain.DTOs;
using Solution.Base.Interfaces.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ICategoryApplicationService : IBaseEntityApplicationService<CategoryDTO>
    {
        Task<CategoryDTO> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
