using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.DomainServices
{
    public interface IMarketDomainService : IDomainService
    {
        Task<IEnumerable<Country>> GetByLocale(string locale, CancellationToken cancellationToken);
        Task<Country> GetAsync(string id, CancellationToken cancellationToken);
    }
}
