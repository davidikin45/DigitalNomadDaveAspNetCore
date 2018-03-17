
using DND.Domain.DTOs;
using Solution.Base.Interfaces.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ILocationApplicationService : IBaseEntityApplicationService<LocationDTO>
    {
        Task<LocationDTO> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
