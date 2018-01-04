
using DND.Domain.DTOs;
using Solution.Base.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.Services
{
    public interface ILocationService : IBaseEntityService<LocationDTO>
    {
        Task<LocationDTO> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
