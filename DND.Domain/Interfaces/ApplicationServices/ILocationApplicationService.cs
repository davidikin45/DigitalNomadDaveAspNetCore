
using DND.Domain.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ILocationApplicationService : IBaseEntityApplicationService<LocationDTO>
    {
        Task<LocationDTO> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
