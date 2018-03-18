using DND.Domain.Models;
using DND.Common.Interfaces.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface ILocationDomainService : IBaseEntityDomainService<Location>
    {
        Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken);
    }
}
