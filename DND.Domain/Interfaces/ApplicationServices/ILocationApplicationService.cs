using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.Blog.Locations.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ILocationApplicationService : IBaseEntityApplicationService<LocationDto, LocationDto, LocationDto, LocationDeleteDto>
    {
        Task<LocationDto> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
