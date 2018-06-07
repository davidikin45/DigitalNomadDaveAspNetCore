using DND.Domain.Skyscanner.Model;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.SearchEngines.Interfaces
{
    public interface ILocationSearchEngine
    {
        Task<LocationServiceResponse> SearchByQueryAsync(string country, string currency, string locale, string query, CancellationToken cancellationToken);

        Task<Place> GetByIDAsync(string country, string currency, string locale, string id, CancellationToken cancellationToken);

    }
}
