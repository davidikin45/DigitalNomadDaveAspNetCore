using DND.Domain.DTOs;
using Solution.Base.Interfaces.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ITagApplicationService : IBaseEntityApplicationService<TagDTO>
    {
        Task<TagDTO> GetTagAsync(string tagSlug, CancellationToken cancellationToken);
    }
}
