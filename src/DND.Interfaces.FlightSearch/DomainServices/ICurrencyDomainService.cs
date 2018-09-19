using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.DomainServices
{
    public interface ILocaleDomainService : IDomainService
    {
        Task<IEnumerable<Locale>> GetAllAsync(CancellationToken cancellationToken);
        Task<Locale> GetAsync(string id, CancellationToken cancellationToken);
    }
}
