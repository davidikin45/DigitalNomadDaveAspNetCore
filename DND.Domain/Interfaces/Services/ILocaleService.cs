using DND.Domain.DTOs;
using Solution.Base.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.Services
{
    public interface ICurrencyService : IBaseBusinessService
    {
        Task<IEnumerable<CurrencyDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<CurrencyDTO> GetAsync(string id, CancellationToken cancellationToken);
    }
}
