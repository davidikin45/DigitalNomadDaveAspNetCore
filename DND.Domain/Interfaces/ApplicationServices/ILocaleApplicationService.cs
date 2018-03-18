using DND.Domain.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ICurrencyApplicationService : IBaseApplicationService
    {
        Task<IEnumerable<CurrencyDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<CurrencyDTO> GetAsync(string id, CancellationToken cancellationToken);
    }
}
