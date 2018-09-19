using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.Blog.Locations.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface ILocationApplicationService : IApplicationServiceEntity<LocationDto, LocationDto, LocationDto, LocationDeleteDto>
    {
        Task<LocationDto> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
