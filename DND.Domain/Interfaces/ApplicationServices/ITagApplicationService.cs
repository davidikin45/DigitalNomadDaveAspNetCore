using DND.Domain.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ITagApplicationService : IBaseEntityApplicationService<TagDTO>
    {
        Task<TagDTO> GetTagAsync(string tagSlug, CancellationToken cancellationToken);
    }
}
