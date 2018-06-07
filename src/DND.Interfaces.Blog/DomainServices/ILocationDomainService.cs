using DND.Common.Interfaces.DomainServices;
using DND.Domain.Blog.Locations;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.DomainServices
{
    public interface ILocationDomainService : IBaseEntityDomainService<Location>
    {
        Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
