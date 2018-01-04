using DND.Domain.DTOs;
using Solution.Base.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.Services
{
    public interface IMarketService : IBaseBusinessService
    {
        Task<IEnumerable<MarketDTO>> GetByLocale(string locale, CancellationToken cancellationToken);
        Task<MarketDTO> GetAsync(string id, CancellationToken cancellationToken);
    }
}
