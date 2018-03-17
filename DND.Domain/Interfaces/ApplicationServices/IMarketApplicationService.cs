using DND.Domain.DTOs;
using Solution.Base.Interfaces.ApplicationServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IMarketApplicationService : IBaseApplicationService
    {
        Task<IEnumerable<MarketDTO>> GetByLocale(string locale, CancellationToken cancellationToken);
        Task<MarketDTO> GetAsync(string id, CancellationToken cancellationToken);
    }
}
