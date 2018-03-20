using DND.Common.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface ICurrencyDomainService : IBaseDomainService
    {
        Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken);
        Task<Currency> GetAsync(string id, CancellationToken cancellationToken);
    }
}
