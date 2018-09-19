using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.DomainServices
{
    public interface ICurrencyDomainService : IDomainService
    {
        Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken);
        Task<Currency> GetAsync(string id, CancellationToken cancellationToken);
    }
}
